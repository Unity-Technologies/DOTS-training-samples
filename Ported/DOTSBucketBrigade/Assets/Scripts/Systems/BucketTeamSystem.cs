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

public struct SplayParams
{
    public float SplayMin;
    public float SplayMax;
    public float SplayStart;
    public float SplayEnd;
};

[BurstCompile]
public class BucketTeamSystem : SystemBase
{
    [BurstCompile]
    static void UpdateTranslation(ref Translation translation, float2 start, float2 end, float t, float bias, SplayParams splayParams)
    {
        float splayMin = splayParams.SplayMin;
        float splayMax = splayParams.SplayMax;
        float splayStart = splayParams.SplayStart;
        float splayEnd = splayParams.SplayEnd;

        float2 zdir2d = end - start;
        float2 pos = start + (end - start) * t;

        float zlen = math.length(zdir2d);
        if (zlen > math.EPSILON)
        {
            float splayRangeLinear0 = (zlen - splayStart) / splayEnd;
            float splayRangeLinear1 = math.min(splayRangeLinear0, 1.0f);
            float splayRangeLinear2 = math.max(splayRangeLinear1, 0.0f);
            float splayFactor = splayRangeLinear2 * (splayMax - splayMin) + splayMin;

            float2 zdir2dn = math.normalize(end - start);
            float3 zdir = new float3(zdir2dn.x, 0, zdir2dn.y);
            float3 ydir = new float3(0,1,0);
            float3 xdir = math.cross(ydir, zdir);
            float2 xdir2d = new float2(xdir.x, xdir.z);

            pos += bias * splayFactor * xdir2d * math.sin(math.PI * t);
        }

        translation.Value = new float3(pos.x, 1.0f, pos.y);
    }

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
        var componentTypeHandle = GetComponentTypeHandle<Translation>();

        JobHandle dep0 = Entities
            .WithReadOnly(throwerCoords)
            .WithReadOnly(fetcherCoords)
            .WithNativeDisableContainerSafetyRestriction(componentTypeHandle)
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
            .WithNativeDisableContainerSafetyRestriction(componentTypeHandle)
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

        SplayParams splayParams = new SplayParams
        {
            SplayMin = VisualConfig.kSplayMin,
            SplayMax = VisualConfig.kSplayMax,
            SplayStart = VisualConfig.kSplayStart,
            SplayEnd = VisualConfig.kSplayEnd
        };

        Entities
            .WithReadOnly(throwerCoords)
            .WithReadOnly(fetcherCoords)
            .ForEach((ref Translation translation, in BucketEmptyBot bucketEmptyBot, in TeamIndex teamIndex) =>
            {
                float2 end = throwerCoords[teamIndex.Value];
                float2 start = fetcherCoords[teamIndex.Value];
                float t = (1.0f + bucketEmptyBot.Index) * rMaxBucketEmpty;

                UpdateTranslation(ref translation, start, end, t, 1.0f, splayParams);
            }).Schedule();

        Entities
            .WithReadOnly(throwerCoords)
            .WithReadOnly(fetcherCoords)
            .ForEach((ref Translation translation, in BucketFullBot bucketFullBot, in TeamIndex teamIndex) =>
            {
                float2 end = throwerCoords[teamIndex.Value];
                float2 start = fetcherCoords[teamIndex.Value];
                float t = (1.0f + bucketFullBot.Index) * rMaxBucketFull;
                UpdateTranslation(ref translation, start, end, t, -1.0f, splayParams);
            }).Schedule();
#endif

        throwerCoords.Dispose(Dependency);
        fetcherCoords.Dispose(Dependency);
    }
}
