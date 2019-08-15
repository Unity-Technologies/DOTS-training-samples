using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ExitIntersectionSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct ExitIntersectionJob : IJobForEachWithEntity<ReachedEndOfSpline, ExitIntersectionData>
    {
        [ReadOnly] public DynamicBuffer<Spline> SplineBuffer;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref ReachedEndOfSpline reachedEndOfSpline, [ReadOnly] ref ExitIntersectionData exitIntersectionData)
        {
            var spline = SplineBuffer[exitIntersectionData.TargetSplineId];
            CommandBuffer.SetComponent(index, entity, new SplineData { Spline = spline });
            CommandBuffer.RemoveComponent<ExitIntersectionData>(index, entity);
            CommandBuffer.RemoveComponent<ReachedEndOfSpline>(index, entity);
            CommandBuffer.AddComponent<InterpolatorTComponent>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle handle)
    {
        var splineBuffer = EntityManager.GetBuffer<Spline>(GetSingletonEntity<Spline>());

        var job = new ExitIntersectionJob
        {
            SplineBuffer = splineBuffer,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(this, handle);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}