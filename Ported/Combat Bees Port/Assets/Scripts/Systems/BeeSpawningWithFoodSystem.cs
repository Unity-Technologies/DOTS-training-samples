using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BeeSpawningWithFoodSystem : ISystem
{
    Random random;
    
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
        random = Random.CreateFromIndex(state.GlobalSystemVersion);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var baseComponent = SystemAPI.GetSingleton<Base>();
        
        foreach (var (foodPiece, translation) in SystemAPI.Query<Entity, RefRO<Translation>>().WithAll<Food>())
        {
            var position = translation.ValueRO.Value;
            if (baseComponent.blueBase.GetBaseRightCorner().x < position.x && position.y < -5)
            {
                ecb.DestroyEntity(foodPiece);
                
                var baseInfo = SystemAPI.GetSingleton<Base>();
                var beeSpawn = SystemAPI.GetSingleton<BeeSpawnData>();
                var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.beeCount, Allocator.Temp);
                var randomSpawn = random.NextFloat3(baseInfo.blueBase.GetBaseLowerLeftCorner(), baseInfo.blueBase.GetBaseRightCorner());
        
                ecb.Instantiate(beeSpawn.blueBeePrefab, newBeeArray);

                foreach (var instance in newBeeArray)
                {
                    ecb.SetComponent(instance, new Translation { Value = randomSpawn});
                    ecb.SetComponent(instance, new Bee
                    {
                        state = BeeState.Idle
                    });
        
                    ecb.AddComponent(instance, new BlueTeam());
                }
            }
            
            else if (baseComponent.yellowBase.GetBaseRightCorner().x > position.x && position.y < -5)
            {
                ecb.DestroyEntity(foodPiece);
                
                var baseInfo = SystemAPI.GetSingleton<Base>();
                var beeSpawn = SystemAPI.GetSingleton<BeeSpawnData>();
                var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.beeCount, Allocator.Temp);
                var randomSpawn = random.NextFloat3(baseInfo.yellowBase.GetBaseLowerLeftCorner(), baseInfo.yellowBase.GetBaseRightCorner());
        
                ecb.Instantiate(beeSpawn.yellowBeePrefab, newBeeArray);

                foreach (var instance in newBeeArray)
                {
                    ecb.SetComponent(instance, new Translation { Value = randomSpawn});
                    ecb.SetComponent(instance, new Bee
                    {
                        state = BeeState.Idle
                    });
        
                    ecb.AddComponent(instance, new YellowTeam());
                }
            }
        }
    }
}

[BurstCompile]
partial struct YellowBeeSpawnJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    [ReadOnly]
    public Random random;

    void Execute(in BeeSpawnAspect beeSpawn)
    {
        var baseInfo = SystemAPI.GetSingleton<Base>();
        var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.BeeCount, Allocator.Temp);
        var randomSpawn = random.NextFloat3(baseInfo.yellowBase.GetBaseLowerLeftCorner(), baseInfo.yellowBase.GetBaseRightCorner());
        
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
    public EntityCommandBuffer ECB;
    [ReadOnly]
    public Random random;

    void Execute(in BeeSpawnAspect beeSpawn)
    {
        var baseInfo = SystemAPI.GetSingleton<Base>();
        var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.BeeCount, Allocator.Temp);
        var randomSpawn = random.NextFloat3(baseInfo.blueBase.GetBaseLowerLeftCorner(), baseInfo.blueBase.GetBaseRightCorner());
        
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
