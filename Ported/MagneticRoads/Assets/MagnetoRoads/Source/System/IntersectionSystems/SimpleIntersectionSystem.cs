using Unity.Collections;
using Unity.Entities;

public class SimpleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float elapsedTime = Time.fixedDeltaTime;

        Entities
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection) =>
            {
                if (simpleIntersection.car == Entity.Null)
                {
                    Lane lane = GetComponent<Lane>(simpleIntersection.laneIn0);
                    DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(simpleIntersection.laneIn0);
                    if (!cars.IsEmpty)
                    {
                        Entity laneCar = cars[0];
                        CarPosition carPosition = GetComponent<CarPosition>(laneCar);
                        if (carPosition.Value == lane.Length)
                        {
                            simpleIntersection.car = laneCar;
                            cars.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(simpleIntersection.laneOut0);
                    cars.Add(simpleIntersection.car);
                    ecb.SetComponent(simpleIntersection.car, new CarPosition{Value = 0});
                    simpleIntersection.car = Entity.Null;
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
