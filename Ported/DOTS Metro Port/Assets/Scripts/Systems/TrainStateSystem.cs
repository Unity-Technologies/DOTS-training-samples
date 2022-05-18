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
            train.ValueRW.TrainState = IsTrainOnPlatform(trainPosition, platforms, ref state)
                ? TrainState.Stopped
                : TrainState.Moving;
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
