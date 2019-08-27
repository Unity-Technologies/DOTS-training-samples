using System;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Entities
{
    internal unsafe partial struct EntityComponentStore
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------

        public void CreateEntities(Archetype* archetype, Entity* entities, int count)
        {
            var sharedComponentValues = stackalloc int[archetype->NumSharedComponents];
            UnsafeUtility.MemClear(sharedComponentValues, archetype->NumSharedComponents * sizeof(int));

            while (count != 0)
            {
                var chunk = GetChunkWithEmptySlots(archetype, sharedComponentValues);

                int allocatedIndex;
                var allocatedCount = AllocateIntoChunk(chunk, count, out allocatedIndex);
                AllocateEntities(archetype, chunk, allocatedIndex, allocatedCount, entities);
                ChunkDataUtility.InitializeComponents(chunk, allocatedIndex, allocatedCount);
                chunk->SetAllChangeVersions(GlobalSystemVersion);
                entities += allocatedCount;
                count -= allocatedCount;
            }

            IncrementComponentTypeOrderVersion(archetype);
        }

        public void DestroyEntities(NativeArray<ArchetypeChunk> chunkArray)
        {
            var chunks = (ArchetypeChunk*) chunkArray.GetUnsafeReadOnlyPtr();
            for (int i = 0; i != chunkArray.Length; i++)
            {
                var chunk = chunks[i].m_Chunk;
                DestroyBatch((Entity*) chunk->Buffer, chunk, 0, chunk->Count);
            }
        }

        public void DestroyEntities(Entity* entities, int count)
        {
            var entityIndex = 0;

            var additionalDestroyList = new UnsafeList(Allocator.Persistent);
            int minDestroyStride = int.MaxValue;
            int maxDestroyStride = 0;

            while (entityIndex != count)
            {
                var entityBatchInChunk =
                    GetFirstEntityBatchInChunk(entities + entityIndex, count - entityIndex);
                var chunk = entityBatchInChunk.Chunk;
                var batchCount = entityBatchInChunk.Count;
                var indexInChunk = entityBatchInChunk.StartIndex;

                if (chunk == null)
                {
                    entityIndex += batchCount;
                    continue;
                }

                AddToDestroyList(chunk, indexInChunk, batchCount, count, ref additionalDestroyList,
                    ref minDestroyStride, ref maxDestroyStride);

                DestroyBatch(entities + entityIndex, chunk, indexInChunk, batchCount);

                entityIndex += batchCount;
            }

            // Apply additional destroys from any LinkedEntityGroup
            if (additionalDestroyList.Ptr != null)
            {
                var additionalDestroyPtr = (Entity*) additionalDestroyList.Ptr;
                // Optimal for destruction speed is if entities with same archetype/chunk are followed one after another.
                // So we lay out the to be destroyed objects assuming that the destroyed entities are "similar":
                // Reorder destruction by index in entityGroupArray...

                //@TODO: This is a very specialized fastpath that is likely only going to give benefits in the stress test.
                ///      Figure out how to make this more general purpose.
                if (minDestroyStride == maxDestroyStride)
                {
                    var reordered = (Entity*) UnsafeUtility.Malloc(additionalDestroyList.Length * sizeof(Entity), 16,
                        Allocator.TempJob);
                    int batchCount = additionalDestroyList.Length / minDestroyStride;
                    for (int i = 0; i != batchCount; i++)
                    {
                        for (int j = 0; j != minDestroyStride; j++)
                            reordered[j * batchCount + i] = additionalDestroyPtr[i * minDestroyStride + j];
                    }

                    DestroyEntities(reordered, additionalDestroyList.Length);
                    UnsafeUtility.Free(reordered, Allocator.TempJob);
                }
                else
                {
                    DestroyEntities(additionalDestroyPtr, additionalDestroyList.Length);
                }

                UnsafeUtility.Free(additionalDestroyPtr, Allocator.Persistent);
            }
        }

        public void SetChunkCountKeepMetaChunk(Chunk* chunk, int newCount)
        {
            Assert.AreNotEqual(newCount, chunk->Count);
            Assert.IsFalse(chunk->Locked);
            Assert.IsTrue(!chunk->LockedEntityOrder || newCount == 0);

            // Chunk released to empty chunk pool
            if (newCount == 0)
            {
                ReleaseChunk(chunk);
                return;
            }

            var capacity = chunk->Capacity;

            // Chunk is now full
            if (newCount == capacity)
            {
                // this chunk no longer has empty slots, so it shouldn't be in the empty slot list.
                chunk->Archetype->EmptySlotTrackingRemoveChunk(chunk);
            }
            // Chunk is no longer full
            else if (chunk->Count == capacity)
            {
                Assert.IsTrue(newCount < chunk->Count);
                chunk->Archetype->EmptySlotTrackingAddChunk(chunk);
            }

            chunk->Count = newCount;
            chunk->Archetype->Chunks.SetChunkEntityCount(chunk->ListIndex, newCount);
        }

        public void SetChunkCount(Chunk* chunk, int newCount)
        {
            var metaChunkEntity = chunk->metaChunkEntity;
            if (newCount == 0 && metaChunkEntity != Entity.Null)
                DestroyMetaChunkEntity(metaChunkEntity);

             SetChunkCountKeepMetaChunk(chunk, newCount);
        }

        public void CreateChunks(Archetype* archetype, ArchetypeChunk* chunks, int entityCount)
        {
            fixed (EntityComponentStore* entityComponentStore = &this)
            {
                int* sharedComponentValues = stackalloc int[archetype->NumSharedComponents];
                UnsafeUtility.MemClear(sharedComponentValues, archetype->NumSharedComponents * sizeof(int));

                int chunkIndex = 0;
                while (entityCount != 0)
                {
                    var chunk = GetCleanChunk(archetype, sharedComponentValues);
                    int allocatedIndex;

                    var allocatedCount = AllocateIntoChunk(chunk, entityCount, out allocatedIndex);

                    AllocateEntities(archetype, chunk, allocatedIndex, allocatedCount, null);
                    ChunkDataUtility.InitializeComponents(chunk, allocatedIndex, allocatedCount);
                    chunk->SetAllChangeVersions(GlobalSystemVersion);
                    chunks[chunkIndex] = new ArchetypeChunk(chunk, entityComponentStore);

                    entityCount -= allocatedCount;
                    chunkIndex++;
                }

                IncrementComponentTypeOrderVersion(archetype);
            }
        }

        public Chunk* GetCleanChunkNoMetaChunk(Archetype* archetype, SharedComponentValues sharedComponentValues)
        {
            var newChunk = AllocateChunk();
            ConstructChunk(archetype, newChunk, sharedComponentValues);

            return newChunk;
        }

        public Chunk* GetCleanChunk(Archetype* archetype, SharedComponentValues sharedComponentValues)
        {
            var newChunk = AllocateChunk();
            ConstructChunk(archetype, newChunk, sharedComponentValues);

            if (archetype->MetaChunkArchetype != null)
                CreateMetaEntityForChunk(newChunk);

            return newChunk;
        }


        public void InstantiateEntities(Entity srcEntity, Entity* outputEntities, int instanceCount)
        {
            if (HasComponent(srcEntity, m_LinkedGroupType))
            {
                var header = (BufferHeader*) GetComponentDataWithTypeRO(srcEntity, m_LinkedGroupType);
                var entityPtr = (Entity*) BufferHeader.GetElementPointer(header);
                var entityCount = header->Length;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (entityCount == 0 || entityPtr[0] != srcEntity)
                    throw new ArgumentException("LinkedEntityGroup[0] must always be the Entity itself.");
                for (int i = 0; i < entityCount; i++)
                {
                    if (!Exists(entityPtr[i]))
                        throw new ArgumentException(
                            "The srcEntity's LinkedEntityGroup references an entity that is invalid. (Entity at index {i} on the LinkedEntityGroup.)");

                    var archetype = GetArchetype(entityPtr[i]);
                    if (archetype->InstantiableArchetype == null)
                        throw new ArgumentException(
                            "The srcEntity's LinkedEntityGroup references an entity that has already been destroyed. (Entity at index {i} on the LinkedEntityGroup. Only system state components are left on the entity)");
                }
#endif
                InstantiateEntitiesGroup(entityPtr, entityCount, outputEntities, instanceCount);
            }
            else
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!Exists(srcEntity))
                    throw new ArgumentException("srcEntity is not a valid entity");

                var srcArchetype = GetArchetype(srcEntity);
                if (srcArchetype->InstantiableArchetype == null)
                    throw new ArgumentException(
                        "srcEntity is not instantiable because it has already been destroyed. (Only system state components are left on it)");
#endif
                InstantiateEntitiesOne(srcEntity, outputEntities, instanceCount, null, 0);
            }
        }

        public void ReserveManagedObjectArrays(NativeArray<int> indices)
        {
            int available = m_ManagedArrayFreeIndex.Size / UnsafeUtility.SizeOf<int>();

            if (available > indices.Length)
                available = indices.Length;

            m_ManagedArrayFreeIndex.Pop(indices.GetUnsafePtr(), available * UnsafeUtility.SizeOf<int>());

            int remainder = indices.Length - available;

            if (remainder > 0)
            {
                for(int i = available; i < indices.Length; ++i)
                    indices[i] = m_ManagedArrayIndex + i - available;

                m_ManagedArrayIndex += remainder;
                ManagedChangesTracker.ReserveManagedArrayStorage(remainder);
            }
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------

        Chunk* GetChunkWithEmptySlots(Archetype* archetype, SharedComponentValues sharedComponentValues)
        {
            var chunk = archetype->GetExistingChunkWithEmptySlots(sharedComponentValues);
            if (chunk == null)
            {
                chunk = GetCleanChunk(archetype, sharedComponentValues);
            }

            return chunk;
        }

        int AllocateIntoChunk(Chunk* chunk)
        {
            int outIndex;
            var res = AllocateIntoChunk(chunk, 1, out outIndex);
            Assert.AreEqual(1, res);
            return outIndex;
        }

        int AllocateIntoChunk(Chunk* chunk, int count, out int outIndex)
        {
            var allocatedCount = Math.Min(chunk->Capacity - chunk->Count, count);
            outIndex = chunk->Count;
            SetChunkCount(chunk, chunk->Count + allocatedCount);
            chunk->Archetype->EntityCount += allocatedCount;
            return allocatedCount;
        }

        void DeleteChunk(Chunk* chunk)
        {
            var entityCount = chunk->Count;
            DeallocateDataEntitiesInChunk((Entity*) chunk->Buffer, chunk, 0, chunk->Count);
            ManagedChangesTracker.IncrementComponentOrderVersion(chunk->Archetype,
                chunk->SharedComponentValues);
            IncrementComponentTypeOrderVersion(chunk->Archetype);
            chunk->Archetype->EntityCount -= entityCount;
            SetChunkCount(chunk, 0);
        }

        void DestroyMetaChunkEntity(Entity entity)
        {
            RemoveComponent(entity, m_ChunkHeaderComponentType);
            DestroyEntities(&entity, 1);
        }

        void CreateMetaEntityForChunk(Chunk* chunk)
        {
            fixed (EntityComponentStore* entityComponentStore = &this)
            {
                CreateEntities(chunk->Archetype->MetaChunkArchetype, &chunk->metaChunkEntity, 1);

                var chunkHeader = (ChunkHeader*) GetComponentDataWithTypeRW(chunk->metaChunkEntity, m_ChunkHeaderType, GlobalSystemVersion);
                
                chunkHeader->ArchetypeChunk = new ArchetypeChunk(chunk, entityComponentStore);
            }
        }

        struct InstantiateRemapChunk
        {
            public Chunk* Chunk;
            public int IndexInChunk;
            public int AllocatedCount;
            public int InstanceBeginIndex;
        }

        void AddToDestroyList(Chunk* chunk, int indexInChunk, int batchCount, int inputDestroyCount,
            ref UnsafeList entitiesList, ref int minBufferLength, ref int maxBufferLength)
        {
            int indexInArchetype = ChunkDataUtility.GetIndexInTypeArray(chunk->Archetype, m_LinkedGroupType);
            if (indexInArchetype != -1)
            {
                var baseHeader = ChunkDataUtility.GetComponentDataWithTypeRO(chunk, indexInChunk, m_LinkedGroupType);
                var stride = chunk->Archetype->SizeOfs[indexInArchetype];
                for (int i = 0; i != batchCount; i++)
                {
                    var header = (BufferHeader*) (baseHeader + stride * i);

                    var entityGroupCount = header->Length - 1;
                    if (entityGroupCount == 0)
                        continue;

                    var entityGroupArray = (Entity*) BufferHeader.GetElementPointer(header) + 1;

                    if (entitiesList.Capacity == 0)
                        entitiesList.SetCapacity<Entity>(inputDestroyCount * entityGroupCount /*, Allocator.TempJob*/);
                    entitiesList.AddRange<Entity>(entityGroupArray, entityGroupCount /*, Allocator.TempJob*/);

                    minBufferLength = math.min(minBufferLength, entityGroupCount);
                    maxBufferLength = math.max(maxBufferLength, entityGroupCount);
                }
            }
        }

        void DestroyBatch(Entity* entities, Chunk* chunk, int indexInChunk, int batchCount)
        {
            var archetype = chunk->Archetype;
            if (!archetype->SystemStateCleanupNeeded)
            {
                DeallocateDataEntitiesInChunk(entities, chunk, indexInChunk, batchCount);
                ManagedChangesTracker.IncrementComponentOrderVersion(archetype,
                    chunk->SharedComponentValues);
                IncrementComponentTypeOrderVersion(archetype);

                if (chunk->ManagedArrayIndex >= 0)
                {
                    // We can just chop-off the end, no need to copy anything
                    if (chunk->Count != indexInChunk + batchCount)
                        ManagedChangesTracker.CopyManagedObjects(chunk, chunk->Count - batchCount,
                            chunk,
                            indexInChunk, batchCount);

                    ManagedChangesTracker.ClearManagedObjects(chunk, chunk->Count - batchCount,
                        batchCount);
                }

                chunk->Archetype->EntityCount -= batchCount;
                SetChunkCount(chunk, chunk->Count - batchCount);
            }
            else
            {
                var newType = archetype->SystemStateResidueArchetype;

                var sharedComponentValues = chunk->SharedComponentValues;

                if (RequiresBuildingResidueSharedComponentIndices(archetype, newType))
                {
                    var tempAlloc = stackalloc int[newType->NumSharedComponents];
                    BuildResidueSharedComponentIndices(archetype, newType, sharedComponentValues, tempAlloc);
                    sharedComponentValues = tempAlloc;
                }

                // See: https://github.com/Unity-Technologies/dots/issues/1387
                // For Locked Order Chunks specfically, need to make sure that structural changes are always done per-chunk.
                // If trying to muutate structure in a way that is not per chunk, will hit an exception in the else clause anyway.
                // This ultimately needs to be replaced by entity batch interface.

                if (batchCount == chunk->Count)
                {
                    ManagedChangesTracker.IncrementComponentOrderVersion(archetype,
                        chunk->SharedComponentValues);
                    IncrementComponentTypeOrderVersion(archetype);

                    SetArchetype(chunk, newType, sharedComponentValues);
                }
                else
                {
                    for (var i = 0; i < batchCount; i++)
                    {
                        var entity = entities[i];
                        ManagedChangesTracker.IncrementComponentOrderVersion(archetype, GetChunk(entity)->SharedComponentValues);
                        IncrementComponentTypeOrderVersion(archetype);
                        SetArchetype(entity, newType, sharedComponentValues);
                    }
                }
            }
        }

        bool RequiresBuildingResidueSharedComponentIndices(Archetype* srcArchetype,
            Archetype* dstArchetype)
        {
            return dstArchetype->NumSharedComponents > 0 &&
                   dstArchetype->NumSharedComponents != srcArchetype->NumSharedComponents;
        }

        void BuildResidueSharedComponentIndices(Archetype* srcArchetype, Archetype* dstArchetype,
            SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            int oldFirstShared = srcArchetype->FirstSharedComponent;
            int newFirstShared = dstArchetype->FirstSharedComponent;
            int newCount = dstArchetype->NumSharedComponents;

            for (int oldIndex = 0, newIndex = 0; newIndex < newCount; ++newIndex, ++oldIndex)
            {
                var t = dstArchetype->Types[newIndex + newFirstShared];
                while (t != srcArchetype->Types[oldIndex + oldFirstShared])
                    ++oldIndex;
                dstSharedComponentValues[newIndex] = srcSharedComponentValues[oldIndex];
            }
        }

        void ReleaseChunk(Chunk* chunk)
        {
            // Remove references to shared components
            if (chunk->Archetype->NumSharedComponents > 0)
            {
                var sharedComponentValueArray = chunk->SharedComponentValues;

                for (var i = 0; i < chunk->Archetype->NumSharedComponents; ++i)
                    ManagedChangesTracker.RemoveReference(sharedComponentValueArray[i]);
            }

            if (chunk->ManagedArrayIndex != -1)
            {
                ManagedChangesTracker.DeallocateManagedArrayStorage(chunk->ManagedArrayIndex);
                m_ManagedArrayFreeIndex.Add(chunk->ManagedArrayIndex);
                chunk->ManagedArrayIndex = -1;
            }

            // this chunk is going away, so it shouldn't be in the empty slot list.
            if (chunk->Count < chunk->Capacity)
                chunk->Archetype->EmptySlotTrackingRemoveChunk(chunk);

            chunk->Archetype->RemoveFromChunkList(chunk);
            chunk->Archetype = null;

            FreeChunk(chunk);
        }

        int InstantiateEntitiesOne(Entity srcEntity, Entity* outputEntities,
            int instanceCount, InstantiateRemapChunk* remapChunks, int remapChunksCount)
        {
            var src = GetEntityInChunk(srcEntity);
            var srcArchetype = src.Chunk->Archetype;
            var dstArchetype = srcArchetype->InstantiableArchetype;

            var temp = stackalloc int[dstArchetype->NumSharedComponents];
            if (RequiresBuildingResidueSharedComponentIndices(srcArchetype, dstArchetype))
            {
                BuildResidueSharedComponentIndices(srcArchetype, dstArchetype,
                    src.Chunk->SharedComponentValues, temp);
            }
            else
            {
                // Always copy shared component indices since GetChunkWithEmptySlots might reallocate the storage of SharedComponentValues
                src.Chunk->SharedComponentValues.CopyTo(temp, 0, dstArchetype->NumSharedComponents);
            }

            SharedComponentValues sharedComponentValues = temp;

            Chunk* chunk = null;

            int instanceBeginIndex = 0;
            while (instanceBeginIndex != instanceCount)
            {
                chunk = GetChunkWithEmptySlots(dstArchetype, sharedComponentValues);

                int indexInChunk;
                var allocatedCount = AllocateIntoChunk(chunk, instanceCount - instanceBeginIndex, out indexInChunk);

                ChunkDataUtility.ReplicateComponents(src.Chunk, src.IndexInChunk, chunk, indexInChunk, allocatedCount);
                AllocateEntities(dstArchetype, chunk, indexInChunk, allocatedCount,
                    outputEntities + instanceBeginIndex);
                chunk->SetAllChangeVersions(GlobalSystemVersion);

#if UNITY_EDITOR
                for (var i = 0; i < allocatedCount; ++i)
                    SetName(outputEntities[i + instanceBeginIndex],
                        GetName(srcEntity));
#endif

                if (remapChunks != null)
                {
                    remapChunks[remapChunksCount].Chunk = chunk;
                    remapChunks[remapChunksCount].IndexInChunk = indexInChunk;
                    remapChunks[remapChunksCount].AllocatedCount = allocatedCount;
                    remapChunks[remapChunksCount].InstanceBeginIndex = instanceBeginIndex;
                    remapChunksCount++;
                }


                instanceBeginIndex += allocatedCount;
            }

            if (chunk != null)
            {
                ManagedChangesTracker.IncrementComponentOrderVersion(dstArchetype,
                    chunk->SharedComponentValues);
                IncrementComponentTypeOrderVersion(dstArchetype);
            }

            return remapChunksCount;
        }

        void InstantiateEntitiesGroup(Entity* srcEntities, int srcEntityCount,
            Entity* outputRootEntities, int instanceCount)
        {
            int totalCount = srcEntityCount * instanceCount;

            var tempAllocSize = sizeof(EntityRemapUtility.SparseEntityRemapInfo) * totalCount +
                                sizeof(InstantiateRemapChunk) * totalCount + sizeof(Entity) * instanceCount;
            byte* allocation;
            const int kMaxStackAllocSize = 16 * 1024;

            if (tempAllocSize > kMaxStackAllocSize)
            {
                allocation = (byte*) UnsafeUtility.Malloc(tempAllocSize, 16, Allocator.Temp);
            }
            else
            {
                var temp = stackalloc byte[tempAllocSize];
                allocation = temp;
            }

            var entityRemap = (EntityRemapUtility.SparseEntityRemapInfo*) allocation;
            var remapChunks = (InstantiateRemapChunk*) (entityRemap + totalCount);
            var outputEntities = (Entity*) (remapChunks + totalCount);

            var remapChunksCount = 0;

            for (int i = 0; i != srcEntityCount; i++)
            {
                var srcEntity = srcEntities[i];

                remapChunksCount = InstantiateEntitiesOne(srcEntity,
                    outputEntities, instanceCount, remapChunks, remapChunksCount);

                for (int r = 0; r != instanceCount; r++)
                {
                    var ptr = entityRemap + (r * srcEntityCount + i);
                    ptr->Src = srcEntity;
                    ptr->Target = outputEntities[r];
                }

                if (i == 0)
                {
                    for (int r = 0; r != instanceCount; r++)
                        outputRootEntities[r] = outputEntities[r];
                }
            }

            for (int i = 0; i != remapChunksCount; i++)
            {
                var chunk = remapChunks[i].Chunk;
                var dstArchetype = chunk->Archetype;
                var allocatedCount = remapChunks[i].AllocatedCount;
                var indexInChunk = remapChunks[i].IndexInChunk;
                var instanceBeginIndex = remapChunks[i].InstanceBeginIndex;

                var localRemap = entityRemap + instanceBeginIndex * srcEntityCount;

                EntityRemapUtility.PatchEntitiesForPrefab(dstArchetype->ScalarEntityPatches + 1,
                    dstArchetype->ScalarEntityPatchCount - 1, dstArchetype->BufferEntityPatches,
                    dstArchetype->BufferEntityPatchCount, chunk->Buffer, indexInChunk, allocatedCount, localRemap,
                    srcEntityCount);
            }

            if (tempAllocSize > kMaxStackAllocSize)
                UnsafeUtility.Free(allocation, Allocator.Temp);
        }

        void ConstructChunk(Archetype* archetype, Chunk* chunk, SharedComponentValues sharedComponentValues)
        {
            chunk->Archetype = archetype;
            chunk->Count = 0;
            chunk->Capacity = archetype->ChunkCapacity;
            chunk->SequenceNumber = AssignSequenceNumber(chunk);
            chunk->metaChunkEntity = Entity.Null;

            var numSharedComponents = archetype->NumSharedComponents;

            if (numSharedComponents > 0)
            {
                for (var i = 0; i < archetype->NumSharedComponents; ++i)
                {
                    var sharedComponentIndex = sharedComponentValues[i];
                    ManagedChangesTracker.AddReference(sharedComponentIndex);
                }
            }

            archetype->AddToChunkList(chunk, sharedComponentValues, GlobalSystemVersion);

            Assert.IsTrue(archetype->Chunks.Count != 0);

            // Chunk can't be locked at at construction time
            archetype->EmptySlotTrackingAddChunk(chunk);

            if (numSharedComponents == 0)
            {
                Assert.IsTrue(archetype->ChunksWithEmptySlots.Length != 0);
            }
            else
            {
                Assert.IsTrue(archetype->FreeChunksBySharedComponents.TryGet(chunk->SharedComponentValues,
                                  archetype->NumSharedComponents) != null);
            }

            if (archetype->NumManagedArrays > 0)
            {
                int managedArrayIndex;
                if (!m_ManagedArrayFreeIndex.IsEmpty)
                {
                    managedArrayIndex = m_ManagedArrayFreeIndex.Pop<int>();
                }
                else
                {
                    managedArrayIndex = m_ManagedArrayIndex;
                    m_ManagedArrayIndex++;
                }

                ManagedChangesTracker.AllocateManagedArrayStorage(managedArrayIndex,
                    archetype->NumManagedArrays * chunk->Capacity);
                chunk->ManagedArrayIndex = managedArrayIndex;
            }
            else
            {
                chunk->ManagedArrayIndex = -1;
            }

            chunk->Flags = 0;
        }

        void DeallocateDataEntitiesInChunk(Entity* entities, Chunk* chunk, int indexInChunk, int batchCount)
        {
            DeallocateBuffers(entities, chunk, batchCount);

            var freeIndex = m_NextFreeEntityIndex;

            for (var i = batchCount - 1; i >= 0; --i)
            {
                var entityIndex = entities[i].Index;

                m_EntityInChunkByEntity[entityIndex].Chunk = null;
                m_VersionByEntity[entityIndex]++;
                m_EntityInChunkByEntity[entityIndex].IndexInChunk = freeIndex;
#if UNITY_EDITOR
                m_NameByEntity[entityIndex] = new NumberedWords();
#endif
                freeIndex = entityIndex;
            }

            m_NextFreeEntityIndex = freeIndex;

            // Compute the number of things that need to moved and patched.
            int patchCount = Math.Min(batchCount, chunk->Count - indexInChunk - batchCount);

            if (0 == patchCount)
                return;

            // updates indexInChunk to point to where the components will be moved to
            //Assert.IsTrue(chunk->archetype->sizeOfs[0] == sizeof(Entity) && chunk->archetype->offsets[0] == 0);
            var movedEntities = (Entity*) chunk->Buffer + (chunk->Count - patchCount);
            for (var i = 0; i != patchCount; i++)
                m_EntityInChunkByEntity[movedEntities[i].Index].IndexInChunk = indexInChunk + i;

            // Move component data from the end to where we deleted components
            ChunkDataUtility.Copy(chunk, chunk->Count - patchCount, chunk, indexInChunk, patchCount);
        }
 
        void DeallocateBuffers(Entity* entities, Chunk* chunk, int batchCount)
        {
            var archetype = chunk->Archetype;

            for (var ti = 0; ti < archetype->TypesCount; ++ti)
            {
                var type = archetype->Types[ti];

                if (!type.IsBuffer)
                    continue;

                var basePtr = chunk->Buffer + archetype->Offsets[ti];
                var stride = archetype->SizeOfs[ti];

                for (int i = 0; i < batchCount; ++i)
                {
                    Entity e = entities[i];
                    int indexInChunk = m_EntityInChunkByEntity[e.Index].IndexInChunk;
                    byte* bufferPtr = basePtr + stride * indexInChunk;
                    BufferHeader.Destroy((BufferHeader*) bufferPtr);
                }
            }
        }

        EntityBatchInChunk GetFirstEntityBatchInChunk(Entity* entities, int count)
        {
            // This is optimized for the case where the array of entities are allocated contigously in the chunk
            // Thus the compacting of other elements can be batched

            // Calculate baseEntityIndex & chunk
            var baseEntityIndex = entities[0].Index;

            var versions = m_VersionByEntity;
            var chunkData = m_EntityInChunkByEntity;

            var chunk = versions[baseEntityIndex] == entities[0].Version
                ? m_EntityInChunkByEntity[baseEntityIndex].Chunk
                : null;
            var indexInChunk = chunkData[baseEntityIndex].IndexInChunk;
            var batchCount = 0;

            while (batchCount < count)
            {
                var entityIndex = entities[batchCount].Index;
                var curChunk = chunkData[entityIndex].Chunk;
                var curIndexInChunk = chunkData[entityIndex].IndexInChunk;

                if (versions[entityIndex] == entities[batchCount].Version)
                {
                    if (curChunk != chunk || curIndexInChunk != indexInChunk + batchCount)
                        break;
                }
                else
                {
                    if (chunk != null)
                        break;
                }

                batchCount++;
            }

            return new EntityBatchInChunk
            {
                Chunk = chunk,
                Count = batchCount,
                StartIndex = indexInChunk
            };
        }

    }
}