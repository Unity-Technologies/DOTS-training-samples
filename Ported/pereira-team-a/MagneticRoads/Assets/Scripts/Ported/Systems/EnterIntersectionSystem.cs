using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class EnterIntersectionSystem : JobComponentSystem
{
    private EntityQuery m_Query;
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadOnly<ReachedEndOfSplineComponent>(), ComponentType.ReadOnly<SplineComponent>()},
            None = new []{ComponentType.ReadOnly<ExitIntersectionComponent>() }
        });
        
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct AssignDestinationJob : IJobForEachWithEntity<ReachedEndOfSplineComponent, SplineComponent>
    {
        [ReadOnly] public DynamicBuffer<IntersectionBufferElementData> IntersectionBuffer;
        [ReadOnly] public DynamicBuffer<SplineBufferElementData> SplineBuffer;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public unsafe void Execute(Entity entity, int index, [ReadOnly] ref ReachedEndOfSplineComponent reachedEndOfSplineComponent,
            [ReadOnly] ref SplineComponent currentSplineComponent)
        {
            // Get the intersection object, to which we have arrived
            var currentIntersection = IntersectionBuffer[currentSplineComponent.SplineBufferElementData.EndIntersectionId];
            
            // Select the next spline to travel
            currentIntersection.LastIntersection += 1;
            IntersectionBuffer[currentSplineComponent.SplineBufferElementData.EndIntersectionId] = currentIntersection;
        
            var targetSplineIndex = currentIntersection.LastIntersection % currentIntersection.SplineIdCount;
            var targetSplineId = 0;
            if (targetSplineIndex == 0)
                targetSplineId = currentIntersection.SplineId0; // 0
            else if (targetSplineIndex == 1)
                targetSplineId = currentIntersection.SplineId1; // 1
            else if (targetSplineIndex == 2)
                targetSplineId = currentIntersection.SplineId2; // 2

            var targetSpline = SplineBuffer[targetSplineId];
            
            var newSpline = new SplineBufferElementData()
            {
                EndIntersectionId = -1,
                
                StartPosition = currentSplineComponent.SplineBufferElementData.EndPosition,
                EndPosition = targetSpline.StartPosition,
                
                //intersectionSpline.anchor1 = (intersection.position + intersectionSpline.startPoint) * .5f;
                Anchor1 = currentSplineComponent.SplineBufferElementData.EndPosition,//(currentIntersection.Position + currentSplineData.Spline.EndPosition) * .5f,
                // intersectionSpline.anchor2 = (intersection.position + intersectionSpline.endPoint) * .5f;
                Anchor2 = targetSpline.StartPosition,//(currentIntersection.Position + targetSpline.StartPosition) * .5f,
                // intersectionSpline.startTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.startPoint).normalized);
                StartTangent = math.round(math.normalize(currentIntersection.Position - currentSplineComponent.SplineBufferElementData.EndPosition)),
                // intersectionSpline.endTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.endPoint).normalized);
                EndTangent = math.round(math.normalize(currentIntersection.Position - targetSpline.StartPosition)),
                // intersectionSpline.startNormal = intersection.normal;
                StartNormal = currentSplineComponent.SplineBufferElementData.EndNormal,
                // intersectionSpline.endNormal = intersection.normal;
                EndNormal = targetSpline.EndNormal
            };
            
            CommandBuffer.SetComponent(index, entity, new SplineComponent{SplineBufferElementData = newSpline, IsInsideIntersection = true});
            CommandBuffer.AddComponent(index, entity, new ExitIntersectionComponent {TargetSplineId = targetSplineId});
            CommandBuffer.RemoveComponent<ReachedEndOfSplineComponent>(index, entity);
            CommandBuffer.AddComponent<InterpolatorTComponent>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var intersectionBuffer = EntityManager.GetBuffer<IntersectionBufferElementData>(GetSingletonEntity<IntersectionBufferElementData>());
        var splineBuffer = EntityManager.GetBuffer<SplineBufferElementData>(GetSingletonEntity<SplineBufferElementData>());

        var job = new AssignDestinationJob
        {
            IntersectionBuffer = intersectionBuffer,
            SplineBuffer = splineBuffer,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(m_Query, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}