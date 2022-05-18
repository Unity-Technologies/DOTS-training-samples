using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
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

        foreach (var (trainPosition, train) in SystemAPI.Query<RefRW<DistanceAlongBezier>, RefRO<Train>>())
        {
            if(train.ValueRO.TrainState == TrainState.Moving)
			{
                trainPosition.ValueRW.Distance += config.MaxTrainSpeed * state.Time.DeltaTime;
            }
        }
    }
}

