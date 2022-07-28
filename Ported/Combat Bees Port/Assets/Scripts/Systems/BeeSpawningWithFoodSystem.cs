using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct BeeSpawningWithFoodSystem : ISystem
{
    ComponentDataFromEntity<LocalToWorld> m_LocalToWorldFromEntity;
    Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Base>();
        state.RequireForUpdate<InitialSpawn>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    public void SpawnParticles(EntityCommandBuffer ecb, InitialSpawn beeSpawn, float3 spawnPosition)
    {
        var particleArray = CollectionHelper.CreateNativeArray<Entity>(4, Allocator.Temp);
        
        ecb.Instantiate(beeSpawn.spawnFlashPrefab, particleArray);

        foreach (var instance in particleArray)
        {
            ecb.SetComponent(instance, new Translation { Value = spawnPosition + random.NextFloat3()});
        }
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        random = Random.CreateFromIndex(state.GlobalSystemVersion);
        
        var beeSpawn = SystemAPI.GetSingleton<InitialSpawn>();
        var baseInfo = SystemAPI.GetSingleton<Base>();
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (foodPiece, translation) in SystemAPI.Query<Entity, RefRO<Translation>>().WithAny<Food>())
        {
            var position = translation.ValueRO.Value;

            if (position.y < -8 && baseInfo.blueBase.GetBaseUpperRightCorner().x <= position.x)
            {
                var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.beePulseSpawnCount, Allocator.Temp);

                ecb.Instantiate(beeSpawn.blueBeePrefab, newBeeArray);
                
                foreach (var instance in newBeeArray)
                {
                    ecb.SetComponent(instance, new Translation
                    {
                        Value = position
                    });
                    ecb.AddComponent(instance, new Bee
                    {
                        target = Entity.Null,
                        state = BeeState.Idle
                    });

                    ecb.AddComponent(instance, new BlueTeam());
                }

                SpawnParticles(ecb, beeSpawn, position);

                ecb.DestroyEntity(foodPiece);
            }

            else if (position.y < -8 && baseInfo.yellowBase.GetBaseUpperRightCorner().x >= position.x)
            {
                var newBeeArray = CollectionHelper.CreateNativeArray<Entity>(beeSpawn.beePulseSpawnCount, Allocator.Temp);
        
                ecb.Instantiate(beeSpawn.yellowBeePrefab, newBeeArray);

                foreach (var instance in newBeeArray)
                {
                    ecb.SetComponent(instance, new Translation { Value = position});
                    ecb.AddComponent(instance, new Bee
                    {
                        target = Entity.Null,
                        state = BeeState.Idle
                    });
        
                    ecb.AddComponent(instance, new YellowTeam());
                }
                
                SpawnParticles(ecb, beeSpawn, position);

                ecb.DestroyEntity(foodPiece);
            }
        }
    }
}
