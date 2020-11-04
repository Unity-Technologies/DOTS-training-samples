using Unity.Collections;
using Unity.Entities;


[UpdateAfter(typeof(TrafficSpawnerSystem))]
public class SimpleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection) =>
            {
                Lane lane = GetComponent<Lane>(simpleIntersection.laneIn0);
                DynamicBuffer<CarBufferElement> laneInCars = GetBuffer<CarBufferElement>(simpleIntersection.laneIn0);
                Entity reachedEndOfLaneCar = Entity.Null;
                if (!laneInCars.IsEmpty)
                {
                    Entity laneCar = laneInCars[0];
                    CarPosition carPosition = GetComponent<CarPosition>(laneCar);
                    CarSpeed carSpeed = GetComponent<CarSpeed>(laneCar);
                        
                    carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
                    if (carSpeed.NormalizedValue > 1.0f){
                        carSpeed.NormalizedValue = 1.0f;    
                    }
	                
                    float newPosition = carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                        
                    if(newPosition > lane.Length)
                    {
                        reachedEndOfLaneCar = laneCar;
                        newPosition = lane.Length;
                    }
                    SetComponent(laneCar, new CarPosition{Value = newPosition});
                    SetComponent(laneCar, carSpeed);
                }
                
                if (simpleIntersection.car == Entity.Null)
                {
                    if (reachedEndOfLaneCar != Entity.Null)
                    {
                        simpleIntersection.car = reachedEndOfLaneCar;
                        laneInCars.RemoveAt(0);
                    }
                }
                else 
                {
                    // TODO: MBRIAU: Still make that car accelerate but cap the normalized speed to 0.7f while in an intersection (Look at Car.cs)
                    // TODO: MBRIAU: We also need to make the first car of each input lane slowdown since the intersection is busy
                    
                    DynamicBuffer<CarBufferElement> laneOutCars = GetBuffer<CarBufferElement>(simpleIntersection.laneOut0);
                    laneOutCars.Add(simpleIntersection.car);
                    SetComponent(simpleIntersection.car, new CarPosition{Value = 0});
                    simpleIntersection.car = Entity.Null;
                }
            }).Schedule();

    }
}
