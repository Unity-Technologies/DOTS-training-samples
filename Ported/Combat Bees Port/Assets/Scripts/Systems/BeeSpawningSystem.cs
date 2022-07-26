using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

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
        var food = CollectionHelper.CreateNativeArray<Entity>(spawner.foodCount/2, Allocator.Temp);
        
        ecb.Instantiate(spawner.blueBeePrefab, blueBees);
        ecb.Instantiate(spawner.yellowBeePrefab, yellowBees);
        ecb.Instantiate(spawner.foodPrefab, food);

        state.Enabled = false;
    }
}
