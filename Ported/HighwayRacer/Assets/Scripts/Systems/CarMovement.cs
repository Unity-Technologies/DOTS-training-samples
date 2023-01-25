using Aspects;
using Authoring;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct CarMovement : ISystem
{
    EntityQuery carQuery;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        carQuery = SystemAPI.QueryBuilder().WithAll<Car>().Build();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        Unity.Collections.NativeArray<Car> allCars = carQuery.ToComponentDataArray<Car>(Allocator.Temp);

        foreach (var car in SystemAPI.Query<CarAspect>())
        {
            // if (car.Distance > 100.0)
            // {
            //     car.Distance = 0.0f;
            // }
            //get car in front of us
            Car neighbor = allCars[0];//assume there is a car and that we will have one close to us
            float neighborDelta = 1000.0f;
            foreach (var other in allCars)
            {
                if(other.Index == car.Index) { continue; }
                if(other.LaneNumber != car.LaneNumber) { continue; }

                float delta = other.Distance - car.Distance;
                if(delta >= 0.0f && delta < neighborDelta)
                {
                    neighbor = other;
                    neighborDelta = delta;
                }
            }
            //if they are within range
            if(neighborDelta < (config.FollowClearance + (car.Length + neighbor.Length) / 2))
            {
                //slow down
                //if they are slower than us, decelerate until we match their speed
                //look to change lanes
                car.Speed = math.max(0.0f, car.Speed - car.Acceleration);
            }
            else
            {
                //speed up
                car.Speed = math.min(car.MaxSpeed, car.Speed + car.Acceleration);
            }

            car.Distance += car.Speed;//use delta time

            float xPos = -3.0f + 2.0f * car.LaneNumber;
            car.Position = new float3(xPos, 0.0f, car.Distance);
        }
    }
}
