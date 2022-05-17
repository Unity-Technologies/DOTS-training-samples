using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct TrainMoverSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();
        foreach (var position in SystemAPI.Query<RefRW<DistanceAlongBezier>>().WithAll<Train>())
        {
            position.ValueRW.Distance += config.MaxTrainSpeed * state.Time.DeltaTime;
        }
    }
}

