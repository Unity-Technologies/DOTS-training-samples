using System;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Entities
{
    [BurstCompile]
    unsafe struct GatherChunks : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction] public EntityComponentStore* entityComponentStore;
        [NativeDisableUnsafePtrRestriction] public MatchingArchetype** MatchingArchetypes;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<int> Offsets;
        [NativeDisableParallelForRestriction] public NativeArray<ArchetypeChunk> Chunks;

        public void Execute(int index)
        {
            var archetype = MatchingArchetypes[index]->Archetype;
            var chunkCount = archetype->Chunks.Count;
            var offset = Offsets[index];
            for (int i = 0; i < chunkCount; i++)
            {
                var srcChunk = archetype->Chunks.p[i];
                Chunks[offset+i] = new ArchetypeChunk(srcChunk,entityComponentStore);
            }
        }
    }

    [BurstCompile]
    internal unsafe struct GatherChunksAndOffsetsJob : IJob
    {
        public UnsafeMatchingArchetypePtrList Archetypes;
        [NativeDisableUnsafePtrRestriction] public EntityComponentStore* entityComponentStore;

        [NativeDisableUnsafePtrRestriction]
        public void* PrefilterData;
        public int   UnfilteredChunkCount;

        public void Execute()
        {
            var chunks = (ArchetypeChunk*) PrefilterData;
            var entityIndices = (int*) (chunks + UnfilteredChunkCount);

            var chunkCounter = 0;
            var entityOffsetPrefixSum = 0;

            for (var m = 0; m < Archetypes.Length; ++m)
            {
                var match = Archetypes.Ptr[m];
                if (match->Archetype->EntityCount <= 0)
                    continue;

                var archetype = match->Archetype;
                int chunkCount = archetype->Chunks.Count;
                var chunkEntityCountArray = archetype->Chunks.GetChunkEntityCountArray();

                for (int chunkIndex = 0; chunkIndex < chunkCount; ++chunkIndex)
                {
                    chunks[chunkCounter] = new ArchetypeChunk(archetype->Chunks.p[chunkIndex], entityComponentStore);
                    entityIndices[chunkCounter++] = entityOffsetPrefixSum;
                    entityOffsetPrefixSum += chunkEntityCountArray[chunkIndex];
                }
            }

            var outChunkCounter = entityIndices + UnfilteredChunkCount;
            *outChunkCounter = chunkCounter;
        }
    }

    [BurstCompile]
    unsafe struct GatherChunksWithFiltering : IJobParallelFor
    {
        [NativeDisableUnsafePtrRestriction] public EntityComponentStore* entityComponentStore;
        [NativeDisableUnsafePtrRestriction] public MatchingArchetype** MatchingArchetypes;
        public EntityQueryFilter Filter;

        [ReadOnly] public NativeArray<int> Offsets;
        public NativeArray<int> FilteredCounts;

        [NativeDisableParallelForRestriction] public NativeArray<ArchetypeChunk> SparseChunks;

        public void Execute(int index)
        {
            int filteredCount = 0;
            var match = MatchingArchetypes[index];
            var archetype = match->Archetype;
            int chunkCount = archetype->Chunks.Count;
            var writeIndex = Offsets[index];
            var archetypeChunks = archetype->Chunks.p;
            
            for (var i = 0; i < chunkCount; ++i)
            {
                if (match->ChunkMatchesFilter(i, ref Filter))
                    SparseChunks[writeIndex + filteredCount++] =
                        new ArchetypeChunk(archetypeChunks[i], entityComponentStore);
            }

            FilteredCounts[index] = filteredCount;
        }
    }

    [BurstCompile]
    internal unsafe struct GatherChunksAndOffsetsWithFilteringJob : IJob
    {
        public UnsafeMatchingArchetypePtrList Archetypes;
        public EntityQueryFilter Filter;

        [NativeDisableUnsafePtrRestriction]
        public void* PrefilterData;
        public int   UnfilteredChunkCount;

        public void Execute()
        {
            var chunks = (ArchetypeChunk*) PrefilterData;
            var entityIndices = (int*) (chunks + UnfilteredChunkCount);
            
            var filteredChunkCount = 0;
            var filteredEntityOffset = 0;

            for (var m = 0; m < Archetypes.Length; ++m)
            {
                var match = Archetypes.Ptr[m];
                if (match->Archetype->EntityCount <= 0)
                    continue;

                var archetype = match->Archetype;
                int chunkCount = archetype->Chunks.Count;
                var chunkEntityCountArray = archetype->Chunks.GetChunkEntityCountArray();

                for (var i = 0; i < chunkCount; ++i)
                {
                    if (match->ChunkMatchesFilter(i, ref Filter))
                    {
                        chunks[filteredChunkCount] =
                            new ArchetypeChunk(archetype->Chunks.p[i], Archetypes.entityComponentStore);
                        entityIndices[filteredChunkCount++] = filteredEntityOffset;
                        filteredEntityOffset += chunkEntityCountArray[i];
                    }
                }
            }

            UnsafeUtility.MemMove(chunks + filteredChunkCount, chunks + UnfilteredChunkCount, filteredChunkCount * sizeof(int));

            var chunkCounter = entityIndices + UnfilteredChunkCount;
            *chunkCounter = filteredChunkCount;
        }
    }

    unsafe struct JoinChunksJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction] public NativeArray<int> DestinationOffsets;
        [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction] public NativeArray<ArchetypeChunk> SparseChunks;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<int> Offsets;
        [NativeDisableParallelForRestriction] public NativeArray<ArchetypeChunk> JoinedChunks;

        public void Execute(int index)
        {
            int destOffset = DestinationOffsets[index];
            int count = DestinationOffsets[index+1]-destOffset;
            if(count != 0)
                NativeArray<ArchetypeChunk>.Copy(SparseChunks, Offsets[index], JoinedChunks, destOffset, count);
        }
    }

    [BurstCompile]
    unsafe struct GatherEntitiesJob : IJobChunk
    {
        public NativeArray<Entity> Entities;
        [ReadOnly]public ArchetypeChunkEntityType EntityType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            var destinationPtr = (Entity*)Entities.GetUnsafePtr() + entityOffset;
            var sourcePtr = chunk.GetNativeArray(EntityType).GetUnsafeReadOnlyPtr();
            var copySizeInBytes = sizeof(Entity) * chunk.Count;

            UnsafeUtility.MemCpy(destinationPtr, sourcePtr, copySizeInBytes);
        }
    }

    [BurstCompile]
    unsafe struct GatherComponentDataJob<T> : IJobChunk
        where T : struct,IComponentData
    {
        public NativeArray<T> ComponentData;
        [ReadOnly]public ArchetypeChunkComponentType<T> ComponentType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            var sourcePtr = chunk.GetNativeArray(ComponentType).GetUnsafeReadOnlyPtr();
            var destinationPtr = (byte*) ComponentData.GetUnsafePtr() + UnsafeUtility.SizeOf<T>() * entityOffset;
            var copySizeInBytes = UnsafeUtility.SizeOf<T>() * chunk.Count;

            UnsafeUtility.MemCpy(destinationPtr, sourcePtr, copySizeInBytes);
        }
    }

    [BurstCompile]
    unsafe struct CopyComponentArrayToChunks<T> : IJobChunk
        where T : struct,IComponentData
    {
        [ReadOnly]
        public NativeArray<T> ComponentData;
        public ArchetypeChunkComponentType<T> ComponentType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityOffset)
        {
            var destinationPtr = chunk.GetNativeArray(ComponentType).GetUnsafePtr();
            var srcPtr = (byte*) ComponentData.GetUnsafeReadOnlyPtr() + UnsafeUtility.SizeOf<T>() * entityOffset;
            var copySizeInBytes = UnsafeUtility.SizeOf<T>() * chunk.Count;

            UnsafeUtility.MemCpy(destinationPtr, srcPtr, copySizeInBytes);
        }
    }
    
    [BurstCompile]
    unsafe struct CalculateEntityCountJob : IJob
    {
        public UnsafeMatchingArchetypePtrList MatchingArchetypes;
        public EntityQueryFilter Filter;

        [NativeDisableUnsafePtrRestriction] 
        public int* OutEntityCount;

        public void Execute()
        {
            *OutEntityCount = ChunkIterationUtility.CalculateEntityCount(MatchingArchetypes, ref Filter);
        }
    }

    [BurstCompile]
    unsafe struct CalculateChunkCountJob : IJob
    {
        public UnsafeMatchingArchetypePtrList MatchingArchetypes;
        public EntityQueryFilter Filter;

        [NativeDisableUnsafePtrRestriction] 
        public int* OutChunkCount;

        public void Execute()
        {
            *OutChunkCount = ChunkIterationUtility.CalculateChunkCount(MatchingArchetypes, ref Filter);
        }
    }
}
