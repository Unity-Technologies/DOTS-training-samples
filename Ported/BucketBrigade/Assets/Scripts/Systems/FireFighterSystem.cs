using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct FireFighterSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    private Entity InitFireFighter(ref SystemState state, FireFighterConfig config, ComponentType[] components)
    {
        var FireFighterPrefab = state.EntityManager.Instantiate(config.FireFighterPrefab);

        foreach(var component in components)
        {
            state.EntityManager.AddComponent(FireFighterPrefab, component);
        }

        return FireFighterPrefab;
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<FireFighterConfig>();

        var WaterBringersEntity = InitFireFighter(ref state, config, new ComponentType[] { 
            typeof(WaterBringer), 
            typeof(Target), 
            typeof(LineId), 
            typeof(LineIndex), 
            typeof(BucketId), 
            typeof(URPMaterialPropertyBaseColor) });
        var BucketBringerEntity = InitFireFighter(ref state, config, new ComponentType[] { 
            typeof(BucketBringer), 
            typeof(Target), 
            typeof(LineId), 
            typeof(LineIndex), 
            typeof(BucketId), 
            typeof(URPMaterialPropertyBaseColor) });
        var BucketFillerFetcherEntity = InitFireFighter(ref state, config, new ComponentType[] { 
            typeof(BucketFillerFetcher), 
            typeof(Target), 
            typeof(LineId), 
            typeof(BucketId), 
            typeof(URPMaterialPropertyBaseColor) });
        var WaterDumperEntity = InitFireFighter(ref state, config, new ComponentType[] { 
            typeof(WaterDumper), 
            typeof(Target), 
            typeof(LineId), 
            typeof(BucketId), 
            typeof(URPMaterialPropertyBaseColor) });

        state.EntityManager.Instantiate(WaterBringersEntity,        config.LinesCount * config.PerLinesCount, Allocator.Temp);
        state.EntityManager.Instantiate(BucketBringerEntity,        config.LinesCount * config.PerLinesCount, Allocator.Temp);
        state.EntityManager.Instantiate(BucketFillerFetcherEntity,  config.LinesCount                       , Allocator.Temp);
        state.EntityManager.Instantiate(WaterDumperEntity,          config.LinesCount                       , Allocator.Temp);

        state.EntityManager.DestroyEntity(WaterBringersEntity);
        state.EntityManager.DestroyEntity(BucketBringerEntity);
        state.EntityManager.DestroyEntity(BucketFillerFetcherEntity);
        state.EntityManager.DestroyEntity(WaterDumperEntity);

        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)(config.LinesCount * config.PerLinesCount));
        float gridRadius = config.GridSize * 0.5f * config.CellSize;
        foreach (var tran in SystemAPI.Query<RefRW<Translation>>().WithAny<WaterBringer, BucketBringer, BucketFillerFetcher>().WithAny<WaterDumper>())
        {
            tran.ValueRW.Value = new float3(rand.NextFloat(-gridRadius, gridRadius), 0, rand.NextFloat(-gridRadius, gridRadius));
        }

        int index = 0;
        foreach (var (color, lineId, LineIndex) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRW<LineId>, RefRW<LineIndex>>().WithAll<WaterBringer>())
        {
            color.ValueRW.Value = new float4(config.WaterBringersColor.r, config.WaterBringersColor.g, config.WaterBringersColor.b, 1.0f);

            lineId.ValueRW.Value = index / config.PerLinesCount;
            LineIndex.ValueRW.Value = index % config.PerLinesCount;
            ++index;
        }
        index = 0;
        foreach (var (color, lineId, LineIndex) in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRW<LineId>, RefRW<LineIndex>>().WithAll<BucketBringer>())
        {
            color.ValueRW.Value = new float4(config.BucketBringersColor.r, config.BucketBringersColor.g, config.BucketBringersColor.b, 1.0f);

            lineId.ValueRW.Value = index / config.PerLinesCount;
            LineIndex.ValueRW.Value = index % config.PerLinesCount;
            ++index;
        }
        index = 0;
        foreach (var (lineId, color) in SystemAPI.Query<RefRW<LineId>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<BucketFillerFetcher>())
        {
            color.ValueRW.Value = new float4(config.BucketFillerFetcherColor.r, config.BucketFillerFetcherColor.g, config.BucketFillerFetcherColor.b, 1.0f);

            lineId.ValueRW.Value = index / config.PerLinesCount;
            ++index;
        }
        index = 0;
        foreach (var (lineId, color) in SystemAPI.Query<RefRW<LineId>, RefRW<URPMaterialPropertyBaseColor>>().WithAll<WaterDumper>())
        {
            color.ValueRW.Value = new float4(config.WaterDumperColor.r, config.WaterDumperColor.g, config.WaterDumperColor.b, 1.0f);

            lineId.ValueRW.Value = index / config.PerLinesCount;
            ++index;
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}