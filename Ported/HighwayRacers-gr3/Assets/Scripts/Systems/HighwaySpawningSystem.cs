using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[BurstCompile]
partial struct HighwaySpawningSystem : ISystem
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
        var highwayConfig = SystemAPI.GetSingleton<HighwayConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;

        //TODO total Road segments needs to be calculated ahead of time, hard coded for now
        int totalSegments = 20;
        var roadSegments = CollectionHelper.CreateNativeArray<Entity>(totalSegments, allocator);
        ecb.Instantiate(highwayConfig.StraightRoadPrefab, roadSegments);
        
        state.Enabled = false;
    }
}