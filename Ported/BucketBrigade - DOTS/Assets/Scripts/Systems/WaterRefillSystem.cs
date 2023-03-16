using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BucketFillingSystem))]
[BurstCompile]
public partial struct WaterRefillSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        new RefillWaterJob
        {
            config = config
        }.ScheduleParallel();
    }
}

[BurstCompile]
partial struct RefillWaterJob : IJobEntity
{
    public Config config;

    [BurstCompile]
    private void Execute(RefRW<LocalTransform> transform, RefRW<Water> water)
    {
        if(water.ValueRO.CurrCapacity < water.ValueRO.MaxCapacity){
            water.ValueRW.CurrCapacity += config.refillRate;
        }
        float3 newWaterScale= math.lerp(float3.zero, math.float3(1,0.01f,1), water.ValueRO.CurrCapacity/water.ValueRO.MaxCapacity);
        transform.ValueRW.Scale = 5*newWaterScale.x; //times 5 because of the original scale in spawner
    }
}