using Unity.Entities;
using Unity.Transforms;

// For traffic that must do a U-turn at a dead end road
// laneOut0 <--|
// laneIn0  ---|
[UpdateAfter(typeof(TrafficSpawnerSystem))]
public class SimpleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection, in Spline spline) =>
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
                        SetComponent(reachedEndOfLaneCar, new CarPosition {Value = 0});
                    }
                }
                // Car in intersection handled by SimpleIntersectionActiveCarSystem
                // Car finished in intersection and added to out lane handled by SimpleIntersectionOutSystem
            }).Schedule();
    }
}
