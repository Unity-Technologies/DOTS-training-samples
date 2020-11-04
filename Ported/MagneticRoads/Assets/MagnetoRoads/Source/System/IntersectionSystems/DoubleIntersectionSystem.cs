using Unity.Collections;
using Unity.Entities;

[UpdateAfter(typeof(SimpleIntersectionSystem))]
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
        Lane lane = GetComponent<Lane>(laneIn);
        DynamicBuffer<MyBufferElement> laneInCars = GetBuffer<MyBufferElement>(laneIn);
        Entity reachedEndOfLaneCar = Entity.Null;
        if(!laneInCars.IsEmpty){
            Entity laneCar = laneInCars[0];
            CarPosition carPosition = GetComponent<CarPosition>(laneCar);
            CarSpeed carSpeed = GetComponent<CarSpeed>(laneCar);
                
            carSpeed.NormalizedValue += deltaTime * CarSpeed.ACCELERATION;
            if (carSpeed.NormalizedValue > 1.0f){
                carSpeed.NormalizedValue = 1.0f;    
            }
	                
            float newPosition = carPosition.Value + carSpeed.NormalizedValue * CarSpeed.MAX_SPEED * deltaTime;
            
            if(newPosition > lane.Length){
                reachedEndOfLaneCar = laneCar;
                newPosition = lane.Length;
            }
            SetComponent(laneCar, new CarPosition{Value = newPosition});
            SetComponent(laneCar, carSpeed);
        }

        if (car == Entity.Null){
            if (reachedEndOfLaneCar != Entity.Null){
                laneInCars.RemoveAt(0);
                return reachedEndOfLaneCar;    
            }
        }
        else{
            // TODO: MBRIAU: Still make that car accelerate but cap the normalized speed to 0.7f while in an intersection (Look at Car.cs)
            // TODO: MBRIAU: We also need to make the first car of each input lane slowdown since the intersection is busy
            DynamicBuffer<MyBufferElement> laneOutCars = GetBuffer<MyBufferElement>(laneOut);
            laneOutCars.Add(car);
            SetComponent(car, new CarPosition{Value = 0});
        }
        
        return Entity.Null;
    }
}
