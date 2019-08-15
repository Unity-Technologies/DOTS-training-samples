using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class EnterRoadSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct ExitIntersectionJob : IJobForEachWithEntity<ReachedEndOfSplineComponent, ExitIntersectionComponent>
    {
        [ReadOnly] public DynamicBuffer<SplineBufferElementData> SplineBuffer;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] ref ReachedEndOfSplineComponent reachedEndOfSplineComponent, [ReadOnly] ref ExitIntersectionComponent exitIntersectionComponent)
        {
            var spline = SplineBuffer[exitIntersectionComponent.TargetSplineId];
            var splineComponent = new SplineComponent
            {
                splineId = exitIntersectionComponent.TargetSplineId,
                Spline = spline, 
                IsInsideIntersection = false,
                t = 0,
            };
            CommandBuffer.SetComponent(index, entity, splineComponent);
            CommandBuffer.RemoveComponent<ExitIntersectionComponent>(index, entity);
            CommandBuffer.RemoveComponent<ReachedEndOfSplineComponent>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle handle)
    {
        var splineBuffer = EntityManager.GetBuffer<SplineBufferElementData>(GetSingletonEntity<SplineBufferElementData>());

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