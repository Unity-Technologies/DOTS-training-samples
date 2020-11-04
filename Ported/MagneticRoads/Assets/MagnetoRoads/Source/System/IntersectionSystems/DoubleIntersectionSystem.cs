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
                doubleIntersection.car0 = UpdateCarLane(doubleIntersection.laneIn0, doubleIntersection.laneOut1, doubleIntersection.car0);
                doubleIntersection.car1 = UpdateCarLane(doubleIntersection.laneIn1, doubleIntersection.laneOut0, doubleIntersection.car1);
            }).Run();

        ecb.Playback(EntityManager);
    }

    Entity UpdateCarLane(Entity laneIn, Entity laneOut, Entity car)
    {
        if (car == Entity.Null)
        {
            Lane lane = GetComponent<Lane>(laneIn);
            DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(laneIn);
            if(!cars.IsEmpty)
            {
                Entity laneCar = cars[0];
                CarPosition carPosition = GetComponent<CarPosition>(laneCar);
                if (carPosition.Value == lane.Length)
                {
                    var prevCar = laneCar;
                    cars.RemoveAt(0);
                    return prevCar;
                }
            }
        }
        else
        {
            DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(laneOut);
            cars.Add(car);
            SetComponent(car, new CarPosition{Value = 0});
        }
        
        return Entity.Null;
    }
}
