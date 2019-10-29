using System;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Profiling;

namespace Unity.Entities
{
    internal unsafe partial struct EntityComponentStore
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------

        public void AddComponent(Entity entity, ComponentType type)
        {
            var archetype = GetArchetype(entity);
            int indexInTypeArray = 0;
            var newType = GetArchetypeWithAddedComponentType(archetype, type, &indexInTypeArray);
            if (newType == null)
            {
                // This can happen if we are adding a tag component to an entity that already has it.
                return;
            }

            var sharedComponentValues = GetChunk(entity)->SharedComponentValues;
            if (type.IsSharedComponent)
            {
                int* temp = stackalloc int[newType->NumSharedComponents];
                int indexOfNewSharedComponent = indexInTypeArray - newType->FirstSharedComponent;
                BuildSharedComponentIndicesWithAddedComponent(indexOfNewSharedComponent, 0,
                    newType->NumSharedComponents, sharedComponentValues, temp);
                sharedComponentValues = temp;
            }

            SetArchetype(entity, newType, sharedComponentValues);
        }

        public void AddSharedComponent(NativeArray<ArchetypeChunk> chunkArray, ComponentType componentType,
            int sharedComponentIndex)
        {
            Assert.IsTrue(componentType.IsSharedComponent);

            // Create batch lists here with default(T) to avoid allocating any memory.
            // The following loop will go over chunks and possibly add something to these lists.
            // It's expected that the common case will not require touching these lists so
            // we want to avoid allocating them unless absolutely necessary.
            bool batchListsInitialized = false;
            var sourceBlittableEntityBatchList = default(NativeList<EntityBatchInChunk>);
            var destinationBlittableEntityBatchList = default(NativeList<EntityBatchInChunk>);
            var sourceManagedEntityBatchList = default(NativeList<EntityBatchInChunk>);
            var destinationManagedEntityBatchList = default(NativeList<EntityBatchInChunk>);
            var sourceCountEntityBatchList = default(NativeList<EntityBatchInChunk>);

            for (int i = 0; i < chunkArray.Length; i++)
            {
                var chunk = chunkArray[i];
                var archetype = chunk.Archetype.Archetype;
                int indexInTypeArray = 0;
                var newType = GetArchetypeWithAddedComponentType(archetype, componentType, &indexInTypeArray);
                if (newType == null)
                {
                    // This can happen if we are adding a tag component to an entity that already has it.
                    Assert.AreEqual(0, sharedComponentIndex);
                    continue;
                }

                var sharedComponentValues = chunk.m_Chunk->SharedComponentValues;

                int* temp = stackalloc int[newType->NumSharedComponents];
                int indexOfNewSharedComponent = indexInTypeArray - newType->FirstSharedComponent;
                BuildSharedComponentIndicesWithAddedComponent(indexOfNewSharedComponent, sharedComponentIndex,
                    newType->NumSharedComponents, sharedComponentValues, temp);
                sharedComponentValues = temp;

                if (ChunkDataUtility.AreLayoutCompatible(archetype, newType))
                {
                    MoveChunkToNewArchetype(chunk.m_Chunk, newType, sharedComponentValues);
                    ManagedChangesTracker.AddReference(sharedComponentIndex);
                }
                else
                {
                    // New archetype chunks are not layout compatible so we must copy over the
                    // entities manually before exiting this function.  Build the batch lists that
                    // will copy over the data from src to dst chunks.
                    if (!batchListsInitialized)
                    {
                        batchListsInitialized = true;
                        sourceBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent);
                        destinationBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent);
                        sourceManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent);
                        destinationManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent);
                        sourceCountEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent);
                    }

                    var srcRemainingCount = chunk.Count;
                    var srcChunk = chunk.m_Chunk;
                    var srcOffset = 0;
                    var srcStartIndex = 0;
                    var srcChunkManagedData = srcChunk->ManagedArrayIndex >= 0;
                    sourceCountEntityBatchList.Add(new EntityBatchInChunk { Chunk = srcChunk, Count = srcRemainingCount, StartIndex = 0 });

                    while (srcRemainingCount > 0)
                    {
                        var dstChunk = GetChunkWithEmptySlots(newType, sharedComponentValues);
                        int dstIndexBase;
                        var dstCount = AllocateIntoChunk(dstChunk, srcRemainingCount, out dstIndexBase);

                        var partialSrcEntityBatch = new EntityBatchInChunk
                        {
                            Chunk = srcChunk,
                            Count = dstCount,
                            StartIndex = srcStartIndex + srcOffset
                        };
                        var partialDstEntityBatch = new EntityBatchInChunk
                        {
                            Chunk = dstChunk,
                            Count = dstCount,
                            StartIndex = dstIndexBase
                        };

                        sourceBlittableEntityBatchList.Add(partialSrcEntityBatch);
                        destinationBlittableEntityBatchList.Add(partialDstEntityBatch);

                        if (srcChunkManagedData)
                        {
                            sourceManagedEntityBatchList.Add(partialSrcEntityBatch);
                            destinationManagedEntityBatchList.Add(partialDstEntityBatch);
                        }

                        srcOffset += dstCount;
                        srcRemainingCount -= dstCount;
                    }
                }
            }

            if (batchListsInitialized)
            {
                // Copy the data from old archetype chunk to new archetype chunk.
                var copyBlittableChunkDataJobHandle = CopyBlittableChunkDataJob(sourceBlittableEntityBatchList,
                    destinationBlittableEntityBatchList);
                copyBlittableChunkDataJobHandle.Complete();
                CopyManagedChunkData(sourceManagedEntityBatchList, destinationManagedEntityBatchList);

                UpdateDestinationVersions(destinationBlittableEntityBatchList);
                UpdateSourceCountsAndVersions(sourceCountEntityBatchList);

                sourceBlittableEntityBatchList.Dispose();
                destinationBlittableEntityBatchList.Dispose();
                sourceManagedEntityBatchList.Dispose();
                destinationManagedEntityBatchList.Dispose();
                sourceCountEntityBatchList.Dispose();
            }
        }

        public void AddComponentFromMainThread(NativeList<EntityBatchInChunk> entityBatchList,
            ComponentType type,
            int existingSharedComponentIndex)
        {
            using (var sourceBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var destinationBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var sourceManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var destinationManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var packBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var packManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var sourceCountEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var moveChunkList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            {
                AllocateChunksForAddComponent(entityBatchList, type, existingSharedComponentIndex,
                    sourceCountEntityBatchList, packBlittableEntityBatchList, packManagedEntityBatchList,
                    sourceBlittableEntityBatchList, destinationBlittableEntityBatchList, sourceManagedEntityBatchList,
                    destinationManagedEntityBatchList, moveChunkList);

                var copyBlittableChunkDataJobHandle = CopyBlittableChunkDataJob(sourceBlittableEntityBatchList,
                    destinationBlittableEntityBatchList);
                var packBlittableChunkDataJobHandle =
                    PackBlittableChunkDataJob(packBlittableEntityBatchList,
                        copyBlittableChunkDataJobHandle);
                packBlittableChunkDataJobHandle.Complete();

                CopyManagedChunkData(sourceManagedEntityBatchList, destinationManagedEntityBatchList);
                PackManagedChunkData(packManagedEntityBatchList);

                UpdateDestinationVersions(destinationBlittableEntityBatchList);
                UpdateSourceCountsAndVersions(sourceCountEntityBatchList);
                MoveChunksForAddComponent(moveChunkList, type, existingSharedComponentIndex);
            }
        }
        
        public void RemoveComponentFromMainThread(NativeList<EntityBatchInChunk> entityBatchList,
            ComponentType type,
            int existingSharedComponentIndex)
        {
            using (var sourceBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var destinationBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var sourceManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var destinationManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var packBlittableEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var packManagedEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var sourceCountEntityBatchList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            using (var moveChunkList = new NativeList<EntityBatchInChunk>(Allocator.Persistent))
            {
                AllocateChunksForRemoveComponent(entityBatchList, type, existingSharedComponentIndex,
                    sourceCountEntityBatchList, packBlittableEntityBatchList, packManagedEntityBatchList,
                    sourceBlittableEntityBatchList, destinationBlittableEntityBatchList, sourceManagedEntityBatchList,
                    destinationManagedEntityBatchList, moveChunkList);

                var copyBlittableChunkDataJobHandle = CopyBlittableChunkDataJob(sourceBlittableEntityBatchList,
                    destinationBlittableEntityBatchList);
                var packBlittableChunkDataJobHandle =
                    PackBlittableChunkDataJob(packBlittableEntityBatchList,
                        copyBlittableChunkDataJobHandle);
                packBlittableChunkDataJobHandle.Complete();

                CopyManagedChunkData(sourceManagedEntityBatchList, destinationManagedEntityBatchList);
                PackManagedChunkData(packManagedEntityBatchList);

                UpdateDestinationVersions(destinationBlittableEntityBatchList);
                UpdateSourceCountsAndVersions(sourceCountEntityBatchList);
                MoveChunksForRemoveComponent(moveChunkList, type, existingSharedComponentIndex);
            }
        }

        public void AddComponents(Entity entity, ComponentTypes types)
        {
            var oldArchetype = GetArchetype(entity);
            var oldTypes = oldArchetype->Types;

            var newTypesCount = oldArchetype->TypesCount + types.Length;
            ComponentTypeInArchetype* newTypes = stackalloc ComponentTypeInArchetype[newTypesCount];

            var indexOfNewTypeInNewArchetype = stackalloc int[types.Length];

            // zipper the two sorted arrays "type" and "componentTypeInArchetype" into "componentTypeInArchetype"
            // because this is done in-place, it must be done backwards so as not to disturb the existing contents.

            var unusedIndices = 0;
            {
                var oldThings = oldArchetype->TypesCount;
                var newThings = types.Length;
                var mixedThings = oldThings + newThings;
                while (oldThings > 0 && newThings > 0) // while both are still zippering,
                {
                    var oldThing = oldTypes[oldThings - 1];
                    var newThing = types.GetComponentType(newThings - 1);
                    if (oldThing.TypeIndex > newThing.TypeIndex) // put whichever is bigger at the end of the array
                    {
                        newTypes[--mixedThings] = oldThing;
                        --oldThings;
                    }
                    else
                    {
                        if (oldThing.TypeIndex == newThing.TypeIndex && newThing.IgnoreDuplicateAdd)
                            --oldThings;

                        var componentTypeInArchetype = new ComponentTypeInArchetype(newThing);
                        newTypes[--mixedThings] = componentTypeInArchetype;
                        --newThings;
                        indexOfNewTypeInNewArchetype[newThings] = mixedThings; // "this new thing ended up HERE"
                    }
                }

                Assert.AreEqual(0, newThings); // must not be any new things to copy remaining, oldThings contain entity

                while (oldThings > 0) // if there are remaining old things, copy them here
                {
                    newTypes[--mixedThings] = oldTypes[--oldThings];
                }

                unusedIndices = mixedThings; // In case we ignored duplicated types, this will be > 0
            }

            var newArchetype =
                GetOrCreateArchetype(newTypes + unusedIndices, newTypesCount);

            var sharedComponentValues = GetChunk(entity)->SharedComponentValues;
            if (types.m_masks.m_SharedComponentMask != 0)
            {
                int* alloc2 = stackalloc int[newArchetype->NumSharedComponents];
                var oldSharedComponentValues = sharedComponentValues;
                sharedComponentValues = alloc2;
                BuildSharedComponentIndicesWithAddedComponents(oldArchetype, newArchetype,
                    oldSharedComponentValues, alloc2);
            }

            SetArchetype(entity, newArchetype, sharedComponentValues);
        }

        public void RemoveComponent(NativeArray<ArchetypeChunk> chunkArray, ComponentType type)
        {
            var chunks = (ArchetypeChunk*) chunkArray.GetUnsafeReadOnlyPtr();
            if (type.IsZeroSized)
            {
                Archetype* prevOldArchetype = null;
                Archetype* newArchetype = null;
                int indexInOldTypeArray = 0;
                for (int i = 0; i < chunkArray.Length; ++i)
                {
                    var chunk = chunks[i].m_Chunk;
                    var oldArchetype = chunk->Archetype;
                    if (oldArchetype != prevOldArchetype)
                    {
                        if (ChunkDataUtility.GetIndexInTypeArray(oldArchetype, type.TypeIndex) != -1)
                            newArchetype = GetArchetypeWithRemovedComponentType(
                                oldArchetype, type,
                                &indexInOldTypeArray);
                        else
                            newArchetype = null;
                        prevOldArchetype = oldArchetype;
                    }

                    if (newArchetype == null)
                        continue;

                    if (newArchetype->SystemStateCleanupComplete)
                    {
                        DeleteChunk(chunk);
                        continue;
                    }

                    var sharedComponentValues = chunk->SharedComponentValues;
                    if (type.IsSharedComponent)
                    {
                        int* temp = stackalloc int[newArchetype->NumSharedComponents];
                        int indexOfRemovedSharedComponent = indexInOldTypeArray - oldArchetype->FirstSharedComponent;
                        var sharedComponentDataIndex = chunk->GetSharedComponentValue(indexOfRemovedSharedComponent);
                        ManagedChangesTracker.RemoveReference(sharedComponentDataIndex);
                        BuildSharedComponentIndicesWithRemovedComponent(indexOfRemovedSharedComponent,
                            newArchetype->NumSharedComponents, sharedComponentValues, temp);
                        sharedComponentValues = temp;
                    }

                    MoveChunkToNewArchetype(chunk, newArchetype, sharedComponentValues);
                }
            }
            else
            {
                Archetype* prevOldArchetype = null;
                Archetype* newArchetype = null;
                for (int i = 0; i < chunkArray.Length; ++i)
                {
                    var chunk = chunks[i].m_Chunk;
                    var oldArchetype = chunk->Archetype;
                    if (oldArchetype != prevOldArchetype)
                    {
                        if (ChunkDataUtility.GetIndexInTypeArray(oldArchetype, type.TypeIndex) != -1)
                            newArchetype =
                                GetArchetypeWithRemovedComponentType(oldArchetype,
                                    type);
                        else
                            newArchetype = null;
                        prevOldArchetype = oldArchetype;
                    }

                    if (newArchetype != null)
                        if (newArchetype->SystemStateCleanupComplete)
                        {
                            DeleteChunk(chunk);
                        }
                        else
                        {
                            SetArchetype(chunk, newArchetype, chunk->SharedComponentValues);
                        }
                }
            }
        }

        public void RemoveComponent(Entity entity, ComponentType type)
        {
            if (!HasComponent(entity, type))
                return;

            var archetype = GetArchetype(entity);
            var chunk = GetChunk(entity);

            if (chunk->Locked || chunk->LockedEntityOrder)
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                throw new InvalidOperationException(
                    "Cannot remove components in locked Chunks. Unlock Chunk first.");
#else
                return;
#endif
            }

            int indexInOldTypeArray = -1;
            var newType =
                GetArchetypeWithRemovedComponentType(archetype, type, &indexInOldTypeArray);

            var sharedComponentValues = chunk->SharedComponentValues;

            if (type.IsSharedComponent)
            {
                int* temp = stackalloc int[newType->NumSharedComponents];
                int indexOfRemovedSharedComponent = indexInOldTypeArray - archetype->FirstSharedComponent;
                BuildSharedComponentIndicesWithRemovedComponent(indexOfRemovedSharedComponent,
                    newType->NumSharedComponents, sharedComponentValues, temp);
                sharedComponentValues = temp;
            }

            SetArchetype(entity, newType, sharedComponentValues);

            // Cleanup residue component
            if (newType->SystemStateCleanupComplete)
                DestroyEntities(&entity, 1);
        }

        public void SetSharedComponentDataIndex(Entity entity, int typeIndex, int newSharedComponentDataIndex)
        {
            var archetype = GetArchetype(entity);
            var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(archetype, typeIndex);
            var srcChunk = GetChunk(entity);
            var srcSharedComponentValueArray = srcChunk->SharedComponentValues;
            var sharedComponentOffset = indexInTypeArray - archetype->FirstSharedComponent;
            var oldSharedComponentDataIndex = srcSharedComponentValueArray[sharedComponentOffset];

            if (newSharedComponentDataIndex == oldSharedComponentDataIndex)
                return;

            var sharedComponentIndices = stackalloc int[archetype->NumSharedComponents];

            srcSharedComponentValueArray.CopyTo(sharedComponentIndices, 0, archetype->NumSharedComponents);

            sharedComponentIndices[sharedComponentOffset] = newSharedComponentDataIndex;

            var newChunk = GetChunkWithEmptySlots(archetype, sharedComponentIndices);
            var newChunkIndex = AllocateIntoChunk(newChunk);

            ManagedChangesTracker.IncrementComponentOrderVersion(archetype, srcChunk->SharedComponentValues);
            IncrementComponentTypeOrderVersion(archetype);

            MoveEntityToChunk(entity, newChunk, newChunkIndex);
        }
        

        public void SetArchetype(Entity entity, EntityArchetype archetype)
        {
            var oldArchetype = GetArchetype(entity);
            var newArchetype = archetype.Archetype;

            var sharedComponentValues = GetChunk(entity)->SharedComponentValues;
            int* temp = stackalloc int[archetype.Archetype->NumSharedComponents];
            Unity.Entities.EntityComponentStore.BuildSharedComponentIndicesWithChangedArchetype(oldArchetype, newArchetype, sharedComponentValues, temp);
            sharedComponentValues = temp;

            SetArchetype(entity, newArchetype, sharedComponentValues);
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------

        void SetArchetype(Entity entity, Archetype* archetype, SharedComponentValues sharedComponentValues)
        {
            var oldArchetype = GetArchetype(entity);
            var oldEntityInChunk = GetEntityInChunk(entity);
            var oldChunk = oldEntityInChunk.Chunk;
            var oldChunkIndex = oldEntityInChunk.IndexInChunk;
            if (oldArchetype == archetype)
                return;

            var chunk = GetChunkWithEmptySlots(archetype, sharedComponentValues);
            var chunkIndex = AllocateIntoChunk(chunk);

            ChunkDataUtility.Convert(oldChunk, oldChunkIndex, chunk, chunkIndex);
            if (chunk->ManagedArrayIndex >= 0 && oldChunk->ManagedArrayIndex >= 0)
                ManagedChangesTracker.CopyManagedObjects(oldChunk, oldChunkIndex, chunk,
                    chunkIndex, 1);

            SetArchetype(entity, archetype);
            SetEntityInChunk(entity, new EntityInChunk {Chunk = chunk, IndexInChunk = chunkIndex});

            var lastIndex = oldChunk->Count - 1;
            // No need to replace with ourselves
            if (lastIndex != oldChunkIndex)
            {
                var lastEntity = *(Entity*) ChunkDataUtility.GetComponentDataRO(oldChunk, lastIndex, 0);
                var lastEntityInChunk = new EntityInChunk
                {
                    Chunk = oldChunk,
                    IndexInChunk = oldChunkIndex
                };
                SetEntityInChunk(lastEntity, lastEntityInChunk);

                ChunkDataUtility.Copy(oldChunk, lastIndex, oldChunk, oldChunkIndex, 1);
                if (oldChunk->ManagedArrayIndex >= 0)
                    ManagedChangesTracker.CopyManagedObjects(oldChunk, lastIndex, oldChunk,
                        oldChunkIndex,
                        1);
            }

            if (oldChunk->ManagedArrayIndex >= 0)
                ManagedChangesTracker.ClearManagedObjects(oldChunk, lastIndex, 1);

            --oldArchetype->EntityCount;

            chunk->SetAllChangeVersions(GlobalSystemVersion);
            oldChunk->SetAllChangeVersions(GlobalSystemVersion);

            ManagedChangesTracker.IncrementComponentOrderVersion(oldArchetype, oldChunk->SharedComponentValues);
            IncrementComponentTypeOrderVersion(oldArchetype);

            SetChunkCount(oldChunk, lastIndex);

            // #todo @macton Validate whether or not this is necessary. But tests would need to be fixed up if not.
            IncrementComponentTypeOrderVersion(archetype);
            
            ManagedChangesTracker.IncrementComponentOrderVersion(archetype, chunk->SharedComponentValues);
        }

        void SetArchetype(Chunk* chunk, Archetype* archetype, SharedComponentValues sharedComponentValues)
        {
            var srcChunk = chunk;
            var srcArchetype = srcChunk->Archetype;
            if (srcArchetype == archetype)
                return;

            var srcEntities = (Entity*) srcChunk->Buffer;
            var srcEntitiesCount = srcChunk->Count;
            var srcRemainingCount = srcEntitiesCount;
            var srcOffset = 0;

            var dstArchetype = archetype;

            while (srcRemainingCount > 0)
            {
                var dstChunk = GetChunkWithEmptySlots(archetype, sharedComponentValues);
                int dstIndexBase;
                var dstCount = AllocateIntoChunk(dstChunk, srcRemainingCount, out dstIndexBase);
                ChunkDataUtility.Convert(srcChunk, srcOffset, dstChunk, dstIndexBase, dstCount);

                dstChunk->SetAllChangeVersions(GlobalSystemVersion);

                ManagedChangesTracker.IncrementComponentOrderVersion(archetype,
                    dstChunk->SharedComponentValues);
                IncrementComponentTypeOrderVersion(archetype);

                for (int i = 0; i < dstCount; i++)
                {
                    var entity = srcEntities[srcOffset + i];

                    SetArchetype(entity, dstArchetype);
                    SetEntityInChunk(entity,
                        new EntityInChunk {Chunk = dstChunk, IndexInChunk = dstIndexBase + i});
                }

                if (srcChunk->ManagedArrayIndex >= 0 && dstChunk->ManagedArrayIndex >= 0)
                    ManagedChangesTracker.CopyManagedObjects(srcChunk, srcOffset, dstChunk,
                        dstIndexBase,
                        dstCount);

                srcRemainingCount -= dstCount;
                srcOffset += dstCount;
            }

            srcArchetype->EntityCount -= srcEntitiesCount;

            if (srcChunk->ManagedArrayIndex >= 0)
                ManagedChangesTracker.ClearManagedObjects(srcChunk, 0, srcEntitiesCount);
            SetChunkCount(srcChunk, 0);
        }

        static void BuildSharedComponentIndicesWithAddedComponent(int indexOfNewSharedComponent, int value,
            int newCount, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            srcSharedComponentValues.CopyTo(dstSharedComponentValues, 0, indexOfNewSharedComponent);
            dstSharedComponentValues[indexOfNewSharedComponent] = value;
            srcSharedComponentValues.CopyTo(dstSharedComponentValues + indexOfNewSharedComponent + 1,
                indexOfNewSharedComponent, newCount - indexOfNewSharedComponent - 1);
        }

        static void BuildSharedComponentIndicesWithRemovedComponent(int indexOfRemovedSharedComponent,
            int newCount, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            srcSharedComponentValues.CopyTo(dstSharedComponentValues, 0, indexOfRemovedSharedComponent);
            srcSharedComponentValues.CopyTo(dstSharedComponentValues + indexOfRemovedSharedComponent,
                indexOfRemovedSharedComponent + 1, newCount - indexOfRemovedSharedComponent);
        }

        static void BuildSharedComponentIndicesWithAddedComponents(Archetype* srcArchetype,
            Archetype* dstArchetype, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            int oldFirstShared = srcArchetype->FirstSharedComponent;
            int newFirstShared = dstArchetype->FirstSharedComponent;
            int oldCount = srcArchetype->NumSharedComponents;
            int newCount = dstArchetype->NumSharedComponents;

            for (int oldIndex = oldCount - 1, newIndex = newCount - 1; newIndex >= 0; --newIndex)
            {
                // oldIndex might become -1 which is ok since oldFirstShared is always at least 1. The comparison will then always be false
                if (dstArchetype->Types[newIndex + newFirstShared] == srcArchetype->Types[oldIndex + oldFirstShared])
                    dstSharedComponentValues[newIndex] = srcSharedComponentValues[oldIndex--];
                else
                    dstSharedComponentValues[newIndex] = 0;
            }
        }

        static void BuildSharedComponentIndicesWithChangedArchetype(Archetype* srcArchetype,
            Archetype* dstArchetype, SharedComponentValues srcSharedComponentValues, int* dstSharedComponentValues)
        {
            int oldFirstShared = srcArchetype->FirstSharedComponent;
            int newFirstShared = dstArchetype->FirstSharedComponent;
            int oldCount = srcArchetype->NumSharedComponents;
            int newCount = dstArchetype->NumSharedComponents;

            int o = 0;
            int n = 0;
            
            for (; n < newCount && o < oldCount;)
            {
                int srcType = srcArchetype->Types[o + oldFirstShared].TypeIndex;
                int dstType = dstArchetype->Types[n + newFirstShared].TypeIndex;
                if (srcType == dstType)
                    dstSharedComponentValues[n++] = srcSharedComponentValues[o++];
                else if (dstType > srcType)
                    o++;
                else
                    dstSharedComponentValues[n++] = 0;
            }
            
            for (;n < newCount;n++)
                dstSharedComponentValues[n] = 0;
        }
        
        void AllocateChunksForAddComponent(NativeList<EntityBatchInChunk> entityBatchList, ComponentType type,
            int existingSharedComponentIndex,
            NativeList<EntityBatchInChunk> sourceCountEntityBatchList,
            NativeList<EntityBatchInChunk> packBlittableEntityBatchList,
            NativeList<EntityBatchInChunk> packManagedEntityBatchList,
            NativeList<EntityBatchInChunk> sourceBlittableEntityBatchList,
            NativeList<EntityBatchInChunk> destinationBlittableEntityBatchList,
            NativeList<EntityBatchInChunk> sourceManagedEntityBatchList,
            NativeList<EntityBatchInChunk> destinationManagedEntityBatchList,
            NativeList<EntityBatchInChunk> moveChunkList)
        {
            Profiler.BeginSample("Allocate Chunks");

            Archetype* prevSrcArchetype = null;
            Archetype* dstArchetype = null;
            int indexInTypeArray = 0;
            var layoutCompatible = false;

            for (int i = 0; i < entityBatchList.Length; i++)
            {
                var srcEntityBatch = entityBatchList[i];
                var srcRemainingCount = srcEntityBatch.Count;
                var srcChunk = srcEntityBatch.Chunk;
                if (srcChunk == null)
                    continue;
                var srcArchetype = srcChunk->Archetype;
                var srcStartIndex = srcEntityBatch.StartIndex;
                var srcTail = (srcStartIndex + srcRemainingCount) == srcChunk->Count;
                var srcChunkManagedData = srcChunk->ManagedArrayIndex >= 0;

                if (prevSrcArchetype != srcArchetype)
                {
                    dstArchetype = GetArchetypeWithAddedComponentType(srcArchetype,
                        type, &indexInTypeArray);
                    layoutCompatible = ChunkDataUtility.AreLayoutCompatible(srcArchetype, dstArchetype);
                    prevSrcArchetype = srcArchetype;
                }

                if (dstArchetype == null)
                    continue;

                var srcWholeChunk = srcEntityBatch.Count == srcChunk->Count;
                if (srcWholeChunk && layoutCompatible)
                {
                    moveChunkList.Add(srcEntityBatch);
                    continue;
                }

                var sharedComponentValues = srcChunk->SharedComponentValues;
                if (type.IsSharedComponent)
                {
                    int* temp = stackalloc int[dstArchetype->NumSharedComponents];
                    int indexOfNewSharedComponent = indexInTypeArray - dstArchetype->FirstSharedComponent;
                    BuildSharedComponentIndicesWithAddedComponent(indexOfNewSharedComponent,
                        existingSharedComponentIndex,
                        dstArchetype->NumSharedComponents, sharedComponentValues, temp);

                    sharedComponentValues = temp;
                }

                sourceCountEntityBatchList.Add(srcEntityBatch);
                if (!srcTail)
                {
                    packBlittableEntityBatchList.Add(srcEntityBatch);
                    if (srcChunkManagedData)
                    {
                        packManagedEntityBatchList.Add(srcEntityBatch);
                    }
                }

                var srcOffset = 0;
                while (srcRemainingCount > 0)
                {
                    var dstChunk = GetChunkWithEmptySlots(dstArchetype, sharedComponentValues);
                    int dstIndexBase;
                    var dstCount = AllocateIntoChunk(dstChunk, srcRemainingCount, out dstIndexBase);

                    var partialSrcEntityBatch = new EntityBatchInChunk
                    {
                        Chunk = srcChunk,
                        Count = dstCount,
                        StartIndex = srcStartIndex + srcOffset
                    };
                    var partialDstEntityBatch = new EntityBatchInChunk
                    {
                        Chunk = dstChunk,
                        Count = dstCount,
                        StartIndex = dstIndexBase
                    };

                    sourceBlittableEntityBatchList.Add(partialSrcEntityBatch);
                    destinationBlittableEntityBatchList.Add(partialDstEntityBatch);

                    if (srcChunkManagedData)
                    {
                        sourceManagedEntityBatchList.Add(partialSrcEntityBatch);
                        destinationManagedEntityBatchList.Add(partialDstEntityBatch);
                    }

                    srcOffset += dstCount;
                    srcRemainingCount -= dstCount;
                }
            }

            Profiler.EndSample();
        }
        
        void AllocateChunksForRemoveComponent(NativeList<EntityBatchInChunk> entityBatchList, ComponentType type,
            int existingSharedComponentIndex,
            NativeList<EntityBatchInChunk> sourceCountEntityBatchList,
            NativeList<EntityBatchInChunk> packBlittableEntityBatchList,
            NativeList<EntityBatchInChunk> packManagedEntityBatchList,
            NativeList<EntityBatchInChunk> sourceBlittableEntityBatchList,
            NativeList<EntityBatchInChunk> destinationBlittableEntityBatchList,
            NativeList<EntityBatchInChunk> sourceManagedEntityBatchList,
            NativeList<EntityBatchInChunk> destinationManagedEntityBatchList,
            NativeList<EntityBatchInChunk> moveChunkList)
        {
            Profiler.BeginSample("Allocate Chunks");

            Archetype* prevSrcArchetype = null;
            Archetype* dstArchetype = null;
            int indexInTypeArray = 0;
            var layoutCompatible = false;

            for (int i = 0; i < entityBatchList.Length; i++)
            {
                var srcEntityBatch = entityBatchList[i];
                var srcRemainingCount = srcEntityBatch.Count;
                var srcChunk = srcEntityBatch.Chunk;
                if (srcChunk == null)
                    continue;
                var srcArchetype = srcChunk->Archetype;
                var srcStartIndex = srcEntityBatch.StartIndex;
                var srcTail = (srcStartIndex + srcRemainingCount) == srcChunk->Count;
                var srcChunkManagedData = srcChunk->ManagedArrayIndex >= 0;

                if (prevSrcArchetype != srcArchetype)
                {
                    dstArchetype = GetArchetypeWithRemovedComponentType(srcArchetype,
                        type, &indexInTypeArray);
                    layoutCompatible = ChunkDataUtility.AreLayoutCompatible(srcArchetype, dstArchetype);
                    prevSrcArchetype = srcArchetype;
                }

                if (dstArchetype == null)
                    continue;

                if (dstArchetype == srcArchetype)
                    continue;
                
                if (dstArchetype->SystemStateCleanupComplete)
                    continue;

                var srcWholeChunk = srcEntityBatch.Count == srcChunk->Count;
                if (srcWholeChunk && layoutCompatible)
                {
                    moveChunkList.Add(srcEntityBatch);
                    continue;
                }

                var sharedComponentValues = srcChunk->SharedComponentValues;
                if (type.IsSharedComponent)
                {
                    int* temp = stackalloc int[dstArchetype->NumSharedComponents];
                    BuildSharedComponentIndicesWithRemovedComponent(existingSharedComponentIndex,
                        dstArchetype->NumSharedComponents, sharedComponentValues, temp);

                    sharedComponentValues = temp;
                }

                sourceCountEntityBatchList.Add(srcEntityBatch);
                if (!srcTail)
                {
                    packBlittableEntityBatchList.Add(srcEntityBatch);
                    if (srcChunkManagedData)
                    {
                        packManagedEntityBatchList.Add(srcEntityBatch);
                    }
                }

                var srcOffset = 0;
                while (srcRemainingCount > 0)
                {
                    var dstChunk = GetChunkWithEmptySlots(dstArchetype, sharedComponentValues);
                    int dstIndexBase;
                    var dstCount = AllocateIntoChunk(dstChunk, srcRemainingCount, out dstIndexBase);

                    var partialSrcEntityBatch = new EntityBatchInChunk
                    {
                        Chunk = srcChunk,
                        Count = dstCount,
                        StartIndex = srcStartIndex + srcOffset
                    };
                    var partialDstEntityBatch = new EntityBatchInChunk
                    {
                        Chunk = dstChunk,
                        Count = dstCount,
                        StartIndex = dstIndexBase
                    };

                    sourceBlittableEntityBatchList.Add(partialSrcEntityBatch);
                    destinationBlittableEntityBatchList.Add(partialDstEntityBatch);

                    if (srcChunkManagedData)
                    {
                        sourceManagedEntityBatchList.Add(partialSrcEntityBatch);
                        destinationManagedEntityBatchList.Add(partialDstEntityBatch);
                    }

                    srcOffset += dstCount;
                    srcRemainingCount -= dstCount;
                }
            }

            Profiler.EndSample();
        }

        [BurstCompile]
        struct CopyBlittableChunkData : IJobParallelFor
        {
            [ReadOnly] public NativeList<EntityBatchInChunk> DestinationEntityBatchList;
            [ReadOnly] public NativeList<EntityBatchInChunk> SourceEntityBatchList;
            [NativeDisableUnsafePtrRestriction] public EntityComponentStore* entityComponentStore;

            public void Execute(int i)
            {
                var srcEntityBatch = SourceEntityBatchList[i];
                var dstEntityBatch = DestinationEntityBatchList[i];

                var srcChunk = srcEntityBatch.Chunk;
                var srcOffset = srcEntityBatch.StartIndex;
                var dstChunk = dstEntityBatch.Chunk;
                var dstIndexBase = dstEntityBatch.StartIndex;
                var dstCount = dstEntityBatch.Count;
                var srcEntities = (Entity*) srcChunk->Buffer;
                var dstEntities = (Entity*) dstChunk->Buffer;
                var dstArchetype = dstChunk->Archetype;

                ChunkDataUtility.Convert(srcChunk, srcOffset, dstChunk, dstIndexBase, dstCount);

                for (int entityIndex = 0; entityIndex < dstCount; entityIndex++)
                {
                    var entity = dstEntities[dstIndexBase + entityIndex];
                    srcEntities[srcOffset + entityIndex] = Entity.Null;

                    entityComponentStore->SetArchetype(entity, dstArchetype);
                    entityComponentStore->SetEntityInChunk(entity,
                        new EntityInChunk {Chunk = dstChunk, IndexInChunk = dstIndexBase + entityIndex});
                }
            }
        }

        JobHandle CopyBlittableChunkDataJob(NativeList<EntityBatchInChunk> sourceEntityBatchList,
            NativeList<EntityBatchInChunk> destinationEntityBatchList,
            JobHandle inputDeps = new JobHandle())
        {
            fixed (EntityComponentStore* entityComponentStore = &this)
            {
                Profiler.BeginSample("Copy Blittable Chunk Data");
                var copyBlittableChunkDataJob = new CopyBlittableChunkData
                {
                    DestinationEntityBatchList = destinationEntityBatchList,
                    SourceEntityBatchList = sourceEntityBatchList,
                    entityComponentStore = entityComponentStore
                };
                var copyBlittableChunkDataJobHandle =
                    copyBlittableChunkDataJob.Schedule(sourceEntityBatchList.Length, 64, inputDeps);
                Profiler.EndSample();
                return copyBlittableChunkDataJobHandle;
            }
        }

        [BurstCompile]
        struct PackBlittableChunkData : IJob
        {
            [ReadOnly] public NativeList<EntityBatchInChunk> PackBlittableEntityBatchList;
            [NativeDisableUnsafePtrRestriction] public EntityComponentStore* entityComponentStore;

            public void Execute()
            {
                // Packing is done in reverse (sorted) so that order is preserved of to-be packed batches in same chunk
                for (int i = PackBlittableEntityBatchList.Length - 1; i >= 0; i--)
                {
                    var srcEntityBatch = PackBlittableEntityBatchList[i];
                    var srcChunk = srcEntityBatch.Chunk;
                    var dstIndexBase = srcEntityBatch.StartIndex;
                    var dstCount = srcEntityBatch.Count;
                    var srcOffset = dstIndexBase + dstCount;
                    var srcCount = srcChunk->Count - srcOffset;
                    var srcEntities = (Entity*) srcChunk->Buffer;

                    ChunkDataUtility.Copy(srcChunk, srcOffset, srcChunk, dstIndexBase, srcCount);

                    // Update EntityInChunk in reverse order to ensure that the moved entity is used for computing
                    // IndexInChunk instead of the stale entity that was left behind before the packing was done.
                    //
                    // Before packing where A and B are the live entities that need to be packed:
                    //
                    // 0 1 2 3 4 5 6 7
                    // x x x A x x x B
                    //
                    // After packing B:
                    //
                    // 0 1 2 3 4 5 6 7
                    // x x x A B x x B
                    //
                    // After packing AB (plus all elements to the end of chunk):
                    //
                    // 0 1 2 3 4 5 6 7
                    // A B x x B x x B
                    //
                    // By going back to front, we ensure that A's IndexInChunk is 0 and B's IndexInChunk is 1, whereas
                    // front to back would mean A's IndexInChunk is 0 and B's IndexInChunk is 7, since there's still
                    // a stale copy of B at the end of the chunk.
                    for (int entityIndex = srcCount - 1; entityIndex >= 0; entityIndex--)
                    {
                        var entity = srcEntities[dstIndexBase + entityIndex];
                        if (entity == Entity.Null)
                            continue;

                        entityComponentStore->SetEntityInChunk(entity,
                            new EntityInChunk {Chunk = srcChunk, IndexInChunk = dstIndexBase + entityIndex});
                    }
                }
            }
        }

        JobHandle PackBlittableChunkDataJob(NativeList<EntityBatchInChunk> packBlittableEntityBatchList,
            JobHandle inputDeps = new JobHandle())
        {
            fixed (EntityComponentStore* entityComponentStore = &this)
            {
                var packBlittableChunkDataJob = new PackBlittableChunkData
                {
                    PackBlittableEntityBatchList = packBlittableEntityBatchList,
                    entityComponentStore = entityComponentStore
                };
                var packBlittableChunkDataJobHandle = packBlittableChunkDataJob.Schedule(inputDeps);
                return packBlittableChunkDataJobHandle;
            }
        }

        void CopyManagedChunkData(NativeList<EntityBatchInChunk> sourceEntityBatchList,
            NativeList<EntityBatchInChunk> destinationEntityBatchList)
        {
            Profiler.BeginSample("Copy Managed Chunk Data");
            for (int i = 0; i < sourceEntityBatchList.Length; i++)
            {
                var srcEntityBatch = sourceEntityBatchList[i];
                var dstEntityBatch = destinationEntityBatchList[i];

                var srcChunk = srcEntityBatch.Chunk;
                var srcOffset = srcEntityBatch.StartIndex;
                var dstChunk = dstEntityBatch.Chunk;
                var dstIndexBase = dstEntityBatch.StartIndex;
                var dstCount = dstEntityBatch.Count;

                if (srcChunk->ManagedArrayIndex >= 0 && dstChunk->ManagedArrayIndex >= 0)
                    ManagedChangesTracker.CopyManagedObjects(srcChunk, srcOffset, dstChunk, dstIndexBase, dstCount);

                if (srcChunk->ManagedArrayIndex >= 0)
                    ManagedChangesTracker.ClearManagedObjects(srcChunk, srcOffset, dstCount);
            }

            Profiler.EndSample();
        }

        void PackManagedChunkData(NativeList<EntityBatchInChunk> packManagedEntityBatchList)
        {
            Profiler.BeginSample("Pack Managed Chunk Data");
            // Packing is done in reverse (sorted) so that order is preserved of to-be packed batches in same chunk
            for (int i = packManagedEntityBatchList.Length - 1; i >= 0; i--)
            {
                var srcEntityBatch = packManagedEntityBatchList[i];
                var srcChunk = srcEntityBatch.Chunk;
                var dstIndexBase = srcEntityBatch.StartIndex;
                var dstCount = srcEntityBatch.Count;
                var srcOffset = dstIndexBase + dstCount;
                var srcCount = srcChunk->Count - srcOffset;

                ManagedChangesTracker.CopyManagedObjects(srcChunk, srcOffset, srcChunk, dstIndexBase, srcCount);
            }

            Profiler.EndSample();
        }

        void UpdateDestinationVersions(NativeList<EntityBatchInChunk> destinationBlittableEntityBatchList)
        {
            Profiler.BeginSample("Update Destination Versions");
            for (int i = 0; i < destinationBlittableEntityBatchList.Length; i++)
            {
                var dstEntityBatch = destinationBlittableEntityBatchList[i];
                var dstChunk = dstEntityBatch.Chunk;
                var dstSharedComponentValues = dstChunk->SharedComponentValues;
                var dstArchetype = dstChunk->Archetype;

                dstChunk->SetAllChangeVersions(GlobalSystemVersion);
                ManagedChangesTracker.IncrementComponentOrderVersion(dstArchetype, dstSharedComponentValues);
                IncrementComponentTypeOrderVersion(dstArchetype);
            }

            Profiler.EndSample();
        }

        void UpdateSourceCountsAndVersions(NativeList<EntityBatchInChunk> sourceCountEntityBatchList)
        {
            Profiler.BeginSample("Update Source Counts and Versions");
            for (int i = 0; i < sourceCountEntityBatchList.Length; i++)
            {
                var srcEntityBatch = sourceCountEntityBatchList[i];
                var srcChunk = srcEntityBatch.Chunk;
                var srcCount = srcEntityBatch.Count;
                var srcArchetype = srcChunk->Archetype;
                var srcSharedComponentValues = srcChunk->SharedComponentValues;

                srcArchetype->EntityCount -= srcCount;

                srcChunk->SetAllChangeVersions(GlobalSystemVersion);
                SetChunkCount(srcChunk, srcChunk->Count - srcCount);
                ManagedChangesTracker.IncrementComponentOrderVersion(srcArchetype, srcSharedComponentValues);
                IncrementComponentTypeOrderVersion(srcArchetype);
            }

            Profiler.EndSample();
        }

        void MoveChunksForAddComponent(NativeList<EntityBatchInChunk> entityBatchList, ComponentType type,
            int existingSharedComponentIndex)
        {
            Archetype* prevSrcArchetype = null;
            Archetype* dstArchetype = null;
            int indexInTypeArray = 0;

            for (int i = 0; i < entityBatchList.Length; i++)
            {
                var srcEntityBatch = entityBatchList[i];
                var srcChunk = srcEntityBatch.Chunk;
                var srcArchetype = srcChunk->Archetype;
                if (srcArchetype != prevSrcArchetype)
                {
                    dstArchetype =
                        GetArchetypeWithAddedComponentType(srcArchetype, type,
                            &indexInTypeArray);
                    prevSrcArchetype = srcArchetype;
                }

                var sharedComponentValues = srcChunk->SharedComponentValues;
                if (type.IsSharedComponent)
                {
                    int* temp = stackalloc int[dstArchetype->NumSharedComponents];
                    int indexOfNewSharedComponent = indexInTypeArray - dstArchetype->FirstSharedComponent;
                    BuildSharedComponentIndicesWithAddedComponent(indexOfNewSharedComponent,
                        existingSharedComponentIndex,
                        dstArchetype->NumSharedComponents, sharedComponentValues, temp);
                    sharedComponentValues = temp;
                }

                MoveChunkToNewArchetype(srcChunk, dstArchetype, sharedComponentValues);
            }

            ManagedChangesTracker.AddReference(existingSharedComponentIndex,
                entityBatchList.Length);
        }
        
        void MoveChunksForRemoveComponent(NativeList<EntityBatchInChunk> entityBatchList, ComponentType type,
            int existingSharedComponentIndex)
        {
            Archetype* prevSrcArchetype = null;
            Archetype* dstArchetype = null;
            int indexInTypeArray = 0;

            for (int i = 0; i < entityBatchList.Length; i++)
            {
                var srcEntityBatch = entityBatchList[i];
                var srcChunk = srcEntityBatch.Chunk;
                var srcArchetype = srcChunk->Archetype;
                if (srcArchetype != prevSrcArchetype)
                {
                    dstArchetype =
                        GetArchetypeWithRemovedComponentType(srcArchetype, type,
                            &indexInTypeArray);
                    prevSrcArchetype = srcArchetype;
                }

                var sharedComponentValues = srcChunk->SharedComponentValues;
                if (type.IsSharedComponent)
                {
                    int* temp = stackalloc int[dstArchetype->NumSharedComponents];
                    BuildSharedComponentIndicesWithRemovedComponent( existingSharedComponentIndex,
                        dstArchetype->NumSharedComponents, sharedComponentValues, temp);
                    sharedComponentValues = temp;
                }
                
                // Cleanup residue component
                if (dstArchetype->SystemStateCleanupComplete)
                    DestroyBatch((Entity*) srcChunk->Buffer, srcChunk, srcEntityBatch.StartIndex, srcEntityBatch.Count);
                else
                    MoveChunkToNewArchetype(srcChunk, dstArchetype, sharedComponentValues);
            }

            ManagedChangesTracker.AddReference(existingSharedComponentIndex,
                entityBatchList.Length);
        }

        void MoveChunkToNewArchetype(Chunk* chunk, Archetype* newArchetype,
            SharedComponentValues sharedComponentValues)
        {
            var oldArchetype = chunk->Archetype;
            ChunkDataUtility.AssertAreLayoutCompatible(oldArchetype, newArchetype);
            var count = chunk->Count;
            bool hasEmptySlots = count < chunk->Capacity;

            if (hasEmptySlots)
                oldArchetype->EmptySlotTrackingRemoveChunk(chunk);

            int chunkIndexInOldArchetype = chunk->ListIndex;

            var newTypes = newArchetype->Types;
            var oldTypes = oldArchetype->Types;

            chunk->Archetype = newArchetype;

            //Change version is overriden below
            newArchetype->AddToChunkList(chunk, sharedComponentValues, 0);
            int chunkIndexInNewArchetype = chunk->ListIndex;

            //Copy change versions from old to new archetype
            for (int iOldType = oldArchetype->TypesCount - 1, iNewType = newArchetype->TypesCount - 1;
                iNewType >= 0;
                --iNewType)
            {
                var newType = newTypes[iNewType];
                while (oldTypes[iOldType] > newType)
                    --iOldType;
                var version = oldTypes[iOldType] == newType
                    ? oldArchetype->Chunks.GetChangeVersion(iOldType, chunkIndexInOldArchetype)
                    : GlobalSystemVersion;
                newArchetype->Chunks.SetChangeVersion(iNewType, chunkIndexInNewArchetype, version);
            }

            chunk->ListIndex = chunkIndexInOldArchetype;
            oldArchetype->RemoveFromChunkList(chunk);
            chunk->ListIndex = chunkIndexInNewArchetype;

            if (hasEmptySlots)
                newArchetype->EmptySlotTrackingAddChunk(chunk);

            SetArchetype(chunk, newArchetype);

            oldArchetype->EntityCount -= count;
            newArchetype->EntityCount += count;

            if (oldArchetype->MetaChunkArchetype != newArchetype->MetaChunkArchetype)
            {
                if (oldArchetype->MetaChunkArchetype == null)
                {
                    CreateMetaEntityForChunk(chunk);
                }
                else if (newArchetype->MetaChunkArchetype == null)
                {
                    DestroyMetaChunkEntity(chunk->metaChunkEntity);
                    chunk->metaChunkEntity = Entity.Null;
                }
                else
                {
                    var metaChunk = GetChunk(chunk->metaChunkEntity);
                    var sharedComponentDataIndices = metaChunk->SharedComponentValues;
                    SetArchetype(chunk->metaChunkEntity, newArchetype->MetaChunkArchetype, sharedComponentDataIndices);
                }
            }
        }

        void MoveEntityToChunk(Entity entity, Chunk* newChunk, int newChunkIndex)
        {
            var oldEntityInChunk = GetEntityInChunk(entity);
            var oldChunk = oldEntityInChunk.Chunk;
            var oldChunkIndex = oldEntityInChunk.IndexInChunk;

            Assert.IsTrue(oldChunk->Archetype == newChunk->Archetype);

            ChunkDataUtility.Copy(oldChunk, oldChunkIndex, newChunk, newChunkIndex, 1);

            if (oldChunk->ManagedArrayIndex >= 0)
                ManagedChangesTracker.CopyManagedObjects(oldChunk, oldChunkIndex, newChunk,
                    newChunkIndex, 1);

            SetEntityInChunk(entity, new EntityInChunk
            {
                Chunk = newChunk,
                IndexInChunk = newChunkIndex
            });

            var lastIndex = oldChunk->Count - 1;
            // No need to replace with ourselves
            if (lastIndex != oldChunkIndex)
            {
                var lastEntity = *(Entity*) ChunkDataUtility.GetComponentDataRO(oldChunk, lastIndex, 0);
                SetEntityInChunk(lastEntity, new EntityInChunk
                {
                    Chunk = oldChunk,
                    IndexInChunk = oldChunkIndex
                });
                ChunkDataUtility.Copy(oldChunk, lastIndex, oldChunk, oldChunkIndex, 1);
                if (oldChunk->ManagedArrayIndex >= 0)
                    ManagedChangesTracker.CopyManagedObjects(oldChunk, lastIndex, oldChunk,
                        oldChunkIndex, 1);
            }

            if (oldChunk->ManagedArrayIndex >= 0)
                ManagedChangesTracker.ClearManagedObjects(oldChunk, lastIndex, 1);

            newChunk->SetAllChangeVersions(GlobalSystemVersion);
            oldChunk->SetAllChangeVersions(GlobalSystemVersion);

            newChunk->Archetype->EntityCount--;
            SetChunkCount(oldChunk, oldChunk->Count - 1);
        }
    }
}