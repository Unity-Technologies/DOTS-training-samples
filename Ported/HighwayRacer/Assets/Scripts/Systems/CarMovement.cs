using Aspects;
using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public partial struct CarMovement : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        foreach (var car in SystemAPI.Query<CarAspect>())
        {
            if (car.Distance > 100.0)
            {
                car.Distance = 0.0f;
            }
            float xPos = -3.0f + 2.0f * car.LaneNumber;
            car.Position = new float3(xPos, 0.0f, car.Distance);
            car.Distance += car.Speed;
        }
    }
}
