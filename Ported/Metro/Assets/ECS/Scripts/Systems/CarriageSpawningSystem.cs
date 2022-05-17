using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct CarriageSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var carriages = CollectionHelper.CreateNativeArray<Entity>(config.TrainsToSpawn, allocator);
        ecb.Instantiate(config.CarriagePrefab, carriages);
        
        state.Enabled = false;
    }
}