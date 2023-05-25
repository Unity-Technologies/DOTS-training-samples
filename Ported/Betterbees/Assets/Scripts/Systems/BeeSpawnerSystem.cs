using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public partial struct BeeSpawnerSystem : ISystem
{
    private uint _updateCounter;

    private bool haveBeesSpawned;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (haveBeesSpawned)
        {
            return;
        }

        var config = SystemAPI.GetSingleton<Config>();
        var random = Random.CreateFromIndex(_updateCounter);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (spawner, spawnerTransform) in SystemAPI.Query<RefRO<BeeSpawnerComponent>, RefRO<LocalTransform>>())
        {
            SpawnBees(
                config.beeCount,
                spawner.ValueRO.beePrefab,
                spawnerTransform.ValueRO.Position,
                config.maxSpawnSpeed,
                spawner.ValueRO.hiveTag,
                spawner.ValueRO.minBounds,
                spawner.ValueRO.maxBounds,
                ecb,
                ref random);

            _updateCounter += (uint)config.beeCount;
        }

        haveBeesSpawned = true;
    }

    public static void SpawnBees(
        int beeCount,
        Entity beePrefab,
        float3 spawnPosition,
        float maxSpawnSpeed,
        HiveTag hiveTag,
        float3 homeMinBounds,
        float3 homeMaxBounds,
        EntityCommandBuffer ecb,
        ref Random random)
    {
        for (int i = 0; i < beeCount; i++)
        {
            Entity newBee = ecb.Instantiate(beePrefab);

            ecb.SetComponent(newBee, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f
            });

            ecb.SetComponent(newBee, new VelocityComponent
            {
                Velocity = random.NextFloat3Direction() * maxSpawnSpeed
            });

            ecb.SetComponent(newBee, new ReturnHomeComponent
            {
                HomeMinBounds = homeMinBounds,
                HomeMaxBounds = homeMaxBounds
            });

            ecb.SetComponent(newBee, new BeeState
            {
                state = BeeState.State.IDLE,
                hiveTag = hiveTag
            });

            ecb.AddComponent(newBee, new PostTransformMatrix
            {
                Value = float4x4.identity
            });

            switch (hiveTag)
            {
                case HiveTag.HiveYellow:
                    ecb.AddComponent(newBee, new HiveYellow());
                    break;
                case HiveTag.HiveBlue:
                    ecb.AddComponent(newBee, new HiveBlue());
                    break;
                case HiveTag.HiveOrange:
                default:
                    ecb.AddComponent(newBee, new HiveOrange());
                    break;
            }

            ecb.AddComponent(newBee, new DeadBee());
            ecb.SetComponentEnabled(newBee, typeof(DeadBee), false);
        }
    }
}
