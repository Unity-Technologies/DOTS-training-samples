using Unity.Collections;
using Unity.Entities;

public class DoubleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst()
            .ForEach((Entity entity, ref DoubleIntersection doubleIntersection) =>
            {
                doubleIntersection.car0 = UpdateCarLane(doubleIntersection.laneIn0, doubleIntersection.laneOut0, doubleIntersection.car0);
                doubleIntersection.car1 = UpdateCarLane(doubleIntersection.laneIn1, doubleIntersection.laneOut1, doubleIntersection.car1);
            }).Run();

        ecb.Playback(EntityManager);
    }

    Entity UpdateCarLane(Entity laneIn, Entity laneOut, Entity car)
    {
        if (car == Entity.Null)
        {
            Lane lane = GetComponent<Lane>(laneIn);
            if (lane.Car != Entity.Null)
            {
                CarPosition carPosition = GetComponent<CarPosition>(lane.Car);
                if (carPosition.Value == lane.Length)
                {
                    var prevCar = lane.Car;
                    lane.Car = Entity.Null;
                    SetComponent(laneIn, lane);
                    return prevCar;
                }
            }
        }
        else
        {
            Lane newLaneOut = GetComponent<Lane>(laneOut);
            newLaneOut.Car = car;
            SetComponent(laneOut, newLaneOut);
            SetComponent(car, new CarPosition{Value = 0});
        }
        
        return Entity.Null;
    }
}
