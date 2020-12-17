using System;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;

public struct BucketEmptyBot : IComponentData
{
    public int Index;
    public float2 Position;
}

public struct BucketFullBot : IComponentData
{
    public int Index;
    public float2 Position;
}

public class BucketTeamSystem : SystemBase
{
    protected override void OnUpdate()
    {
        NativeArray<float2> throwerCoords = new NativeArray<float2>(FireSimConfig.maxTeams, Allocator.TempJob);
        NativeArray<float2> fetcherCoords = new NativeArray<float2>(FireSimConfig.maxTeams, Allocator.TempJob);

        Entities.WithAll<Fetcher>().ForEach((in Position position, in TeamIndex teamIndex) =>
        {
            fetcherCoords[teamIndex.Value] = position.coord;
        }).Run();

        Entities.WithAll<Thrower>().ForEach((in Thrower thrower, in TeamIndex teamIndex) =>
        {
            throwerCoords[teamIndex.Value] = thrower.GridPosition;
        }).Run();

        float rMaxBucketEmpty = 1.0f / (1.0f+FireSimConfig.numEmptyBots);
        float rMaxBucketFull = 1.0f / (1.0f+FireSimConfig.numFullBots);

#if false
        JobHandle dep0 = Entities
            .WithReadOnly(throwerCoords)
            .WithReadOnly(fetcherCoords)
            .ForEach((ref Translation translation, in BucketEmptyBot bucketEmptyBot, in TeamIndex teamIndex) =>
            {
                float2 end = throwerCoords[teamIndex.Value];
                float2 start = fetcherCoords[teamIndex.Value];
                float t = (1.0f + bucketEmptyBot.Index) * rMaxBucketEmpty;
                float2 pos = start + (end - start) * t;

                translation.Value = new float3(pos.x, 1.0f, pos.y);
            }).Schedule(Dependency);

        JobHandle dep1 = Entities
            .WithReadOnly(throwerCoords)
            .WithReadOnly(fetcherCoords)
            .ForEach((ref Translation translation, in BucketFullBot bucketFullBot, in TeamIndex teamIndex) =>
            {
                float2 end = throwerCoords[teamIndex.Value];
                float2 start = fetcherCoords[teamIndex.Value];
                float t = (1.0f + bucketFullBot.Index) * rMaxBucketFull;
                float2 pos = start + (end - start) * t;

                translation.Value = new float3(pos.x, 1.0f, pos.y);
            }).Schedule(Dependency);
        Dependency = JobHandle.CombineDependencies(dep0, dep1);
#else
        Entities
            .WithReadOnly(throwerCoords)
            .WithReadOnly(fetcherCoords)
            .ForEach((ref Translation translation, in BucketEmptyBot bucketEmptyBot, in TeamIndex teamIndex) =>
            {
                float2 end = throwerCoords[teamIndex.Value];
                float2 start = fetcherCoords[teamIndex.Value];
                float t = (1.0f + bucketEmptyBot.Index) * rMaxBucketEmpty;
                float2 pos = start + (end - start) * t;

                translation.Value = new float3(pos.x, 1.0f, pos.y);
            }).Schedule();

        Entities
            .WithReadOnly(throwerCoords)
            .WithReadOnly(fetcherCoords)
            .ForEach((ref Translation translation, in BucketFullBot bucketFullBot, in TeamIndex teamIndex) =>
            {
                float2 end = throwerCoords[teamIndex.Value];
                float2 start = fetcherCoords[teamIndex.Value];
                float t = (1.0f + bucketFullBot.Index) * rMaxBucketFull;
                float2 pos = start + (end - start) * t;

                translation.Value = new float3(pos.x, 1.0f, pos.y);
            }).Schedule();
#endif

        throwerCoords.Dispose(Dependency);
        fetcherCoords.Dispose(Dependency);
    }
}
