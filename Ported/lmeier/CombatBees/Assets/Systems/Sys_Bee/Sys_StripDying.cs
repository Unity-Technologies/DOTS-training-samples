using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

//[UpdateInGroup(typeof(LateSimulationSystemGroup))]//, UpdateBefore(typeof(Sys_PruneDeadTargets))]
public class Sys_StripDying : JobComponentSystem
{

    EntityQuery m_StripDyingTargetQuery;
    struct Sys_StripDyingTargetJob : IJobChunk
    {
        public EntityCommandBuffer.Concurrent ecb;
        [ReadOnly] public ArchetypeChunkEntityType EntityType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var Entities = chunk.GetNativeArray(EntityType);

            for(int i = 0; i < chunk.Count; ++i)
            {
                ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Target));
            }

        }
    }

    EntityQuery m_StripDyingHoldingQuery;
    struct Sys_StripDyingHoldingJob : IJobChunk
    {
        public EntityCommandBuffer.Concurrent ecb;
        [ReadOnly] public ArchetypeChunkEntityType EntityType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var Entities = chunk.GetNativeArray(EntityType);

            for (int i = 0; i < chunk.Count; ++i)
            {
                ecb.RemoveComponent(chunkIndex, Entities[i], typeof(C_Holding));
            }

        }
    }

    EntityCommandBufferSystem m_entityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

        m_StripDyingHoldingQuery = GetEntityQuery(ComponentType.ReadOnly<Tag_IsDying>(), ComponentType.ReadOnly<C_Holding>());
        m_StripDyingTargetQuery = GetEntityQuery(ComponentType.ReadOnly<Tag_IsDying>(), ComponentType.ReadOnly<C_Target>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var stripTarget = new Sys_StripDyingTargetJob()
        {
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            EntityType = GetArchetypeChunkEntityType()
        }.Schedule(m_StripDyingTargetQuery, inputDependencies);

        
        var stripHolding = new Sys_StripDyingHoldingJob()
        {
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            EntityType = GetArchetypeChunkEntityType()
        }.Schedule(m_StripDyingHoldingQuery, inputDependencies);

        m_entityCommandBufferSystem.AddJobHandleForProducer(stripTarget);
        m_entityCommandBufferSystem.AddJobHandleForProducer(stripHolding);
        
        return JobHandle.CombineDependencies(stripTarget, stripHolding);
    }
}