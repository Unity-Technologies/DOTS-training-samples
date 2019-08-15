using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class AssignIntersectionSpline : JobComponentSystem
{
    private EntityQuery m_Query;
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{ComponentType.ReadOnly<ReachedEndOfSpline>(), ComponentType.ReadOnly<SplineData>()},
            None = new []{ComponentType.ReadOnly<ExitIntersectionData>() }
        });
        
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct AssignDestinationJob : IJobForEachWithEntity<ReachedEndOfSpline, SplineData>
    {
        [ReadOnly] public DynamicBuffer<IntersectionPoint> IntersectionBuffer;
        [ReadOnly] public DynamicBuffer<Spline> SplineBuffer;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public unsafe void Execute(Entity entity, int index, [ReadOnly] ref ReachedEndOfSpline reachedEndOfSpline,
            [ReadOnly] ref SplineData currentSplineData)
        {
            // Get the intersection object, to which we have arrived
            var currentIntersection = IntersectionBuffer[currentSplineData.Spline.EndIntersectionId];
            
            // Select the next spline to travel
            currentIntersection.LastIntersection += 1;
            IntersectionBuffer[currentSplineData.Spline.EndIntersectionId] = currentIntersection;
        
            var targetSplineIndex = currentIntersection.LastIntersection % currentIntersection.SplineIdCount;
            var targetSplineId = 0;
            if (targetSplineIndex == 0)
                targetSplineId = currentIntersection.SplineId0; // 0
            else if (targetSplineIndex == 1)
                targetSplineId = currentIntersection.SplineId1; // 1
            else if (targetSplineIndex == 2)
                targetSplineId = currentIntersection.SplineId2; // 2

            var targetSpline = SplineBuffer[targetSplineId];
            
            var newSpline = new Spline()
            {
                EndIntersectionId = -1,
                
                StartPosition = currentSplineData.Spline.EndPosition,
                EndPosition = targetSpline.StartPosition,
                
                //intersectionSpline.anchor1 = (intersection.position + intersectionSpline.startPoint) * .5f;
                Anchor1 = (currentIntersection.Position + currentSplineData.Spline.EndPosition) * .5f,
                // intersectionSpline.anchor2 = (intersection.position + intersectionSpline.endPoint) * .5f;
                Anchor2 = (currentIntersection.Position + targetSpline.StartPosition) * .5f,
                // intersectionSpline.startTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.startPoint).normalized);
                StartTangent = math.round(math.normalize(currentIntersection.Position - currentSplineData.Spline.EndPosition)),
                // intersectionSpline.endTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.endPoint).normalized);
                EndTangent = math.round(math.normalize(currentIntersection.Position - targetSpline.StartPosition)),
                // intersectionSpline.startNormal = intersection.normal;
                StartNormal = currentSplineData.Spline.EndNormal,
                // intersectionSpline.endNormal = intersection.normal;
                EndNormal = targetSpline.EndNormal
            };
            
            CommandBuffer.SetComponent(index, entity, new SplineData{Spline = newSpline});
            CommandBuffer.AddComponent(index, entity, new ExitIntersectionData {TargetSplineId = targetSplineId});
            CommandBuffer.RemoveComponent<ReachedEndOfSpline>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var intersectionBuffer = EntityManager.GetBuffer<IntersectionPoint>(GetSingletonEntity<IntersectionPoint>());
        var splineBuffer = EntityManager.GetBuffer<Spline>(GetSingletonEntity<Spline>());

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