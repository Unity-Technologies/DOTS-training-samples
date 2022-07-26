using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Asset = UnityEditor.VersionControl.Asset;

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

    public void OnUpdate(ref SystemState state)
    {
        random = Random.CreateFromIndex(state.GlobalSystemVersion);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var baseComponent = SystemAPI.GetSingleton<Base>();
        
        foreach (var foodPiece in SystemAPI.Query<Entity>().WithAll<Food>())
        {
            var position = state.EntityManager.GetComponentData<Translation>(foodPiece).Value;
            if (baseComponent.blueBase.GetBaseRightCorner().x < position.x)
            {
                var beeSpawnJob = new BlueBeeSpawnJob()
                {
                    ECB = ecb,
                    random = random
                };

                beeSpawnJob.Schedule();
            }
            
            if (baseComponent.yellowBase.GetBaseRightCorner().x > position.x)
            {
                var beeSpawnJob = new YellowBeeSpawnJob()
                {
                    ECB = ecb,
                    random = random
                };

                beeSpawnJob.Schedule();
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

    void Execute(in BeeSpawnAspect beeSpawn, in BaseAspect baseInfo)
    {
        var instance = ECB.Instantiate(beeSpawn.YellowBeePrefab);
        var randomSpawn = random.NextFloat3(baseInfo.YellowBase.GetBaseLowerLeftCorner(), baseInfo.YellowBase.GetBaseRightCorner());
        
        ECB.SetComponent(instance, new Translation { Value = randomSpawn});
        ECB.SetComponent(instance, new Bee
        {
            state = BeeState.Idle
        });
        
        ECB.AddComponent(instance, new YellowTeam());
    }
}

[BurstCompile]
partial struct BlueBeeSpawnJob : IJobEntity
{
    public EntityCommandBuffer ECB;
    [ReadOnly]
    public Random random;

    void Execute(in BeeSpawnAspect beeSpawn, in BaseAspect baseInfo)
    {
        var instance = ECB.Instantiate(beeSpawn.BlueBeePrefab);
        var randomSpawn = random.NextFloat3(baseInfo.BlueBase.GetBaseLowerLeftCorner(), baseInfo.BlueBase.GetBaseRightCorner());
        
        ECB.SetComponent(instance, new Translation { Value = randomSpawn});
        ECB.SetComponent(instance, new Bee
        {
            state = BeeState.Idle
        });
        
        ECB.AddComponent(instance, new BlueTeam());
    }
}
