using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(GravitySystem))]
public class DestroySystem : JobComponentSystem
{
    EntityQuery m_group;
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    //[BurstCompile]
    struct DestroySystemJob : IJobForEachWithEntity<Translation>
    {
        public float3 boundsMin;
        public float3 boundsMax;
        public EntityCommandBuffer.Concurrent cmd;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation)
        {
            if (translation.Value.x < boundsMin.x || translation.Value.y < boundsMin.y || translation.Value.z < boundsMin.z ||
                translation.Value.x > boundsMax.x || translation.Value.y > boundsMax.y || translation.Value.z > boundsMax.z)
            {
                cmd.AddComponent<ResetPosition>(index, entity);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new DestroySystemJob();

        // TODO: extract and update based on the number of arms, etc
        job.boundsMin = new Vector3(-100, -5, -100);
        job.boundsMax = new Vector3( 10, 50,  100);
        job.cmd = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        var jobHandle = job.Schedule(m_group, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        m_group = GetEntityQuery(ComponentType.ReadOnly<Mover>(), ComponentType.ReadOnly<Translation>());
    }
}
