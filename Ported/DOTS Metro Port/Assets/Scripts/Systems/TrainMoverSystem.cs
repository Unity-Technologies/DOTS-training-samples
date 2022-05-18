using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
[UpdateAfter(typeof(TrainStateSystem))]
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
            switch(train.ValueRO.TrainState)
			{
                case TrainState.Moving:
				{
                    MoveTrain(trainPosition, train.ValueRO, config, ref state);
                    break;
				}
                case TrainState.Stopping:
				{
                    MoveTrain(trainPosition, train.ValueRO, config, ref state);
                    break;
				}
			}
        }
    }

    private void MoveTrain(RefRW<DistanceAlongBezier> trainPosition, Train train, Config config, ref SystemState state)
	{
        trainPosition.ValueRW.Distance += config.MaxTrainSpeed * train.SpeedPercentage * state.Time.DeltaTime;
    }
}

