using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

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
            Platform? trainOnPlatform = IsTrainOnPlatform(trainPosition, platforms, config, ref state);

            switch(train.ValueRO.TrainState)
			{
                case TrainState.Moving:
				{
                    if(trainOnPlatform.HasValue)
					{
                        train.ValueRW.TrainState = TrainState.Stopping;
					}
                    else
					{
                        train.ValueRW.SpeedPercentage = GetTrainSpeedOffPlatform(train.ValueRO, ref state);
					}
                    break;
				}
                case TrainState.Stopping:
				{
                    if(trainOnPlatform.HasValue)
					{
                        train.ValueRW.SpeedPercentage = GetTrainSpeedOnPlatform(
                            GetPlatformEnterPoint(trainOnPlatform.Value, config),
                            GetPlatformExitPoint(trainOnPlatform.Value, config),
                            trainPosition.ValueRO.Distance);
                    }
                    else
					{
                        train.ValueRW.TrainState = TrainState.Stopped;
                        train.ValueRW.SpeedPercentage = 0.0f;
                        train.ValueRW.DepartureTime = (float)state.Time.ElapsedTime + 5.0f;
                    }
                    break;
				}
                case TrainState.Stopped:
				{
                    if(train.ValueRO.DepartureTime < (float)state.Time.ElapsedTime)
					{
                        train.ValueRW.TrainState = TrainState.Moving;
					}
                    break;
				}
			}
		}
    }

    [BurstCompile]
    private Platform? IsTrainOnPlatform(RefRO<DistanceAlongBezier> trainPosition, NativeArray<Platform> platforms, Config config, ref SystemState state)
    {
        foreach (Platform platform in platforms)
        {
            if (trainPosition.ValueRO.Distance >= GetPlatformEnterPoint(platform, config)
                && trainPosition.ValueRO.Distance <= GetPlatformExitPoint(platform, config))
            {
                return platform;
            }
        }
        return null;
    }

    private float GetPlatformEnterPoint(Platform platform, Config config)
	{
        return platform.startPoint - config.TrainOffset * 3;
    }
    
    private float GetPlatformExitPoint(Platform platform, Config config)
	{
        return platform.endPoint + config.TrainOffset;
    }

    private float GetTrainSpeedOnPlatform(float platformStart, float platformEnd, float trainPosition)
	{
        float maxDistance = platformEnd + 5.0f - platformStart;
        float curDistance = platformEnd + 5.0f - trainPosition;
        return curDistance / maxDistance;
	}

    private float GetTrainSpeedOffPlatform(Train train, ref SystemState state)
	{
        float timeElapsed = (float)state.Time.ElapsedTime - train.DepartureTime;
        return math.clamp(timeElapsed / 3.0f, 0, 1);
	}
}
