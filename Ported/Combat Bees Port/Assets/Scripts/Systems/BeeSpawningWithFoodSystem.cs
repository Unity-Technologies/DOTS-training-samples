using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BeeSpawningWithFoodSystem : ISystem
{
    Base m_BaseComponent;
    ComponentDataFromEntity<LocalToWorld> m_LocalToWorldFromEntity;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_BaseComponent = SystemAPI.GetSingleton<Base>();
        var beeSpawn = SystemAPI.GetSingleton<InitialSpawn>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var (foodPiece, translation) in SystemAPI.Query<Entity, RefRO<Translation>>().WithAll<Food>())
        {
            var position = translation.ValueRO.Value;

            if (m_BaseComponent.blueBase.GetBaseUpperRightCorner().x <= position.x && position.y < -8)
            {
                var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.beePulseSpawnCount, Allocator.Temp);
        
                ecb.Instantiate(beeSpawn.blueBeePrefab, newBeeArray);
                
                foreach (var instance in newBeeArray)
                {
                    ecb.SetComponent(instance, new Translation { Value = position});
                    ecb.SetComponent(instance, new Bee
                    {
                        state = BeeState.Idle
                    });
        
                    ecb.AddComponent(instance, new BlueTeam());
                }
                
                var spawnFlashJob = new SpawnFlashSpawn
                {
                    ECB = ecb,
                    random = Random.CreateFromIndex(12345),
                    spawnPos = position
                };
                spawnFlashJob.Run();
                
                ecb.DestroyEntity(foodPiece);
            }
            
            else if (m_BaseComponent.yellowBase.GetBaseUpperRightCorner().x >= position.x && position.y < -8)
            {
                var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.beePulseSpawnCount, Allocator.Temp);
        
                ecb.Instantiate(beeSpawn.yellowBeePrefab, newBeeArray);

                foreach (var instance in newBeeArray)
                {
                    ecb.SetComponent(instance, new Translation { Value = position});
                    ecb.SetComponent(instance, new Bee
                    {
                        state = BeeState.Idle
                    });
        
                    ecb.AddComponent(instance, new YellowTeam());
                }
                
                var spawnFlashJob = new SpawnFlashSpawn
                {
                    ECB = ecb,
                    random = Random.CreateFromIndex(12345),
                    spawnPos = position
                };
                spawnFlashJob.Run();

                ecb.DestroyEntity(foodPiece);
            }
        }
    }
}

[BurstCompile]
partial struct YellowBeeSpawnJob : IJobEntity
{
    EntityCommandBuffer ECB;
    [ReadOnly]
    Random random;

    void Execute(in BeeSpawnAspect beeSpawn)
    {
        var baseInfo = SystemAPI.GetSingleton<Base>();
        var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.PulseSpawnCount, Allocator.Temp);
        var randomSpawn = random.NextFloat3(baseInfo.yellowBase.GetBaseLowerLeftCorner(), baseInfo.yellowBase.GetBaseUpperRightCorner());
        
        ECB.Instantiate(beeSpawn.YellowBeePrefab, newBeeArray);

        foreach (var instance in newBeeArray)
        {
            ECB.SetComponent(instance, new Translation { Value = randomSpawn});
            ECB.SetComponent(instance, new Bee
            {
                state = BeeState.Idle
            });
        
            ECB.AddComponent(instance, new YellowTeam());
        }
        
    }
}

[BurstCompile]
partial struct BlueBeeSpawnJob : IJobEntity
{
    EntityCommandBuffer ECB;
    [ReadOnly]
    Random random;

    void Execute(in BeeSpawnAspect beeSpawn)
    {
        var baseInfo = SystemAPI.GetSingleton<Base>();
        var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.PulseSpawnCount, Allocator.Temp);
        var randomSpawn = random.NextFloat3(baseInfo.blueBase.GetBaseLowerLeftCorner(), baseInfo.blueBase.GetBaseUpperRightCorner());
        
        ECB.Instantiate(beeSpawn.BlueBeePrefab, newBeeArray);

        foreach (var instance in newBeeArray)
        {
            ECB.SetComponent(instance, new Translation { Value = randomSpawn});
            ECB.SetComponent(instance, new Bee
            {
                state = BeeState.Idle
            });
        
            ECB.AddComponent(instance, new BlueTeam());
        }
    }
}

[BurstCompile]
partial struct SpawnFlashSpawn : IJobEntity
{
    public EntityCommandBuffer ECB;
    [ReadOnly]
    public Random random;

    public float3 spawnPos;

    void Execute(in InitialSpawn prefab)
    {
        var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(4, Allocator.Temp);
            
        
        ECB.Instantiate(prefab.spawnFlashPrefab, newBeeArray);

        foreach (var instance in newBeeArray)
        {
            ECB.SetComponent(instance, new Translation { Value = spawnPos + random.NextFloat3()});
        }
        
    }
}
