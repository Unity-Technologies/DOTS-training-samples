using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class Sys_Holding_CheckOwnership : JobComponentSystem
{

    struct Sys_Holding_CheckOwnershipJob : IJobChunk
    {
        public EntityCommandBuffer.Concurrent ecb;
        [ReadOnly] public ComponentDataFromEntity<C_Held> HeldType;
        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkComponentType<C_Holding> HoldingType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var Holders = chunk.GetNativeArray(HoldingType);
            var Entities = chunk.GetNativeArray(EntityType);
            for(int i = 0; i < chunk.Count; ++i)
            {
                if (HeldType[Holders[i].ent].Holder != Entities[i])
                {
                    ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Holding));
                }
            }            
        }
    }

    public EntityQuery m_group;
    public EntityCommandBufferSystem m_entityCommandBufferSystem;
    protected override void OnCreate()
    {
        m_group = GetEntityQuery(ComponentType.ReadOnly<C_Held>(),ComponentType.ReadOnly<Tag_IsHeld>(), ComponentType.ReadOnly<C_Holding>());
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        m_group.SetFilterChanged(typeof(C_Holding));
        var job = new Sys_Holding_CheckOwnershipJob()
        {
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            HeldType = GetComponentDataFromEntity<C_Held>(true),
            EntityType = GetArchetypeChunkEntityType(),
            HoldingType = GetArchetypeChunkComponentType<C_Holding>(true)
        }.Schedule(m_group, inputDependencies);

        m_entityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}