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

[BurstCompile]
public class BucketSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<Fetcher>()));
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<Thrower>()));
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<Bucket>()));
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

        SplayParams splayParams = new SplayParams
        {
            SplayMin   = VisualConfig.kSplayMin,
            SplayMax   = VisualConfig.kSplayMax,
            SplayStart = VisualConfig.kSplayStart,
            SplayEnd   = VisualConfig.kSplayEnd
        };

		float currentDeltaTime = Time.DeltaTime;
        float bucketSpeed = FireSimConfig.kBucketSpeed;

        Entities
            .WithNativeDisableContainerSafetyRestriction(fetcherCoords)
            .WithNativeDisableContainerSafetyRestriction(throwerCoords)
            .ForEach((ref Position position, ref Bucket bucket, in BucketOwner bucketOwner, in WaterLevel waterLevel) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    int teamIndex = bucketOwner.AsCohortIndex();
                    float bias = waterLevel.Value < 0.5f ? 1.0f : -1.0f; // empty buckets carried by empty bot who is biased to +1
                    float2 dir = throwerCoords[teamIndex] - fetcherCoords[teamIndex];

                    Translation translation;
                    BucketTeamSystem.UpdateTranslation(out translation, fetcherCoords[teamIndex], throwerCoords[teamIndex], dir, bucket.LinearT, bias, splayParams);
                    position.coord = new float2(translation.Value.x, translation.Value.z);
                }
            }).ScheduleParallel();

        Entities
            .ForEach((ref Bucket bucket, in BucketOwner bucketOwner, in WaterLevel waterLevel) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    float direction = waterLevel.Value < 0.5f ? -1.0f : 1.0f; // full -> forward, empty -> back
                    bucket.LinearT += direction * currentDeltaTime * bucketSpeed;
                }
            }).ScheduleParallel();

        Entities
            .ForEach((ref Bucket bucket, ref BucketOwner bucketOwner) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    if (bucket.LinearT < 0.0f || bucket.LinearT > 1.0f)
                        bucketOwner.Value = 0;
                }
            }).ScheduleParallel();

        Entities
            .WithAll<Bucket>()
            .ForEach((ref Translation translation, in Position position) =>
            {
                translation.Value = new float3(position.coord.x, 1.0f, position.coord.y);
            }).Schedule();

        Dependency = fetcherCoords.Dispose(Dependency);
        Dependency = throwerCoords.Dispose(Dependency);
    }
}
