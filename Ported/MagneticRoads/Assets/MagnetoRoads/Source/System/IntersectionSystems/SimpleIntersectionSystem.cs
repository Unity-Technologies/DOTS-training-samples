using Unity.Collections;
using Unity.Entities;

public class SimpleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float elapsedTime = Time.fixedDeltaTime;

        Entities.WithoutBurst()
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection) =>
            {
                if (simpleIntersection.car == Entity.Null)
                {
                    Lane lane = GetComponent<Lane>(simpleIntersection.laneIn0);
                    if (lane.Car != Entity.Null)
                    {
                        CarPosition carPosition = GetComponent<CarPosition>(lane.Car);
                        if (carPosition.Value == lane.Length)
                        {
                            simpleIntersection.car = lane.Car;
                            lane.Car = Entity.Null;
                            ecb.SetComponent(simpleIntersection.laneIn0, lane);
                        }
                    }
                }
                else
                {
                    Lane laneOut = GetComponent<Lane>(simpleIntersection.laneOut0);
                    laneOut.Car = simpleIntersection.car;
                    ecb.SetComponent(simpleIntersection.laneOut0, laneOut);
                    ecb.SetComponent(simpleIntersection.car, new CarPosition{Value = 0});
                    simpleIntersection.car = Entity.Null;
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
