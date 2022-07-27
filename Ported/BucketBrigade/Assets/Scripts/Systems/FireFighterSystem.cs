using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct FireFighterSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<FireFighterConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var entityArray = CollectionHelper.CreateNativeArray<Entity>((config.PerLinesCount * 2 + 2) * config.LinesCount , Allocator.Temp);


        ecb.Instantiate(config.FireFighterPrefab, entityArray);

        int index = 0;

        var entityItt = entityArray.GetEnumerator();

        var WaterBringersPrefab = state.EntityManager.Instantiate(config.FireFighterPrefab);
        state.EntityManager.AddComponent<WaterBringer>(WaterBringersPrefab);
        state.EntityManager.AddComponent<Target>(WaterBringersPrefab);
        state.EntityManager.AddComponent<LineId>(WaterBringersPrefab);
        state.EntityManager.AddComponent<LineIndex>(WaterBringersPrefab);
        state.EntityManager.AddComponent<BucketId>(WaterBringersPrefab);
        state.EntityManager.AddComponent<URPMaterialPropertyBaseColor>(WaterBringersPrefab);

        state.EntityManager.Instantiate(WaterBringersPrefab, config.LinesCount * config.PerLinesCount, Allocator.Temp);

        //foreach(var (tran, color) in SystemAPI.Query<RefRW<Translation>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<WaterBringer>())
        //{
        //    tran.ValueRW.Value  = new float3(UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f), 0, UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f));
        //    color.ValueRW.Value = new float4(config.WaterBringersColor.r, config.WaterBringersColor.g, config.WaterBringersColor.b, 1.0f);
        //}

        // WaterBringers
        for (int lineId = 0; lineId < config.LinesCount; ++lineId)
        {
            for (int lineIndex = 0; lineIndex < config.PerLinesCount; ++lineIndex)
            {
                ecb.AddComponent(entityArray[index], new WaterBringer { });
                ecb.AddComponent(entityArray[index], new Target() );
                ecb.AddComponent(entityArray[index], new LineId { Value = lineId } );
                ecb.AddComponent(entityArray[index], new LineIndex { Value = lineIndex });
                ecb.AddComponent(entityArray[index], new BucketId () );
                ecb.AddComponent(entityArray[index], new URPMaterialPropertyBaseColor { Value = new float4(config.WaterBringersColor.r, config.WaterBringersColor.g, config.WaterBringersColor.b, 1.0f) });
                ecb.SetComponent(entityArray[index], new Translation { Value = new float3(UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f), 0, UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f)) });
                ++index;
            }
        }
        // BucketBringers
        for (int LineId = 0; LineId < config.LinesCount; ++LineId)
        {
            for (int lineIndex = 0; lineIndex < config.PerLinesCount; ++lineIndex)
            {
                ecb.AddComponent(entityArray[index], new BucketBringer { });
                ecb.AddComponent(entityArray[index], new Target { });
                ecb.AddComponent(entityArray[index], new LineId { Value = LineId });
                ecb.AddComponent(entityArray[index], new LineIndex { Value = lineIndex });
                ecb.AddComponent(entityArray[index], new BucketId { });
                ecb.AddComponent(entityArray[index], new URPMaterialPropertyBaseColor { Value = new float4(config.BucketBringersColor.r, config.BucketBringersColor.g, config.BucketBringersColor.b, 1.0f) });
                ecb.SetComponent(entityArray[index], new Translation { Value = new float3(UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f), 0, UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f)) });
                ++index;
            }
        }
        // BucketFillerFetcher
        for (int LineId = 0; LineId < config.LinesCount; ++LineId)
        {
            ecb.AddComponent(entityArray[index], new BucketFillerFetcher { });
            ecb.AddComponent(entityArray[index], new Target { });
            ecb.AddComponent(entityArray[index], new LineId { Value = LineId });
            ecb.AddComponent(entityArray[index], new LineIndex { });
            ecb.AddComponent(entityArray[index], new BucketId { });
            ecb.AddComponent(entityArray[index], new URPMaterialPropertyBaseColor { Value = new float4(config.BucketFillerFetcherColor.r, config.BucketFillerFetcherColor.g, config.BucketFillerFetcherColor.b, 1.0f) });
            ecb.SetComponent(entityArray[index], new Translation { Value = new float3(UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f), 0, UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f)) });
            ++index;
        }
        // WaterDumper
        for (int LineId = 0; LineId < config.LinesCount; ++LineId)
        {
            ecb.AddComponent(entityArray[index], new WaterDumper { });
            ecb.AddComponent(entityArray[index], new Target { });
            ecb.AddComponent(entityArray[index], new LineId { Value = LineId });
            ecb.AddComponent(entityArray[index], new LineIndex { });
            ecb.AddComponent(entityArray[index], new BucketId { });
            ecb.AddComponent(entityArray[index], new URPMaterialPropertyBaseColor { Value = new float4(config.WaterDumperColor.r, config.WaterDumperColor.g, config.WaterDumperColor.b, 1.0f) });
            ecb.SetComponent(entityArray[index], new Translation { Value = new float3(UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f), 0, UnityEngine.Random.Range(-config.GridSize * 0.5f, config.GridSize * 0.5f)) });
            ++index;
        }
        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}