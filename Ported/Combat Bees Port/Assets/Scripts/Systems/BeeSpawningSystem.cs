using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
partial struct BeeSpawningSystem : ISystem
{
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
        var spawner = SystemAPI.GetSingleton<InitialSpawn>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var blueBees = CollectionHelper.CreateNativeArray<Entity>(spawner.beeCount/2, Allocator.Temp);
        var yellowBees = CollectionHelper.CreateNativeArray<Entity>(spawner.beeCount/2, Allocator.Temp);
        var food = CollectionHelper.CreateNativeArray<Entity>(spawner.foodCount, Allocator.Temp);
        
        ecb.Instantiate(spawner.blueBeePrefab, blueBees);

        foreach (var bee in blueBees)
        {
            ecb.SetComponent(bee, new Translation
            {
                Value = spawner.blueBase
            });
        }

        ecb.Instantiate(spawner.yellowBeePrefab, yellowBees);
        
        foreach (var bee in yellowBees)
        {
            ecb.SetComponent(bee, new Translation
            {
                Value = spawner.yellowBase
            });
        }
        
        ecb.Instantiate(spawner.foodPrefab, food);
        
        foreach (var foodNode in food)
        {
            ecb.SetComponent(foodNode, new Translation
            {
                Value = spawner.mapCenter
            });
        }

        state.Enabled = false;
    }
}
