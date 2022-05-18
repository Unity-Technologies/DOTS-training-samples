using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class BucketSpawningSystem {
    //The purpose of this system is to create all the buckets, spawning them in random cells. 
    //For this I will need to read from the cell array, picking random coords.

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Config>();
    }
    
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var buckets = CollectionHelper.CreateNativeArray<Entity>(config.bucketCount, allocator);
        ecb.Instantiate(config.bucketPrefab, buckets);

        //only meant to run once, so disable afterwards. 
        state.Enabled = false;
    }
}