using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class MovementSystem : JobComponentSystem
{
    private EntityQuery query;
    private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        query = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadWrite<LocalToWorld>(), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Rotation>(), ComponentType.ReadWrite<SplineComponent>() },
            //None = new[] { ComponentType.ReadOnly<ReachedEndOfSplineComponent>() }
        });
        
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    struct MoveJob : IJobForEachWithEntity<LocalToWorld, Translation, Rotation, SplineComponent>
    {
        public float deltaTime;
        
        [ReadOnly] public DynamicBuffer<IntersectionBufferElementData> IntersectionBuffer;
        [ReadOnly] public DynamicBuffer<SplineBufferElementData> SplineBuffer;
        //public EntityCommandBuffer.Concurrent CommandBuffer;
        
        public void Execute(Entity entity, int index, ref LocalToWorld localToWorld, ref Translation translation, ref Rotation rotation, ref SplineComponent splineComponent)
        {
            // MOVE ------
            //velocity based on if we are in an intersection or not
            float velocity = splineComponent.IsInsideIntersection ? 1f : 2.0f;

            //here we calculate the t
            float dist = math.distance(splineComponent.Spline.EndPosition, splineComponent.Spline.StartPosition);
            var moveDisplacement = (deltaTime * velocity) / dist;
            var t = math.clamp(splineComponent.t + moveDisplacement, 0, 1);

            //If we are inside a tempSpline, just interpolate to pass thorugh it
            if (splineComponent.IsInsideIntersection)
            {
                translation.Value = translation.Value + math.normalize(splineComponent.Spline.EndPosition - translation.Value) * deltaTime * velocity;
                //rotation.Value = math.slerp(rotation.Value, quaternion.LookRotationSafe(trackSplineComponent.Spline.EndPosition - translation.Value, trackSplineComponent.Spline.StartNormal), trackSplineComponent.t);
                rotation.Value = quaternion.LookRotationSafe(
                    splineComponent.Spline.EndPosition - translation.Value,
                    splineComponent.Spline.StartNormal);

                localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(
                        translation.Value,
                        rotation.Value,
                        new float3(1.0f, 1.0f, 1.0f))
                };
                splineComponent.t += deltaTime * velocity;
            }
            else
            {
                //if not, process to the full movement across the spline
                float3 pos = EvaluateCurve(splineComponent.t, splineComponent);

                //Calculating up vector
                quaternion fromTo = Quaternion.FromToRotation(splineComponent.Spline.StartNormal, splineComponent.Spline.EndNormal);
                float smoothT = math.smoothstep(0f, 1f, t * 1.02f - .01f);
                float3 up = math.mul(math.slerp(quaternion.identity, fromTo, smoothT), splineComponent.Spline.StartNormal);

                translation.Value = pos + math.normalize(up) * 0.015f;
                rotation.Value =
                    quaternion.LookRotationSafe(splineComponent.Spline.EndPosition - translation.Value, up);

                localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(
                        translation.Value,
                        rotation.Value,
                        new float3(1.0f, 1.0f, 1.0f))
                };

                splineComponent.t = t;
            }
            
            // CHECK IF FINISHED
            bool hasReachedTarget = math.distancesq(translation.Value, splineComponent.Spline.EndPosition) < 0.001f;
            if (hasReachedTarget)
            {
                // If we are exiting an intersection
                if (splineComponent.IsInsideIntersection)
                {
                    var spline = SplineBuffer[splineComponent.TargetSplineId];
                    var newSplineComponent = new SplineComponent
                    {
                        splineId = splineComponent.TargetSplineId,
                        Spline = spline, 
                        IsInsideIntersection = false,
                        t = 0,
                        reachEndOfSpline = false
                    };

                    splineComponent.splineId = splineComponent.TargetSplineId;
                    splineComponent.Spline = spline;
                    splineComponent.IsInsideIntersection = false;
                    splineComponent.t = 0;
                    splineComponent.reachEndOfSpline = false;

                    //CommandBuffer.SetComponent(index, entity, newSplineComponent);
                }
                else
                {
                    // If we are exiting a road
                    
                    // Get the intersection object, to which we have arrived
                    var currentIntersection = IntersectionBuffer[splineComponent.Spline.EndIntersectionId];
                    
                    // Select the next spline to travel
                    // TODO: Replace this with a noise/random solution
                    currentIntersection.LastIntersection += 1;
                    IntersectionBuffer[splineComponent.Spline.EndIntersectionId] = currentIntersection;

                    int nextIntersection = currentIntersection.LastIntersection % currentIntersection.SplineIdCount;
                    var targetSplineId = 0;
                    if (nextIntersection == 0)
                        targetSplineId = currentIntersection.SplineId0;
                    else if (nextIntersection == 1)
                        targetSplineId = currentIntersection.SplineId1;
                    else if (nextIntersection == 2)
                        targetSplineId = currentIntersection.SplineId2;
                    
                    var targetSpline = SplineBuffer[targetSplineId];

                    var newSpline = new SplineBufferElementData()
                    {
                        EndIntersectionId = -1,
                        SplineId = 1,

                        StartPosition = splineComponent.Spline.EndPosition,
                        EndPosition = targetSpline.StartPosition,

                        //intersectionSpline.anchor1 = (intersection.position + intersectionSpline.startPoint) * .5f;
                        Anchor1 = (currentIntersection.Position + targetSpline.StartPosition) * .5f,
                        // intersectionSpline.anchor2 = (intersection.position + intersectionSpline.endPoint) * .5f;
                        Anchor2 = (currentIntersection.Position + targetSpline.EndPosition) * .5f,
                        // intersectionSpline.startTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.startPoint).normalized);
                        StartTangent =
                            math.round(math.normalize(currentIntersection.Position -
                                                      splineComponent.Spline.EndPosition)),
                        // intersectionSpline.endTangent = Vector3Int.RoundToInt((intersection.position - intersectionSpline.endPoint).normalized);
                        EndTangent = math.round(math.normalize(currentIntersection.Position - targetSpline.StartPosition)),
                        // intersectionSpline.startNormal = intersection.normal;
                        StartNormal = currentIntersection.Normal,
                        // intersectionSpline.endNormal = intersection.normal;
                        EndNormal = IntersectionBuffer[targetSpline.EndIntersectionId].Normal
                    };
                    
                    //if (targetSpline.OppositeDirectionSplineId == currentSplineComponent.Spline.SplineId)
                    {
                        newSpline.Anchor1 = newSpline.StartPosition;
                        newSpline.Anchor2 = newSpline.EndPosition;
                        /*
                        // u-turn - make our intersection spline more rounded than usual
                        Vector3 perp = Vector3.Cross(intersectionSpline.startTangent, intersectionSpline.startNormal);
                        intersectionSpline.anchor1 +=
                            (Vector3) intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;
                        intersectionSpline.anchor2 +=
                            (Vector3) intersectionSpline.startTangent * RoadGenerator.intersectionSize * .5f;

                        intersectionSpline.anchor1 -= perp * RoadGenerator.trackRadius * .5f * intersectionSide;
                        intersectionSpline.anchor2 += perp * RoadGenerator.trackRadius * .5f * intersectionSide;*/
                    }
                    
                    splineComponent.splineId = targetSplineId;
                    splineComponent.Spline = newSpline;
                    splineComponent.IsInsideIntersection = true;
                    splineComponent.t = 0;
                    splineComponent.reachEndOfSpline = false;
                    
                    /*CommandBuffer.SetComponent(index, entity, new SplineComponent
                    {
                        Spline = newSpline,
                        IsInsideIntersection = true,
                        t = 0,
                        TargetSplineId = targetSplineId,
                        reachEndOfSpline = false
                    });*/
                }
            }
        }
    }

    public static float3 EvaluateCurve(float t, SplineComponent splineComponent)
    {
        t = math.clamp(t, 0, 1);
        return splineComponent.Spline.StartPosition * (1f - t) * (1f - t) * (1f - t) + 3f * splineComponent.Spline.Anchor1 * (1f - t) * (1f - t) * t + 3f * splineComponent.Spline.Anchor2 * (1f - t) * t * t + splineComponent.Spline.EndPosition * t * t * t;
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!RoadGenerator.ready || !RoadGenerator.useECS)
            return inputDeps;

        var splineBuffer = EntityManager.GetBuffer<SplineBufferElementData>(GetSingletonEntity<SplineBufferElementData>());
        var intersectionBuffer = EntityManager.GetBuffer<IntersectionBufferElementData>(GetSingletonEntity<IntersectionBufferElementData>());
        
        var job = new MoveJob
        {
            deltaTime = Time.deltaTime,
            SplineBuffer = splineBuffer,
            IntersectionBuffer = intersectionBuffer,
            //CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        
        var jobHandle = job.Schedule(this, inputDeps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}