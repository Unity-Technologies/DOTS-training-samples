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
            var currentIntersection = IntersectionBuffer[currentSplineComponent.Spline.EndIntersectionId];
            
            // Select the next spline to travel
            // TODO: Replace this with a noise/random solution
            currentIntersection.LastIntersection += 1;
            IntersectionBuffer[currentSplineComponent.Spline.EndIntersectionId] = currentIntersection;
            
            int[] targetSplineIDs = {
                currentIntersection.SplineId0,
                currentIntersection.SplineId1,
                currentIntersection.SplineId2
            };
            var targetSplineId = targetSplineIDs[currentIntersection.LastIntersection % currentIntersection.SplineIdCount];
            var targetSpline = SplineBuffer[targetSplineId];
            
            var newSpline = new SplineBufferElementData()
            {
                EndIntersectionId = -1,
                SplineId = 1,
                
                StartPosition = currentSplineComponent.Spline.EndPosition,
                EndPosition = targetSpline.StartPosition,
                
                //intersectionSpline.anchor1 = (intersection.position + intersectionSpline.startPoint) * .5f;
                Anchor1 = (currentIntersection.Position + targetSpline.StartPosition) * .5f,
                // intersectionSpline.anchor2 = (intersection.position + intersectionSpline.endPoint) * .5f;
                Anchor2 = (currentIntersection.Position + targetSpline.EndPosition) * .5f,
                // intersectionSpline.startTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.startPoint).normalized);
                StartTangent = math.round(math.normalize(currentIntersection.Position - currentSplineComponent.Spline.EndPosition)),
                // intersectionSpline.endTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.endPoint).normalized);
                EndTangent = math.round(math.normalize(currentIntersection.Position - targetSpline.StartPosition)),
                // intersectionSpline.startNormal = intersection.normal;
                StartNormal = currentIntersection.Normal,
                // intersectionSpline.endNormal = intersection.normal;
                EndNormal = IntersectionBuffer[targetSpline.EndIntersectionId].Normal
            };
            /*
            if (targetSpline.OppositeDirectionSplineId == currentSplineComponent.Spline.SplineId)
            {
                // u-turn - make our intersection spline more rounded than usual
                Vector3 perp = Vector3.Cross(intersectionSpline.startTangent, intersectionSpline.startNormal);
                intersectionSpline.anchor1 +=
                    (Vector3) intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;
                intersectionSpline.anchor2 +=
                    (Vector3) intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;

                intersectionSpline.anchor1 -= perp * RoadGenerator.trackRadius * .5f * intersectionSide;
                intersectionSpline.anchor2 += perp * RoadGenerator.trackRadius * .5f * intersectionSide;
            }
            */
            

            CommandBuffer.SetComponent(index, entity, new SplineComponent { Spline = newSpline, IsInsideIntersection = true, t = 0 });
            CommandBuffer.AddComponent(index, entity, new ExitIntersectionComponent {TargetSplineId = targetSplineId});
            CommandBuffer.RemoveComponent<ReachedEndOfSplineComponent>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!RoadGenerator.ready || !RoadGenerator.useECS)
            return inputDeps;
        
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