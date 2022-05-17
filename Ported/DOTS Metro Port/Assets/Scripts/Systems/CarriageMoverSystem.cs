using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct CarriageMover : ISystem
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

        foreach (var (carriage, position) in SystemAPI.Query<RefRO<Carriage>, RefRW<DistanceAlongBezier>>())
        {
            var train = SystemAPI.GetComponent<DistanceAlongBezier>(carriage.ValueRO.Train);
            float carriageDistance = train.Distance - config.TrainOffset - (config.CarriageLength * carriage.ValueRO.CarriageIndex);
            position.ValueRW.Distance = carriageDistance;

        }
    }
}

