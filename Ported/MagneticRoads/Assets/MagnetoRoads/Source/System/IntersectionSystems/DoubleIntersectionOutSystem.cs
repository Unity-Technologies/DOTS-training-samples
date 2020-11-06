using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
// For traffic that can only go straight through the intersection
// car1: laneOut0 <-- laneIn1
// car0: laneIn0   --> laneOut1
[UpdateAfter(typeof(DoubleIntersectionCarInsideSystem))]
public class DoubleIntersectionOutSystem : SystemBase
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
                    if (carPos.Value >= 1)
                    {
                        CarBufferElement carBufferElement = doubleIntersection.car0;
                        ecbWriter.AppendToBuffer(entityInQueryIndex, doubleIntersection.laneOut1, carBufferElement);
                        
                        doubleIntersection.car0 = Entity.Null;
                    }
                }

                if (doubleIntersection.car1 != Entity.Null)
                {
                    var carPos = GetComponent<CarPosition>(doubleIntersection.car1);
                    if (carPos.Value >= 1)
                    {
                        CarBufferElement carBufferElement = doubleIntersection.car1;
                        ecbWriter.AppendToBuffer(entityInQueryIndex, doubleIntersection.laneOut0, carBufferElement);
                        
                        doubleIntersection.car1 = Entity.Null;
                    }
                }

            }).ScheduleParallel();
        
        ecbSystem.AddJobHandleForProducer(Dependency);
    }
}