using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct TerrainSpawningSystem : ISystem
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
        var config = SystemAPI.GetSingleton<TerrainCellConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var waterCell = CollectionHelper.CreateNativeArray<Entity>(config.GridSize * config.GridSize, Allocator.Temp);


        ecb.Instantiate(config.Prefab, waterCell);


        int i = 0;
        var offset = new float3(-config.GridSize * config.CellSize * 0.5f, -1.0f, -config.GridSize * config.CellSize * 0.5f);
        foreach (var cell in waterCell)
        {
            ecb.SetComponent(cell, new Translation { Value = new float3((i % config.GridSize) * config.CellSize, 0.0f, Mathf.Ceil(i / config.GridSize) * config.CellSize) + offset });
            ecb.SetComponent(cell, new NonUniformScale { Value = new float3( config.CellSize, config.CellSize*2, config.CellSize) });
            ++i;
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}