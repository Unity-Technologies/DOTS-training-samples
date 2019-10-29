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

        public bool AddComponent(Entity entity, ComponentType type)
        {
            var dstChunk = GetChunkWithEmptySlotsWithAddedComponent(entity, type);
            if (dstChunk == null)
                return false;

            Move(entity, dstChunk);
            return true;
        }

        public bool RemoveComponent(Entity entity, ComponentType type)
        {
            var dstChunk = GetChunkWithEmptySlotsWithRemovedComponent(entity, type);
            if (dstChunk == null)
                return false;

            Move(entity, dstChunk);
            return true;
        }

        bool AddComponent(EntityBatchInChunk entityBatchInChunk, ComponentType componentType, int sharedComponentIndex = 0)
        {
            var srcChunk = entityBatchInChunk.Chunk;
            var archetypeChunkFilter = GetArchetypeChunkFilterWithAddedComponent(srcChunk, componentType, sharedComponentIndex);
            if (archetypeChunkFilter.Archetype == null)
                return false;

            Move(entityBatchInChunk, ref archetypeChunkFilter);
            return true;
        }
        
        bool RemoveComponent(EntityBatchInChunk entityBatchInChunk, ComponentType componentType)
        {
            var srcChunk = entityBatchInChunk.Chunk;
            var archetypeChunkFilter = GetArchetypeChunkFilterWithRemovedComponent(srcChunk, componentType);
            if (archetypeChunkFilter.Archetype == null)
                return false;

            Move(entityBatchInChunk, ref archetypeChunkFilter);
            return true;
        }

        public void AddComponent(ArchetypeChunk* chunks, int chunkCount, ComponentType componentType, int sharedComponentIndex = 0)
        {
            Archetype* prevArchetype = null;
            Archetype* dstArchetype = null;
            int indexInTypeArray = 0;
            
            for (int i = 0; i < chunkCount; i++)
            {
                var chunk = chunks[i].m_Chunk;
                var srcArchetype = chunk->Archetype;
                if (prevArchetype != chunk->Archetype)
                {
                    dstArchetype = GetArchetypeWithAddedComponent(srcArchetype, componentType, &indexInTypeArray);
                    prevArchetype = chunk->Archetype;
                }
                
                if (dstArchetype == null)
                    continue;
                
                var archetypeChunkFilter = GetArchetypeChunkFilterWithAddedComponent(chunk, dstArchetype, indexInTypeArray, componentType, sharedComponentIndex);

                Move(chunk, ref archetypeChunkFilter);
            }
        }

        public void AddComponent(NativeArray<ArchetypeChunk> chunkArray, ComponentType componentType, int sharedComponentIndex = 0)
        {
            AddComponent((ArchetypeChunk*)NativeArrayUnsafeUtility.GetUnsafePtr(chunkArray), chunkArray.Length, componentType, sharedComponentIndex);
        }
        
        public void RemoveComponent(ArchetypeChunk* chunks, int chunkCount, ComponentType componentType)
        {
            Archetype* prevArchetype = null;
            Archetype* dstArchetype = null;
            int indexInTypeArray = 0;
            
            for (int i = 0; i < chunkCount; i++)
            {
                var chunk = chunks[i].m_Chunk;
                var srcArchetype = chunk->Archetype;
                
                if (prevArchetype != chunk->Archetype)
                {
                    dstArchetype = GetArchetypeWithRemovedComponent(srcArchetype, componentType, &indexInTypeArray);
                    prevArchetype = chunk->Archetype;
                }
                
                if (dstArchetype == srcArchetype)
                    continue;
                
                var archetypeChunkFilter = GetArchetypeChunkFilterWithRemovedComponent(chunk, dstArchetype, indexInTypeArray, componentType);

                Move(chunk, ref archetypeChunkFilter);
            }
        }
        
        public void AddComponent(UnsafeList* sortedEntityBatchList, ComponentType type, int existingSharedComponentIndex)
        {
            Assert.IsFalse(type.IsChunkComponent);

            // Reverse order so that batch indices do not change while iterating.
            for (int i = sortedEntityBatchList->Length - 1; i >= 0; i--)
                AddComponent(((EntityBatchInChunk*)sortedEntityBatchList->Ptr)[i], type, existingSharedComponentIndex);
        }

        public void RemoveComponent(UnsafeList* sortedEntityBatchList, ComponentType type)
        {
            Assert.IsFalse(type.IsChunkComponent);

            // Reverse order so that batch indices do not change while iterating.
            for (int i = sortedEntityBatchList->Length - 1; i >= 0; i--)
                RemoveComponent(((EntityBatchInChunk*)sortedEntityBatchList->Ptr)[i], type);
        }

        public void AddComponents(Entity entity, ComponentTypes types)
        {
            var archetypeChunkFilter = GetArchetypeChunkFilterWithAddedComponents(GetChunk(entity), types);
            if (archetypeChunkFilter.Archetype == null)
                return;

            Move(entity, ref archetypeChunkFilter);
        }

        public void SetSharedComponentDataIndex(Entity entity, ComponentType componentType, int dstSharedComponentDataIndex)
        {
            var archetypeChunkFilter = GetArchetypeChunkFilterWithChangedSharedComponent(GetChunk(entity), componentType, dstSharedComponentDataIndex);
            if (archetypeChunkFilter.Archetype == null)
                return;

            Move(entity, ref archetypeChunkFilter);
        }

        // Note previously called SetArchetype: SetArchetype is used internally to refer to the function which only creates the cross-reference between the
        // entity id and the archetype (m_ArchetypeByEntity). This is not "Setting" the archetype, it is moving the components to a different archetype.
        public void Move(Entity entity, Archetype* dstArchetype)
        {
            var archetypeChunkFilter = GetArchetypeChunkFilterWithChangedArchetype(GetChunk(entity), dstArchetype);
            if (archetypeChunkFilter.Archetype == null)
                return;

            Move(entity, ref archetypeChunkFilter);
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------

        void Move(Entity entity, Chunk* dstChunk)
        {
            var srcEntityInChunk = GetEntityInChunk(entity);
            var srcChunk = srcEntityInChunk.Chunk;
            var srcChunkIndex = srcEntityInChunk.IndexInChunk;
            var entityBatch = new EntityBatchInChunk { Chunk = srcChunk, Count = 1, StartIndex = srcChunkIndex };

            Move(entityBatch, dstChunk);
        }

        void Move(Entity entity, ref ArchetypeChunkFilter archetypeChunkFilter)
        {
            var srcEntityInChunk = GetEntityInChunk(entity);
            var entityBatch = new EntityBatchInChunk { Chunk = srcEntityInChunk.Chunk, Count = 1, StartIndex = srcEntityInChunk.IndexInChunk };

            Move(entityBatch, ref archetypeChunkFilter);
        }

        void Move(Chunk* srcChunk, ref ArchetypeChunkFilter archetypeChunkFilter)
        {
            if (archetypeChunkFilter.Archetype->SystemStateCleanupComplete)
            {
                DeleteChunk(srcChunk);
                return;
            }

            var srcArchetype = srcChunk->Archetype;
            if (ChunkDataUtility.AreLayoutCompatible(srcArchetype, archetypeChunkFilter.Archetype))
            {
                ChangeArchetypeInPlace(srcChunk, ref archetypeChunkFilter);
                return;
            }

            var entityBatch = new EntityBatchInChunk { Chunk = srcChunk, Count = srcChunk->Count, StartIndex = 0 };
            Move(entityBatch, ref archetypeChunkFilter);
        }

        void Move(EntityBatchInChunk entityBatchInChunk, ref ArchetypeChunkFilter archetypeChunkFilter)
        {
            var systemStateCleanupComplete = archetypeChunkFilter.Archetype->SystemStateCleanupComplete;

            var srcChunk = entityBatchInChunk.Chunk;
            var srcRemainingCount = entityBatchInChunk.Count;
            var startIndex = entityBatchInChunk.StartIndex;

            if ((srcRemainingCount == srcChunk->Count) && systemStateCleanupComplete)
            {
                DeleteChunk(srcChunk);
                return;
            }

            while (srcRemainingCount > 0)
            {
                var dstChunk = GetChunkWithEmptySlots(ref archetypeChunkFilter);
                var dstCount = Move(new EntityBatchInChunk { Chunk = srcChunk, Count = srcRemainingCount, StartIndex = startIndex }, dstChunk);
                srcRemainingCount -= dstCount;
            }
        }

        // ----------------------------------------------------------------------------------------------------------
        // Core, self-contained functions to change chunks. No other functions should actually move data from
        // one Chunk to another, or otherwise change the structure of a Chunk after creation.
        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Move subset of chunk data into another chunk.
        /// - Chunks can be of same archetype (but differ by shared component values)
        /// - Returns number moved. Caller handles if less than indicated in srcBatch.
        /// </summary>
        /// <returns></returns>
        int Move(EntityBatchInChunk srcBatch, Chunk* dstChunk)
        {
            var srcChunk = srcBatch.Chunk;
            var srcChunkIndex = srcBatch.StartIndex;
            var srcCount = srcBatch.Count;
            var srcArchetype = srcChunk->Archetype;
            var dstArchetype = dstChunk->Archetype;

            // Note (srcArchetype == dstArchetype) is valid
            // Archetypes can the the same, but chunks still differ because filter is different (e.g. shared component)

            int dstChunkIndex;
            var dstCount = AllocateIntoChunk(dstChunk, srcCount, out dstChunkIndex);

            // If can only move partial batch, move from the end so that remainder of batch isn't affected.
            srcChunkIndex = srcChunkIndex + srcCount - dstCount;

            ChunkDataUtility.Convert(srcChunk, srcChunkIndex, dstChunk, dstChunkIndex, dstCount);

            if (dstChunk->ManagedArrayIndex >= 0 && srcChunk->ManagedArrayIndex >= 0)
                ManagedChangesTracker.CopyManagedObjects(srcChunk, srcChunkIndex, dstChunk, dstChunkIndex, dstCount);

            var dstEntities = (Entity*)ChunkDataUtility.GetComponentDataRO(dstChunk, dstChunkIndex, 0);
            for (int i = 0; i < dstCount; i++)
            {
                var entity = dstEntities[i];

                SetArchetype(entity, dstArchetype);
                SetEntityInChunk(entity, new EntityInChunk { Chunk = dstChunk, IndexInChunk = dstChunkIndex + i });
            }

            // Fill in moved component data from the end.
            var srcTailIndex = srcChunkIndex + dstCount;
            var srcTailCount = srcChunk->Count - srcTailIndex;
            var fillCount = math.min(dstCount, srcTailCount);
            if (fillCount > 0)
            {
                var fillStartIndex = srcChunk->Count - fillCount;

                ChunkDataUtility.Copy(srcChunk, fillStartIndex, srcChunk, srcChunkIndex, fillCount);

                var fillEntities = (Entity*)ChunkDataUtility.GetComponentDataRO(srcChunk, srcChunkIndex, 0);
                for (int i = 0; i < fillCount; i++)
                {
                    var entity = fillEntities[i];
                    SetEntityInChunk(entity, new EntityInChunk { Chunk = srcChunk, IndexInChunk = srcChunkIndex + i });
                }

                if (srcChunk->ManagedArrayIndex >= 0)
                    ManagedChangesTracker.CopyManagedObjects(srcChunk, fillStartIndex, srcChunk, srcChunkIndex, fillCount);
            }

            if (srcChunk->ManagedArrayIndex >= 0)
                ManagedChangesTracker.ClearManagedObjects(srcChunk, srcChunk->Count - dstCount, dstCount);

            srcArchetype->EntityCount -= dstCount;

            dstChunk->SetAllChangeVersions(GlobalSystemVersion);
            srcChunk->SetAllChangeVersions(GlobalSystemVersion);

            ManagedChangesTracker.IncrementComponentOrderVersion(srcArchetype, srcChunk->SharedComponentValues);
            IncrementComponentTypeOrderVersion(srcArchetype);

            ManagedChangesTracker.IncrementComponentOrderVersion(dstArchetype, dstChunk->SharedComponentValues);
            IncrementComponentTypeOrderVersion(dstArchetype);

            SetChunkCount(srcChunk, srcChunk->Count - dstCount);

            // Cannot DestroyEntities unless SystemStateCleanupComplete on the entity chunk. 
            if (dstChunk->Archetype->SystemStateCleanupComplete)
                DestroyEntities(dstEntities, dstCount);

            return dstCount;
        }

        /// <summary>
        /// Fix-up the chunk to refer to a different (but layout compatible) archetype.
        /// - Should only be called by Move(chunk)
        /// </summary>
        void ChangeArchetypeInPlace(Chunk* srcChunk, ref ArchetypeChunkFilter dstArchetypeChunkFilter)
        {
            var dstArchetype = dstArchetypeChunkFilter.Archetype;
            fixed (int* sharedComponentValues = dstArchetypeChunkFilter.SharedComponentValues)
            {
                var srcArchetype = srcChunk->Archetype;
                ChunkDataUtility.AssertAreLayoutCompatible(srcArchetype, dstArchetype);

                var fixupSharedComponentReferences = (srcArchetype->NumSharedComponents > 0) || (dstArchetype->NumSharedComponents > 0);
                if (fixupSharedComponentReferences)
                {
                    int srcFirstShared = srcArchetype->FirstSharedComponent;
                    int dstFirstShared = dstArchetype->FirstSharedComponent;
                    int srcCount = srcArchetype->NumSharedComponents;
                    int dstCount = dstArchetype->NumSharedComponents;

                    int o = 0;
                    int n = 0;

                    for (; n < dstCount && o < srcCount;)
                    {
                        int srcType = srcArchetype->Types[o + srcFirstShared].TypeIndex;
                        int dstType = dstArchetype->Types[n + dstFirstShared].TypeIndex;
                        if (srcType == dstType)
                        {
                            var srcSharedComponentDataIndex = srcChunk->SharedComponentValues[o];
                            var dstSharedComponentDataIndex = sharedComponentValues[n];
                            if (srcSharedComponentDataIndex != dstSharedComponentDataIndex)
                            {
                                ManagedChangesTracker.RemoveReference(srcSharedComponentDataIndex);
                                ManagedChangesTracker.AddReference(dstSharedComponentDataIndex);
                            }

                            n++;
                            o++;
                        }
                        else if (dstType > srcType) // removed from dstArchetype
                        {
                            var sharedComponentDataIndex = srcChunk->SharedComponentValues[o];
                            ManagedChangesTracker.RemoveReference(sharedComponentDataIndex);
                            o++;
                        }
                        else // added to dstArchetype
                        {
                            var sharedComponentDataIndex = sharedComponentValues[n];
                            ManagedChangesTracker.AddReference(sharedComponentDataIndex);
                            n++;
                        }
                    }

                    for (; n < dstCount; n++) // added to dstArchetype
                    {
                        var sharedComponentDataIndex = sharedComponentValues[n];
                        ManagedChangesTracker.AddReference(sharedComponentDataIndex);
                    }

                    for (; o < srcCount; o++) // removed from dstArchetype
                    {
                        var sharedComponentDataIndex = srcChunk->SharedComponentValues[o];
                        ManagedChangesTracker.RemoveReference(sharedComponentDataIndex);
                    }
                }

                var fixupManagedComponents = (srcArchetype->NumManagedArrays > 0) || (dstArchetype->NumManagedArrays > 0);
                if (fixupManagedComponents)
                {
                    if (dstArchetype->NumManagedArrays == 0) // removed all
                    {
                        ManagedChangesTracker.DeallocateManagedArrayStorage(srcChunk->ManagedArrayIndex);
                        m_ManagedArrayFreeIndex.Add(srcChunk->ManagedArrayIndex);
                        srcChunk->ManagedArrayIndex = -1;
                    }
                    else
                    {
                        // We have changed the managed array order + size so allocate a new managed array
                        // copy the unchanged values into it
                        int dstManagedArrayIndex;
                        if (!m_ManagedArrayFreeIndex.IsEmpty)
                        {
                            dstManagedArrayIndex = m_ManagedArrayFreeIndex.Pop<int>();
                        }
                        else
                        {
                            dstManagedArrayIndex = m_ManagedArrayIndex;
                            m_ManagedArrayIndex++;
                        }

                        ManagedChangesTracker.AllocateManagedArrayStorage(dstManagedArrayIndex, dstArchetype->NumManagedArrays * srcChunk->Capacity);

                        int srcManagedArrayIndex = srcChunk->ManagedArrayIndex;
                        srcChunk->ManagedArrayIndex = dstManagedArrayIndex;

                        if (srcManagedArrayIndex != -1)
                        {
                            ManagedChangesTracker.MoveChunksManagedObjects(srcArchetype, srcManagedArrayIndex, dstArchetype, dstManagedArrayIndex, srcChunk->Capacity, srcChunk->Count);

                            ManagedChangesTracker.DeallocateManagedArrayStorage(srcManagedArrayIndex);
                            m_ManagedArrayFreeIndex.Add(srcManagedArrayIndex);
                        }
                    }
                }

                var count = srcChunk->Count;
                bool hasEmptySlots = count < srcChunk->Capacity;

                if (hasEmptySlots)
                    srcArchetype->EmptySlotTrackingRemoveChunk(srcChunk);

                int chunkIndexInSrcArchetype = srcChunk->ListIndex;

                var dstTypes = dstArchetype->Types;
                var srcTypes = srcArchetype->Types;

                //Change version is overriden below
                dstArchetype->AddToChunkList(srcChunk, sharedComponentValues, 0);
                int chunkIndexInDstArchetype = srcChunk->ListIndex;

                //Copy change versions from src to dst archetype
                for (int isrcType = srcArchetype->TypesCount - 1, idstType = dstArchetype->TypesCount - 1;
                    idstType >= 0;
                    --idstType)
                {
                    var dstType = dstTypes[idstType];
                    while (srcTypes[isrcType] > dstType)
                        --isrcType;
                    var version = srcTypes[isrcType] == dstType
                        ? srcArchetype->Chunks.GetChangeVersion(isrcType, chunkIndexInSrcArchetype)
                        : GlobalSystemVersion;
                    dstArchetype->Chunks.SetChangeVersion(idstType, chunkIndexInDstArchetype, version);
                }

                srcChunk->ListIndex = chunkIndexInSrcArchetype;
                srcArchetype->RemoveFromChunkList(srcChunk);
                srcChunk->ListIndex = chunkIndexInDstArchetype;

                if (hasEmptySlots)
                    dstArchetype->EmptySlotTrackingAddChunk(srcChunk);

                SetArchetype(srcChunk, dstArchetype);

                srcArchetype->EntityCount -= count;
                dstArchetype->EntityCount += count;

                if (srcArchetype->MetaChunkArchetype != dstArchetype->MetaChunkArchetype)
                {
                    if (srcArchetype->MetaChunkArchetype == null)
                    {
                        CreateMetaEntityForChunk(srcChunk);
                    }
                    else if (dstArchetype->MetaChunkArchetype == null)
                    {
                        DestroyMetaChunkEntity(srcChunk->metaChunkEntity);
                        srcChunk->metaChunkEntity = Entity.Null;
                    }
                    else
                    {
                        var metaChunk = GetChunk(srcChunk->metaChunkEntity);
                        var archetypeChunkFilter = new ArchetypeChunkFilter(dstArchetype->MetaChunkArchetype, metaChunk->SharedComponentValues);
                        Move(srcChunk->metaChunkEntity, ref archetypeChunkFilter);
                    }
                }
            }
        }
    }
}