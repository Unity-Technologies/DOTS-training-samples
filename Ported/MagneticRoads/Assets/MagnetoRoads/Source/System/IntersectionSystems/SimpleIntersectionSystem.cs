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
                else
                {
                    var carPosition = GetComponent<CarPosition>(simpleIntersection.car);
                    if (carPosition.Value < 1)
                    {
                        var carSpeed = GetComponent<CarSpeed>(simpleIntersection.car);
                        float newPosition = carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
                        SetComponent(simpleIntersection.car, new CarPosition {Value = newPosition});
                        var eval = BezierUtility.EvaluateBezier(spline.startPos, spline.anchor1, spline.anchor2, spline.endPos, newPosition);
                        SetComponent(simpleIntersection.car, new Translation {Value = eval});
                    }
                    else
                    {
                        // TODO: MBRIAU: Still make that car accelerate but cap the normalized speed to 0.7f while in an intersection (Look at Car.cs)
                        // TODO: MBRIAU: We also need to make the first car of each input lane slowdown since the intersection is busy

                        DynamicBuffer<CarBufferElement> laneOutCars =
                            GetBuffer<CarBufferElement>(simpleIntersection.laneOut0);
                        laneOutCars.Add(simpleIntersection.car);
                        SetComponent(simpleIntersection.car, new CarPosition {Value = 0});
                        simpleIntersection.car = Entity.Null;
                    }
                }
            }).Schedule();
    }
}
