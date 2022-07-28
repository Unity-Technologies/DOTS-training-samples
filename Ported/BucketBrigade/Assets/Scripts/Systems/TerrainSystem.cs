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
        state.RequireForUpdate<Temperature>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var terrainConfig = SystemAPI.GetSingleton<TerrainCellConfig>();
        int GridSize = terrainConfig.GridSize;
        float CellSize = terrainConfig.CellSize;

        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var terrainCell = CollectionHelper.CreateNativeArray<Entity>(GridSize * GridSize, Allocator.Temp);


        ecb.Instantiate(terrainConfig.Prefab, terrainCell);

        var HeatMap = SystemAPI.GetSingletonBuffer<Temperature>();

        int i = 0;
        var offset = new float3(-GridSize * CellSize * 0.5f + CellSize * 0.5f, -CellSize, -GridSize * CellSize * 0.5f + CellSize * 0.5f);
        foreach (var cell in terrainCell)
        {
            ecb.SetComponent(cell, new Translation { Value = new float3((i % GridSize) * CellSize, 0.0f, math.floor(i / GridSize) * CellSize) + offset });
            ecb.SetComponent(cell, new NonUniformScale { Value = new float3( CellSize, CellSize*2, CellSize) });

            HeatMap.Add(new Temperature { Value = 0f });

            ++i;
        }

        // Set in fire some terrainCell
        for(int j = 0; j < config.InitialFireCount; ++j)
        {
            int rndIndex = UnityEngine.Random.Range(0, (GridSize * GridSize) - 1);
            HeatMap.ElementAt(rndIndex).Value = config.FireThreshold;
        }  

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }

}