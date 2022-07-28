using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
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
            ecb.SetComponent(cell, new BucketId { Value = i });
            ++i;
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}


[BurstCompile]
partial struct BucketSystem : ISystem
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

        int bucketCount = 0;
        foreach (var (color, volume) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRO<Volume>>().WithAll<BucketInfo>())
        {
            if (volume.ValueRO.Value > 0)
                color.ValueRW.Value = new float4(0, 0, 1, 1);
            else
                color.ValueRW.Value = new float4(0, 0, 0, 1);

            bucketCount++;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Left mouse clicked");
                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

                Entity bucket = ecb.Instantiate(config.Prefab);

                ecb.SetComponent(bucket, new Translation { Value = new float3(hit.point) });
                ecb.SetComponent(bucket, new NonUniformScale { Value = new float3(configCell.CellSize * 0.5f, configCell.CellSize * 0.5f, configCell.CellSize * 0.5f) });
                ecb.SetComponent(bucket, new BucketId { Value = bucketCount });
                ecb.SetComponent(bucket, new Volume { Value = 1.0f });
            }
        }

    }
}