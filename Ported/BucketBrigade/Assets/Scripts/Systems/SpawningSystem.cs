using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[BurstCompile]
partial struct SpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var workerFull = CollectionHelper.CreateNativeArray<Entity>(config.WorkerFullCount, allocator);
        ecb.Instantiate(config.WorkerFullPrefab, workerFull);
        
        var workerEmpty = CollectionHelper.CreateNativeArray<Entity>(config.WorkerEmptyCount, allocator);
        ecb.Instantiate(config.WorkerEmptyPrefab, workerEmpty);

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}