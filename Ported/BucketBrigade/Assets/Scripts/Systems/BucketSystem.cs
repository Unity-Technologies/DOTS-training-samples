using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct BucketSpawningSystem : ISystem
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
        var config = SystemAPI.GetSingleton<BucketConfig>();
        var configCell = SystemAPI.GetSingleton<TerrainCellConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var buckets = CollectionHelper.CreateNativeArray<Entity>(config.Count, Allocator.Temp);


        ecb.Instantiate(config.Prefab, buckets);

        float gridRadius = config.GridSize * 0.5f * config.CellSize;
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)(config.Count * config.GridSize * config.CellSize));

        int i = 0;
        foreach (var cell in buckets)
        {
            ecb.SetComponent(cell, new Translation { Value = new float3(rand.NextFloat(-gridRadius, gridRadius), configCell.CellSize * 0.25f, rand.NextFloat(-gridRadius, gridRadius)) });
            ecb.SetComponent(cell, new NonUniformScale { Value = new float3(configCell.CellSize * 0.5f, configCell.CellSize * 0.5f, configCell.CellSize * 0.5f) });
            ++i;
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}