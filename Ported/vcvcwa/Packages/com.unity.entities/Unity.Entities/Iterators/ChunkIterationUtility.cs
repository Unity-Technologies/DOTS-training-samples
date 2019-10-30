using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Entities
{
    /// <summary>
    ///     Enables iteration over chunks belonging to a set of archetypes.
    /// </summary>
    internal unsafe struct ChunkIterationUtility
    {
        /// <summary>
        /// Creates a NativeArray with all the chunks in a given archetype filtered by the provided EntityQueryFilter.
        /// This function will not sync the needed types in the EntityQueryFilter so they have to be synced manually before calling this function.
        /// </summary>
        /// <param name="matchingArchetypes">List of matching archetypes.</param>
        /// <param name="allocator">Allocator to use for the array.</param>
        /// <param name="jobHandle">Handle to the GatherChunks job used to fill the output array.</param>
        /// <param name="filter">Filter used to filter the resulting chunks</param>
        /// <param name="dependsOn">All jobs spawned will depend on this JobHandle</param>
        /// <returns>NativeArray of all the chunks in the matchingArchetypes list.</returns>
        public static NativeArray<ArchetypeChunk> CreateArchetypeChunkArrayWithoutSync(UnsafeMatchingArchetypePtrList matchingArchetypes,
            Allocator allocator, out JobHandle jobHandle, ref EntityQueryFilter filter,
            JobHandle dependsOn = default(JobHandle))
        {
            var archetypeCount = matchingArchetypes.Length;

            var offsets =
                new NativeArray<int>(archetypeCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var chunkCount = 0;
            {
                for (int i = 0; i < matchingArchetypes.Length; ++i)
                {
                    var archetype = matchingArchetypes.Ptr[i]->Archetype;
                    offsets[i] = chunkCount;
                    chunkCount += archetype->Chunks.Count;
                }
            }

            if (!filter.RequiresMatchesFilter)
            {
                var chunks = new NativeArray<ArchetypeChunk>(chunkCount, allocator, NativeArrayOptions.UninitializedMemory);
                var gatherChunksJob = new GatherChunks
                {
                    MatchingArchetypes = matchingArchetypes.Ptr,
                    entityComponentStore = matchingArchetypes.entityComponentStore,
                    Offsets = offsets,
                    Chunks = chunks
                };
                var gatherChunksJobHandle = gatherChunksJob.Schedule(archetypeCount,1, dependsOn);
                jobHandle = gatherChunksJobHandle;

                return chunks;
            }
            else
            {
                var filteredCounts =  new NativeArray<int>(archetypeCount+1, Allocator.TempJob);
                var sparseChunks = new NativeArray<ArchetypeChunk>(chunkCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                var gatherChunksJob = new GatherChunksWithFiltering
                {
                    MatchingArchetypes = matchingArchetypes.Ptr,
                    Filter = filter,
                    Offsets = offsets,
                    FilteredCounts = filteredCounts,
                    SparseChunks = sparseChunks,
                    entityComponentStore = matchingArchetypes.entityComponentStore
                };
                gatherChunksJob.Schedule(archetypeCount,1, dependsOn).Complete();

                // accumulated filtered counts: filteredCounts[i] becomes the destination offset
                int totalChunks = 0;
                for (int i = 0; i < archetypeCount; ++i)
                {
                    int currentCount = filteredCounts[i];
                    filteredCounts[i] = totalChunks;
                    totalChunks += currentCount;
                }
                filteredCounts[archetypeCount] = totalChunks;

                var joinedChunks = new NativeArray<ArchetypeChunk>(totalChunks, allocator, NativeArrayOptions.UninitializedMemory);

                jobHandle = new JoinChunksJob
                {
                    DestinationOffsets = filteredCounts,
                    SparseChunks = sparseChunks,
                    Offsets = offsets,
                    JoinedChunks = joinedChunks
                }.Schedule(archetypeCount, 1);

                return joinedChunks;
            }
        }

        /// <summary>
        /// Creates a NativeArray with all the chunks in a given archetype filtered by the provided EntityQueryFilter.
        /// This function will sync the needed types in the EntityQueryFilter.
        /// </summary>
        /// <param name="matchingArchetypes">List of matching archetypes.</param>
        /// <param name="allocator">Allocator to use for the array.</param>
        /// <param name="jobHandle">Handle to the GatherChunks job used to fill the output array.</param>
        /// <param name="filter">Filter used to filter the resulting chunks</param>
        /// <param name="safetyManager">The ComponentJobSafetyManager belonging to this world</param>
        /// <param name="dependsOn">All jobs spawned will depend on this JobHandle</param>

        /// <returns>NativeArray of all the chunks in the matchingArchetypes list.</returns>
        public static NativeArray<ArchetypeChunk> CreateArchetypeChunkArray(
            UnsafeMatchingArchetypePtrList matchingArchetypes, Allocator allocator,
            ref EntityQueryFilter filter, ComponentJobSafetyManager* safetyManager)
        {
            EntityQuery.SyncFilterTypes(ref matchingArchetypes, ref filter, safetyManager);
            var chunks = CreateArchetypeChunkArrayWithoutSync(matchingArchetypes, allocator, out var jobHandle, ref filter);
            jobHandle.Complete();
            return chunks;
        }

        /// <summary>
        ///     Creates a NativeArray containing the entities in a given EntityQuery.
        /// </summary>
        /// <param name="matchingArchetypes">List of matching archetypes.</param>
        /// <param name="allocator">Allocator to use for the array.</param>
        /// <param name="type">An atomic safety handle required by GatherEntitiesJob so it can call GetNativeArray() on chunks.</param>
        /// <param name="entityQuery">EntityQuery to gather entities from.</param>
        /// <param name="filter">EntityQueryFilter for calculating the length of the output array.</param>
        /// <param name="jobHandle">Handle to the GatherEntitiesJob job used to fill the output array.</param>
        /// <param name="dependsOn">Handle to a job this GatherEntitiesJob must wait on.</param>
        /// <returns>NativeArray of the entities in a given EntityQuery.</returns>
        public static NativeArray<Entity> CreateEntityArray(UnsafeMatchingArchetypePtrList matchingArchetypes,
            Allocator allocator,
            ArchetypeChunkEntityType type,
            EntityQuery entityQuery,
            ref EntityQueryFilter filter,
            out JobHandle jobHandle,
            JobHandle dependsOn)

        {
            var entityCount = CalculateEntityCount(matchingArchetypes, ref filter);

            var job = new GatherEntitiesJob
            {
                EntityType = type,
                Entities = new NativeArray<Entity>(entityCount, allocator)
            };
            jobHandle = job.Schedule(entityQuery, dependsOn);

            return job.Entities;
        }

        public static NativeArray<T> CreateComponentDataArray<T>(UnsafeMatchingArchetypePtrList matchingArchetypes,
            Allocator allocator,
            ArchetypeChunkComponentType<T> type,
            EntityQuery entityQuery,
            ref EntityQueryFilter filter,
            out JobHandle jobHandle,
            JobHandle dependsOn)
            where T : struct, IComponentData
        {
            var entityCount = CalculateEntityCount(matchingArchetypes, ref filter);

            var job = new GatherComponentDataJob<T>
            {
                ComponentData = new NativeArray<T>(entityCount, allocator),
                ComponentType = type
            };
            jobHandle = job.Schedule(entityQuery, dependsOn);

            return job.ComponentData;
        }

        // In order to maximize EntityQuery.ForEach performance we want to avoid data allocation, as ForEach is main thread only we can afford to allocate a big array and use it to store result.
        // Let's not forget that calls to ForEach can be re-entrant, so we need to cover this use case too.
        // The current solution is to allocate an array of a fixed size (16kb) where will will store the result, we will fall back to the jobified implementation if we run out of space in the buffer
        static readonly int k_EntityQueryResultBufferSize = 16384 / sizeof(Entity);
        static Entity* s_EntityQueryResultBuffer = null;
        internal static int currentOffsetInResultBuffer = 0;

        public static void GatherEntitiesToArray(EntityQueryData* queryData, ref EntityQueryFilter filter, out EntityQuery.GatherEntitiesResult result)
        {
            if (s_EntityQueryResultBuffer == null)
            {
                s_EntityQueryResultBuffer = (Entity*)UnsafeUtility.Malloc(k_EntityQueryResultBufferSize * sizeof(Entity), 64, Allocator.Persistent);
            }

            var buffer = s_EntityQueryResultBuffer;
            var curOffset = currentOffsetInResultBuffer;

            // Main method that copies the entities of each chunk of a matching archetype to the buffer 
            bool AddArchetype(MatchingArchetype* matchingArchetype, ref EntityQueryFilter queryFilter)
            {
                var archetype = matchingArchetype->Archetype;
                var entityCountInArchetype = archetype->EntityCount;
                if (entityCountInArchetype == 0)
                {
                    return true;
                }
                
                var chunkCount = archetype->Chunks.Count;
                var chunks = archetype->Chunks.p;
                var counts = archetype->Chunks.GetChunkEntityCountArray();
                
                for (int i = 0; i < chunkCount; ++i)
                {
                    // Ignore the chunk if the query uses filter and the chunk doesn't comply
                    if (queryFilter.RequiresMatchesFilter && (chunks[i]->MatchesFilter(matchingArchetype, ref queryFilter) == false))
                    {
                        continue;
                    }
                    var entityCountInChunk = counts[i];

                    if ((curOffset + entityCountInChunk) > k_EntityQueryResultBufferSize)
                    {
                        return false;
                    }
                    
                    UnsafeUtility.MemCpy(buffer + curOffset, chunks[i]->Buffer, entityCountInChunk * sizeof(Entity));
                    curOffset += entityCountInChunk;
                }

                return true;
            }
            
            // Parse all the matching archetypes and add the entities that fits the query and its filter
            bool success = true;
            ref var matchingArchetypes = ref queryData->MatchingArchetypes;
            for (var m = 0; m < matchingArchetypes.Length; m++)
            {
                var match = matchingArchetypes.Ptr[m];
                if (!AddArchetype(match, ref filter))
                {
                    success = false;
                    break;
                }
            }

            result = new EntityQuery.GatherEntitiesResult { StartingOffset = currentOffsetInResultBuffer };
            if (success)
            {
                result.EntityCount = curOffset - currentOffsetInResultBuffer;
                result.EntityBuffer = s_EntityQueryResultBuffer + currentOffsetInResultBuffer;
            }

            currentOffsetInResultBuffer = curOffset;
        }
        
        public static void CopyFromComponentDataArray<T>(UnsafeMatchingArchetypePtrList matchingArchetypes,
            NativeArray<T> componentDataArray,
            ArchetypeChunkComponentType<T> type,
            EntityQuery entityQuery,
            ref EntityQueryFilter filter,
            out JobHandle jobHandle,
            JobHandle dependsOn)
            where T :struct, IComponentData
        {
            var job = new CopyComponentArrayToChunks<T>
            {
                ComponentData = componentDataArray,
                ComponentType = type
            };
            jobHandle = job.Schedule(entityQuery, dependsOn);
        }

        /// <summary>
        ///     Total number of entities contained in a given MatchingArchetype list.
        /// </summary>
        /// <param name="matchingArchetypes">List of matching archetypes.</param>
        /// <param name="filter">EntityQueryFilter to use when calculating total number of entities.</param>
        /// <returns>Number of entities</returns>
        public static int CalculateEntityCount(UnsafeMatchingArchetypePtrList matchingArchetypes, ref EntityQueryFilter filter)
        {
            var length = 0;
            if (!filter.RequiresMatchesFilter)
            {
                for (var m = 0; m < matchingArchetypes.Length; ++m)
                {
                    var match = matchingArchetypes.Ptr[m];
                    length += match->Archetype->EntityCount;
                }
            }
            else
            {
                for (var m = 0; m < matchingArchetypes.Length; ++m)
                {
                    var match = matchingArchetypes.Ptr[m];
                    if (match->Archetype->EntityCount <= 0)
                        continue;

                    int filteredCount = 0;
                    var archetype = match->Archetype;
                    int chunkCount = archetype->Chunks.Count;
                    var chunkEntityCountArray = archetype->Chunks.GetChunkEntityCountArray();

                    for (var i = 0; i < chunkCount; ++i)
                    {
                        if (match->ChunkMatchesFilter(i, ref filter))
                            filteredCount += chunkEntityCountArray[i];
                    }

                    length += filteredCount;
                }
            }

            return length;
        }
        
         /// <summary>
        ///     Total number of chunks in a given MatchingArchetype list.
        /// </summary>
        /// <param name="matchingArchetypes">List of matching archetypes.</param>
        /// <returns>Number of chunks in a list of archetypes.</returns>
        internal static int CalculateChunkCount(UnsafeMatchingArchetypePtrList matchingArchetypes, ref EntityQueryFilter filter)
        {
            var totalChunkCount = 0;

            // If no filter, then fast path it
            if (!filter.RequiresMatchesFilter)
            {
                for (var m = 0; m < matchingArchetypes.Length; ++m)
                {
                    var match = matchingArchetypes.Ptr[m];
                    totalChunkCount += match->Archetype->Chunks.Count;
                }

                return totalChunkCount;
            }

            // Otherwise do filtering
            for (var m = 0; m < matchingArchetypes.Length; ++m)
            {
                var match = matchingArchetypes.Ptr[m];
                var archetype = match->Archetype;
                int chunkCount = archetype->Chunks.Count;
                
                for (var i = 0; i < chunkCount; ++i)
                {
                    if (match->ChunkMatchesFilter(i, ref filter))
                        totalChunkCount++;
                }
            }

            return totalChunkCount;
        }
         

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal static BufferAccessor<T> GetChunkBufferAccessor<T>(Chunk* chunk, bool isWriting, int typeIndexInArchetype, uint systemVersion, AtomicSafetyHandle safety0, AtomicSafetyHandle safety1)
#else
        internal static BufferAccessor<T> GetChunkBufferAccessor<T>(Chunk* chunk, bool isWriting, int typeIndexInArchetype, uint systemVersion)
#endif
            where T : struct, IBufferElementData
        {
            var archetype = chunk->Archetype;
            int internalCapacity = archetype->BufferCapacities[typeIndexInArchetype];

            if (isWriting)
                chunk->SetChangeVersion(typeIndexInArchetype, systemVersion);

            var buffer = chunk->Buffer;
            var length = chunk->Count;
            var startOffset = archetype->Offsets[typeIndexInArchetype];
            int stride = archetype->SizeOfs[typeIndexInArchetype];
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BufferAccessor<T>(buffer + startOffset, length, stride, !isWriting, safety0, safety1, internalCapacity);
#else
            return new BufferAccessor<T>(buffer + startOffset, length, stride, internalCapacity);
#endif
        }

        internal static void* GetChunkComponentDataPtr(Chunk* chunk, bool isWriting, int indexInArchetype, uint systemVersion)
        {
            var archetype = chunk->Archetype;

            if (isWriting)
                chunk->SetChangeVersion(indexInArchetype, systemVersion);

            return chunk->Buffer + archetype->Offsets[indexInArchetype];
        }

        internal static JobHandle PreparePrefilteredChunkLists(int unfilteredChunkCount, UnsafeMatchingArchetypePtrList archetypes, EntityQueryFilter filter, JobHandle dependsOn, ScheduleMode mode, out NativeArray<byte> prefilterDataArray, out void* deferredCountData)
        {
            // Allocate one buffer for all prefilter data and distribute it
            // We keep the full buffer as a "dummy array" so we can deallocate it later with [DeallocateOnJobCompletion]
            var sizeofChunkArray = sizeof(ArchetypeChunk) * unfilteredChunkCount;
            var sizeofIndexArray = sizeof(int) * unfilteredChunkCount;
            var prefilterDataSize = sizeofChunkArray + sizeofIndexArray + sizeof(int);

            var prefilterData = (byte*) UnsafeUtility.Malloc(prefilterDataSize, 64, Allocator.TempJob);
            prefilterDataArray =NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(prefilterData, prefilterDataSize, Allocator.TempJob);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref prefilterDataArray, AtomicSafetyHandle.Create());
#endif

            JobHandle prefilterHandle = default(JobHandle);

            if (filter.RequiresMatchesFilter)
            {
                var prefilteringJob = new GatherChunksAndOffsetsWithFilteringJob
                {
                    Archetypes = archetypes,
                    Filter = filter,
                    PrefilterData = prefilterData,
                    UnfilteredChunkCount = unfilteredChunkCount
                };
                if (mode == ScheduleMode.Batched)
                    prefilterHandle = prefilteringJob.Schedule(dependsOn);
                else
                    prefilteringJob.Run();
            }
            else
            {
                var gatherJob = new GatherChunksAndOffsetsJob
                {
                    Archetypes = archetypes,
                    PrefilterData = prefilterData,
                    UnfilteredChunkCount = unfilteredChunkCount,
                    entityComponentStore = archetypes.entityComponentStore
                };
                if (mode == ScheduleMode.Batched)
                    prefilterHandle = gatherJob.Schedule(dependsOn);
                else
                    gatherJob.Run();
            }

            // ScheduleParallelForDeferArraySize expects a ptr to a structure with a void* and a count.
            // It only uses the count, so this is safe to fudge
            deferredCountData = prefilterData + sizeofChunkArray + sizeofIndexArray;
            deferredCountData = (byte*)deferredCountData - sizeof(void*);

            return prefilterHandle;
        }
        
        internal static void UnpackPrefilterData(NativeArray<byte> prefilterData, out ArchetypeChunk* chunks, out int* entityOffsets, out int filteredChunkCount)
        {
            chunks = (ArchetypeChunk*) prefilterData.GetUnsafePtr();

            filteredChunkCount = *(int*)((byte*) prefilterData.GetUnsafePtr() + prefilterData.Length - sizeof(int));
            entityOffsets = (int*) (chunks + filteredChunkCount);
        }
    }
}
