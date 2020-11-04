using Unity.Collections;
using Unity.Entities;

public class DoubleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float deltaTime = Time.DeltaTime;

        Entities.WithoutBurst()
            .ForEach((Entity entity, ref DoubleIntersection doubleIntersection) =>
            {
                doubleIntersection.car0 = UpdateCarLane(doubleIntersection.laneIn0, doubleIntersection.laneOut1, doubleIntersection.car0, deltaTime);
                doubleIntersection.car1 = UpdateCarLane(doubleIntersection.laneIn1, doubleIntersection.laneOut0, doubleIntersection.car1, deltaTime);
            }).Run();

        ecb.Playback(EntityManager);
    }

    Entity UpdateCarLane(Entity laneIn, Entity laneOut, Entity car, float deltaTime)
    {
        if (car == Entity.Null)
        {
            Lane lane = GetComponent<Lane>(laneIn);
            DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(laneIn);
            if(!cars.IsEmpty)
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
                
                    var prevCar = laneCar;
                    cars.RemoveAt(0);
                    return prevCar;
                }
                SetComponent(laneCar, new CarPosition{Value = newPosition});
                SetComponent(laneCar, carSpeed);
            }
        }
        else
        {
            // TODO: MBRIAU: Still make that car accelerate but cap the normalized speed to 0.7f while in an intersection (Look at Car.cs)
            // TODO: MBRIAU: We also need to make the first car of each input lane slowdown since the intersection is busy
            
            DynamicBuffer<MyBufferElement> cars = GetBuffer<MyBufferElement>(laneOut);
            cars.Add(car);
            SetComponent(car, new CarPosition{Value = 0});
        }
        
        return Entity.Null;
    }
}
