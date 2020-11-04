using Unity.Collections;
using Unity.Entities;

public class SimpleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float deltaTime = Time.DeltaTime;

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
                        CarSpeed carSpeed = GetComponent<CarSpeed>(laneCar);
                        
                        carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
                        if (carSpeed.NormalizedValue > 1.0f){
                            carSpeed.NormalizedValue = 1.0f;    
                        }
	                
                        float newPosition = carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                        
                        if(newPosition > lane.Length)
                        {
                            newPosition = lane.Length;
                            simpleIntersection.car = laneCar;
                            cars.RemoveAt(0);
                        }
                        SetComponent(laneCar, new CarPosition{Value = newPosition});
                        SetComponent(laneCar, carSpeed);
                    }
                }
                else
                {
                    // TODO: MBRIAU: Still make that car accelerate but cap the normalized speed to 0.7f while in an intersection (Look at Car.cs)
                    // TODO: MBRIAU: We also need to make the first car of each input lane slowdown since the intersection is busy
                    
                    DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(simpleIntersection.laneOut0);
                    cars.Add(simpleIntersection.car);
                    ecb.SetComponent(simpleIntersection.car, new CarPosition{Value = 0});
                    simpleIntersection.car = Entity.Null;
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
