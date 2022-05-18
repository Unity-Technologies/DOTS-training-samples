using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct TrainStateSystem : ISystem
{
    private BufferFromEntity<Platform> _bufferFromEntity;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        _bufferFromEntity = state.GetBufferFromEntity<Platform>(true);
    }

	public void OnDestroy(ref SystemState state)
	{
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();
        _bufferFromEntity.Update(ref state);

        foreach (var (trainPosition, train) in SystemAPI.Query<RefRO<DistanceAlongBezier>, RefRW<Train>>())
		{
            NativeArray<Platform> platforms = _bufferFromEntity[trainPosition.ValueRO.TrackEntity].AsNativeArray();
            bool trainOnPlatform = IsTrainOnPlatform(trainPosition, platforms, config, ref state);

            if (trainOnPlatform
                && train.ValueRO.TrainState == TrainState.Moving)
            {
                train.ValueRW.TrainState = TrainState.Stopping;
            }
            else if(!trainOnPlatform 
                 && train.ValueRO.TrainState == TrainState.Stopping)
			{
                train.ValueRW.TrainState = TrainState.Stopped;
                train.ValueRW.WaitTimer = (float)state.Time.ElapsedTime + 5.0f;

			}
            else if (train.ValueRO.TrainState == TrainState.Stopped
                && train.ValueRO.WaitTimer < (float)state.Time.ElapsedTime)
			{
                train.ValueRW.TrainState = TrainState.Moving;
			}
		}
    }

    [BurstCompile]
    private bool IsTrainOnPlatform(RefRO<DistanceAlongBezier> trainPosition, NativeArray<Platform> platforms, Config config, ref SystemState state)
    {
        foreach (Platform platform in platforms)
        {
            if (trainPosition.ValueRO.Distance >= platform.startPoint && trainPosition.ValueRO.Distance <= platform.endPoint + config.TrainOffset)
            {
                return true;
            }
        }
        return false;
    }
}
