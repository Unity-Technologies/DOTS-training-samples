using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class Sys_DestroyDead : JobComponentSystem
{

    struct Sys_DestroyDeadJob : IJobChunk
    {

        public EntityCommandBuffer.Concurrent ecb;

        [ReadOnly] public ArchetypeChunkEntityType entityType;


        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var entities = chunk.GetNativeArray(entityType);

            for (int i = 0; i < chunk.Count; ++i)
            {
                ecb.DestroyEntity(chunkIndex, entities[i]);
            }
        }
    }

    private EntityCommandBufferSystem m_EntityCommandBufferSystem;

    private EntityQuery m_Group;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

        m_Group = GetEntityQuery(ComponentType.ReadOnly<Tag_IsDead>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_DestroyDeadJob()
        {
            ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            entityType = GetArchetypeChunkEntityType()
        }.Schedule(m_Group, inputDependencies);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}