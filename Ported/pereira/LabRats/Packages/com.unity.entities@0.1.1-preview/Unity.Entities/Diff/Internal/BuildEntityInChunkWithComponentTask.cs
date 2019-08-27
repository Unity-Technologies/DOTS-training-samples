using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities
{
    internal readonly struct BuildEntityInChunkWithComponentTask<TComponent> : IDisposable
        where TComponent : unmanaged, IComponentData, IComparable<TComponent>
    {
        [BurstCompile]
        private struct BuildEntityInChunkWithComponent : IJobParallelFor
        {
            public int ComponentTypeIndex;
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly] public NativeArray<ChunkChangeFlags> ChunkChangeFlags;
            [ReadOnly] public NativeArray<int> Offsets;
            [NativeDisableParallelForRestriction, WriteOnly] public NativeArray<EntityInChunkWithComponent<TComponent>> Entities;

            public unsafe void Execute(int index)
            {
                var chunk = Chunks[index].m_Chunk;
                var chunkChangeFlags = ChunkChangeFlags[index];
                var baseIndex = Offsets[index];

                var archetype = chunk->Archetype;
                var componentIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(archetype, ComponentTypeIndex);
                var componentBuffer = (TComponent*) (ChunkUtility.GetBuffer(chunk) + archetype->Offsets[componentIndexInArchetype]);

                for (var i = 0; i < chunk->Count; ++i)
                {
                    Entities[baseIndex + i] = new EntityInChunkWithComponent<TComponent>
                    {
                        EntityInChunk = new EntityInChunk {Chunk = chunk, IndexInChunk = i},
                        Component = componentBuffer[i],
                        ChunkChangeFlags = chunkChangeFlags
                    };
                }
            }
        }
        
        [BurstCompile]
        private struct SortNativeArrayWithComparer<T, TComparer> : IJob
            where T : struct
            where TComparer : struct, IComparer<T>
        {
            public NativeArray<T> Array;
            public void Execute() => Array.Sort(new TComparer());
        }

        private readonly ArchetypeChunkCollection m_ArchetypeChunks;
        private readonly NativeArray<EntityInChunkWithComponent<TComponent>> m_Entities;  
        
        public BuildEntityInChunkWithComponentTask(ArchetypeChunkCollection archetypeChunks, Allocator allocator)
        {
            m_ArchetypeChunks = archetypeChunks;
            m_Entities = new NativeArray<EntityInChunkWithComponent<TComponent>>(m_ArchetypeChunks.EntityCount, allocator, NativeArrayOptions.UninitializedMemory);
        }

        public void Dispose()
        {
            m_Entities.Dispose();
        }

        public JobHandle Schedule()
        {
            var handle = new BuildEntityInChunkWithComponent
            {
                ComponentTypeIndex = TypeManager.GetTypeIndex<TComponent>(),
                Chunks = m_ArchetypeChunks.Chunks,
                ChunkChangeFlags = m_ArchetypeChunks.ChunkChangeFlags,
                Offsets = m_ArchetypeChunks.EntityCountOffsets,
                Entities = m_Entities
            }.Schedule(m_ArchetypeChunks.Chunks.Length, 64);

            handle = new SortNativeArrayWithComparer<EntityInChunkWithComponent<TComponent>, EntityInChunkWithComponentComparer<TComponent>>
            {
                Array = m_Entities
            }.Schedule(handle);
            
            return handle;
        }

        public NativeArray<EntityInChunkWithComponent<TComponent>> GetEntities()
        {
            return m_Entities;
        }
    }
}
