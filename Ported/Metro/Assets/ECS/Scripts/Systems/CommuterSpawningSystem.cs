using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[BurstCompile]
partial struct CommuterSpawningSystem : ISystem
{
    // Queries should not be created on the spot in OnUpdate, so they are cached in fields.
    // private EntityQuery m_BaseColorQuery;

    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        // state.RequireForUpdate<Config>();

        // m_BaseColorQuery = state.GetEntityQuery(typeof(URPMaterialPropertyBaseColor));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<CommuterConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        var commuters = CollectionHelper.CreateNativeArray<Entity>(config.spawnAmount, allocator);
        ecb.Instantiate(config.commuterPrefab, commuters);

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
        
        // var config = SystemAPI.GetSingleton<Config>();
        // var random = Random.CreateFromIndex(1234);
        // var hue = random.NextFloat();
        //
        // // Helper to create any amount of colors as distinct from each other as possible.
        // // The logic behind this approach is detailed at the following address:
        // // https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
        // URPMaterialPropertyBaseColor RandomColor()
        // {
        //     // 0.618034005f == 2 / (math.sqrt(5) + 1) == inverse of the golden ratio
        //     hue = (hue + 0.618034005f) % 1;
        //     var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
        //     return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        // }
        //
        // var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        // var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        // var queryMask = m_BaseColorQuery.GetEntityQueryMask();
        //
        // var allocator = state.WorldUnmanaged.UpdateAllocator.ToAllocator;
        // // var vehicles = CollectionHelper.CreateNativeArray<Entity>(config.TankCount, allocator);
        // // ecb.Instantiate(config.TankPrefab, vehicles);
        //
        // // foreach (var vehicle in vehicles)
        // // {
        // //     // Every prefab root contains a LinkedEntityGroup, a list of all of its entities.
        // //     ecb.SetComponentForLinkedEntityGroup(vehicle, queryMask, RandomColor());
        // // }
        //
        // state.Enabled = false;
    }
}