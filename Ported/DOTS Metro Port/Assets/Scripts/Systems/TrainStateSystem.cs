using Unity.Entities;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public partial struct TrainStateSystem : ISystem
{
    private BufferFromEntity<Platform> _bufferFromEntity;

    public void OnCreate(ref SystemState state)
    {
        _bufferFromEntity = state.GetBufferFromEntity<Platform>(true);
    }

	public void OnDestroy(ref SystemState state)
	{
	}

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
	{
        _bufferFromEntity.Update(ref state);
        foreach (var (trainPosition, train) in SystemAPI.Query<RefRO<DistanceAlongBezier>, RefRW<Train>>())
		{
            NativeArray<Platform> platforms = _bufferFromEntity[trainPosition.ValueRO.TrackEntity].AsNativeArray();
            bool trainOnPlatform = IsTrainOnPlatform(trainPosition, platforms, ref state);
            if (trainOnPlatform
                && train.ValueRO.TrainState == TrainState.Moving)
            {
                train.ValueRW.WaitTimer = (float)state.Time.ElapsedTime + 5.0f;
                train.ValueRW.TrainState = TrainState.Stopped;
            }
            else if (trainOnPlatform
                && train.ValueRO.TrainState == TrainState.Stopped
                && train.ValueRO.WaitTimer < (float)state.Time.ElapsedTime)
			{
                train.ValueRW.TrainState = TrainState.Leaving;
			}
            else if(!trainOnPlatform
                && train.ValueRO.TrainState == TrainState.Leaving)
			{
                train.ValueRW.TrainState = TrainState.Moving;
            }
		}
    }

    [BurstCompile]
    private bool IsTrainOnPlatform(RefRO<DistanceAlongBezier> trainPosition, NativeArray<Platform> platforms, ref SystemState state)
    {
        foreach (Platform platform in platforms)
        {
            if (trainPosition.ValueRO.Distance >= platform.startPoint && trainPosition.ValueRO.Distance <= platform.endPoint)
            {
                return true;
            }
        }
        return false;
    }
}
