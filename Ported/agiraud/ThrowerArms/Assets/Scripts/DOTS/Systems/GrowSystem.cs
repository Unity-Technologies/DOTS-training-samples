using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public class GrowSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;

    [BurstCompile]
    struct GrowSystemJob : IJobForEachWithEntity<Scale>
    {
        public EntityCommandBuffer.Concurrent cmd;
        public float deltaTime;

        public void Execute(Entity entity, int index, ref Scale scale)
        {
            scale.Value += (1f - scale.Value) * 2.0f * deltaTime;

            //if (scale.Value > 0.999f)
            //{
            //    cmd.RemoveComponent(index, entity, typeof(Scale));
            //}
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new GrowSystemJob();
        job.cmd = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        job.deltaTime = Time.deltaTime;

        var jobHandle = job.Schedule(this, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}
