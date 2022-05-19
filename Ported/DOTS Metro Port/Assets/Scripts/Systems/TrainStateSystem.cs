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
        var job = new ManageTrainStateJob { BufferFromEntity = _bufferFromEntity, Config = config, ElapsedTime = (float)state.Time.ElapsedTime };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ManageTrainStateJob : IJobEntity
{
    [ReadOnly]
    public BufferFromEntity<Platform> BufferFromEntity;
    public Config Config;
    public float ElapsedTime;

    [BurstCompile]
    public void Execute(in DistanceAlongBezier trainPosition, ref Train train)
    {
        NativeArray<Platform> platforms = BufferFromEntity[trainPosition.TrackEntity].AsNativeArray();
        Platform? trainOnPlatform = IsTrainOnPlatform(trainPosition, platforms, Config);
        

        switch (train.TrainState)
        {
            case TrainState.Moving:
            {
                if (trainOnPlatform.HasValue)
                {
                    train.TrainState = TrainState.Stopping;
                }
                else
                {
                    train.SpeedPercentage = GetTrainSpeedOffPlatform(train, ElapsedTime);
                }
                break;
            }
            case TrainState.Stopping:
            {
                if (trainOnPlatform.HasValue)
                {
                    train.SpeedPercentage = GetTrainSpeedOnPlatform(
                        GetPlatformEnterPoint(trainOnPlatform.Value, Config),
                        GetPlatformExitPoint(trainOnPlatform.Value, Config),
                        trainPosition.Distance);
                }
                else
                {
                    train.TrainState = TrainState.Stopped;
                    train.SpeedPercentage = 0.0f;
                    train.DepartureTime = ElapsedTime + Config.TrainWaitTime;
                }
                break;
            }
            case TrainState.Stopped:
            {
                if (train.DepartureTime < ElapsedTime)
                {
                    train.TrainState = TrainState.Moving;
                }
                break;
            }
        }
    }

    [BurstCompile]
    private Platform? IsTrainOnPlatform(DistanceAlongBezier trainPosition, NativeArray<Platform> platforms, Config config)
    {
        foreach (Platform platform in platforms)
        {
            if (trainPosition.Distance >= GetPlatformEnterPoint(platform, config)
                && trainPosition.Distance <= GetPlatformExitPoint(platform, config))
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

    private float GetTrainSpeedOffPlatform(Train train, float elapsedTime)
    {
        float timeElapsed = elapsedTime - train.DepartureTime;
        return math.clamp(timeElapsed / 3.0f, 0, 1);
    }
}

