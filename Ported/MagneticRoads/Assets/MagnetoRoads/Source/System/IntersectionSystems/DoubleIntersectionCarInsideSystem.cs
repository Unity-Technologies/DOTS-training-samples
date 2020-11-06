using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
// For traffic that can only go straight through the intersection
// car1: laneOut0 <-- laneIn1
// car0: laneIn0   --> laneOut1
[UpdateAfter(typeof(DoubleIntersectionSystem))]
public class DoubleIntersectionCarInsideSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        ecbSystem = World.GetExistingSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        var ecb = ecbSystem.CreateCommandBuffer();
        var ecbWriter = ecb.AsParallelWriter();

        Entities
            .WithNone<IntersectionNeedsInit>()
            .ForEach((Entity entity, int entityInQueryIndex, ref DoubleIntersection doubleIntersection) =>
            {
                if (doubleIntersection.car0 != Entity.Null)
                {
                    var carPos = GetComponent<CarPosition>(doubleIntersection.car0);
                    //Debug.Log("Car at Intersection 0 " + carPos.Value + " entity: " + doubleIntersection.car0);
                    if (carPos.Value < 1)
                    {
                        var carSpeed = GetComponent<CarSpeed>(doubleIntersection.car0);
                        float newPosition = carPos.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                        ecbWriter.SetComponent(entityInQueryIndex, doubleIntersection.car0, new CarPosition {Value = newPosition});
                        var splineData = GetComponent<Spline>(doubleIntersection.spline0);
                        var eval = BezierUtility.EvaluateBezier(splineData.startPos, splineData.anchor1, splineData.anchor2,
                            splineData.endPos, newPosition);
                        ecbWriter.SetComponent(entityInQueryIndex, doubleIntersection.car0, new Translation {Value = eval});
                    }
                }

                if (doubleIntersection.car1 != Entity.Null)
                {
                    var carPos = GetComponent<CarPosition>(doubleIntersection.car1);
                    if (carPos.Value < 1)
                    {
                        var carSpeed = GetComponent<CarSpeed>(doubleIntersection.car1);
                        float newPosition = carPos.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                        ecbWriter.SetComponent(entityInQueryIndex, doubleIntersection.car1, new CarPosition {Value = newPosition});
                        var splineData = GetComponent<Spline>(doubleIntersection.spline1);
                        var eval = BezierUtility.EvaluateBezier(splineData.startPos, splineData.anchor1, splineData.anchor2,
                            splineData.endPos, newPosition);
                        ecbWriter.SetComponent(entityInQueryIndex, doubleIntersection.car1, new Translation {Value = eval});
                    }
                }

            }).Schedule();
        
        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}