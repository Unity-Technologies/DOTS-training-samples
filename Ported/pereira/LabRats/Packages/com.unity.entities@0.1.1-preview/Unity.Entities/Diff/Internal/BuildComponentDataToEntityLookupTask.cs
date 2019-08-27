using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities
{
    public readonly struct BuildComponentDataToEntityLookupTask<TComponentData> : IDisposable
        where TComponentData : unmanaged, IComponentData, IEquatable<TComponentData>
    {
        [BurstCompile]
        private struct BuildComponentDataToEntityLookup : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [WriteOnly] public NativeMultiHashMap<TComponentData, Entity>.ParallelWriter ComponentDataToEntity;

            public int ComponentTypeIndex;

            public unsafe void Execute(int index)
            {
                var chunk = Chunks[index].m_Chunk;
                var archetype = chunk->Archetype;
                var entities = (Entity*) (ChunkUtility.GetBuffer(chunk) + archetype->Offsets[0]);
                var componentTypeIndexInArchetype = ChunkDataUtility.GetIndexInTypeArray(archetype, ComponentTypeIndex);
                var componentBuffer = (TComponentData*) (ChunkUtility.GetBuffer(chunk) + archetype->Offsets[componentTypeIndexInArchetype]);
                for (var i = 0; i < chunk->Count; ++i)
                {
                    ComponentDataToEntity.Add(componentBuffer[i], entities[i]);
                }
            }
        }

        private readonly NativeMultiHashMap<TComponentData, Entity> m_ComponentDataToEntity;

        public bool IsCreated { get; }

        public BuildComponentDataToEntityLookupTask(int capacity, Allocator allocator)
        {
            m_ComponentDataToEntity = new NativeMultiHashMap<TComponentData, Entity>(capacity, allocator);
            IsCreated = true;
        }

        public void Dispose()
        {
            if (!IsCreated)
            {
                return;
            }
            
            m_ComponentDataToEntity.Dispose();
        }

        public JobHandle Schedule(NativeArray<ArchetypeChunk> chunks)
        {
            var handle = new BuildComponentDataToEntityLookup
            {
                Chunks = chunks,
                ComponentDataToEntity = m_ComponentDataToEntity.AsParallelWriter(),
                ComponentTypeIndex = TypeManager.GetTypeIndex<TComponentData>()
            }.Schedule(chunks.Length, 64);
            return handle;
        }

        public NativeMultiHashMap<TComponentData, Entity> GetComponentDataToEntityMap()
        {
            return m_ComponentDataToEntity;
        }
    }
}