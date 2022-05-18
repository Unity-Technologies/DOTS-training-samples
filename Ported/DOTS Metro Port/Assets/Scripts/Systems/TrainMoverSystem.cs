using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct TrainMoverSystem : ISystem
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

    public void OnUpdate(ref SystemState state)
    {
        Config config = SystemAPI.GetSingleton<Config>();
        _bufferFromEntity.Update(ref state);

        foreach (var train in SystemAPI.Query<RefRW<DistanceAlongBezier>>().WithAll<Train>())
        {
            NativeArray<Platform> platforms = _bufferFromEntity[train.ValueRW.TrackEntity].AsNativeArray();

            if (IsTrainOnPlatform(train, platforms, ref state))
            {
                train.ValueRW.Distance += config.MaxTrainSpeed/4 * state.Time.DeltaTime;

            }

            else
            {
                train.ValueRW.Distance += config.MaxTrainSpeed * state.Time.DeltaTime;

            }

        }


    }

    private bool IsTrainOnPlatform(RefRW<DistanceAlongBezier> train, NativeArray<Platform> platforms, ref SystemState state)
    {
        foreach (Platform platform in platforms)
        {
            if (train.ValueRW.Distance >= platform.startPoint && train.ValueRW.Distance <= platform.endPoint)
            {
                return true;
            }

        }

        return false;
    }

}

