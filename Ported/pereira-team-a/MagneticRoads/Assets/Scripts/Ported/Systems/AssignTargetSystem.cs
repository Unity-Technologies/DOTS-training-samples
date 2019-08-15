using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class AssignTargetSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    struct AssignDestinationJob : IJobForEachWithEntity<FindTarget, SplineData>
    {
        [ReadOnly] public DynamicBuffer<IntersectionPoint> intersectionBuffer;
        [ReadOnly] public DynamicBuffer<Spline> splineBuffer;
        public float deltaTime;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public unsafe void Execute(Entity entity, int index, [ReadOnly] ref FindTarget findTarget,
            ref SplineData currentSplineData)
        {
            // If we were moving along an intersection, continue to the target spline
            if (currentSplineData.Spline.IsIntersection)
            {
                var spline = splineBuffer[currentSplineData.Spline.TargetSplineId];
                var newSplineData = new SplineData
                {
                    //StartPosition = splineData.TargetPosition,
                    //TargetPosition = intersectionBuffer[spline.EndIntersectionId].Position,
                    Spline = spline
                };
                CommandBuffer.SetComponent(index, entity, newSplineData);
            }
            else
            {
                // Get the intersection object, to which we have arrived
                var currentIntersection = intersectionBuffer[currentSplineData.Spline.EndIntersectionId];
                
                // Select the next spline to travel
                currentIntersection.LastIntersection += 1;
                intersectionBuffer[currentSplineData.Spline.EndIntersectionId] = currentIntersection;
            
                var targetSplineIndex = currentIntersection.LastIntersection % currentIntersection.SplineIdCount;
                var targetSplineId = 0;
                if (targetSplineIndex == 0)
                    targetSplineId = currentIntersection.SplineId0; // 0
                else if (targetSplineIndex == 1)
                    targetSplineId = currentIntersection.SplineId1; // 1
                else if (targetSplineIndex == 2)
                    targetSplineId = currentIntersection.SplineId2; // 2

                var targetSpline = splineBuffer[targetSplineId];
                
                var newSpline = new Spline()
                {
                    TargetSplineId = targetSplineId,
                    IsIntersection = true,
                    
                    EndIntersectionId = currentSplineData.Spline.EndIntersectionId,  // For this temporary spline, the end intersection is this same one
                    
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
                var newSplineData = new SplineData
                {
                    Spline = newSpline
                };
                
                CommandBuffer.SetComponent(index, entity, newSplineData);
            }
            
            CommandBuffer.RemoveComponent<FindTarget>(index, entity);
        }
    }

    protected override JobHandle OnUpdate(JobHandle handle)
    {
        var intersectionBuffer = EntityManager.GetBuffer<IntersectionPoint>(GetSingletonEntity<IntersectionPoint>());
        var splineBuffer = EntityManager.GetBuffer<Spline>(GetSingletonEntity<Spline>());

        var job = new AssignDestinationJob
        {
            intersectionBuffer = intersectionBuffer,
            splineBuffer = splineBuffer,
            deltaTime = Time.time,
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(this, handle);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}