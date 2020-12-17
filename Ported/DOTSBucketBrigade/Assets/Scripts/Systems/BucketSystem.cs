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
            .ForEach((ref Position position, ref Bucket bucket, in BucketOwner bucketOwner, in WaterLevel waterLevel) =>
            {
                if (bucketOwner.IsAssigned())
                {
                    int teamIndex = bucketOwner.AsCohortIndex();
                    float bias = waterLevel.Value < 0.5f ? 1.0f : -1.0f; // empty buckets carried by empty bot who is biased to +1

                    Translation translation;
                    BucketTeamSystem.UpdateTranslation(out translation, fetcherCoords[teamIndex], throwerCoords[teamIndex], bucket.LinearT, bias, splayParams);

                    // jiv fixme: split this into separate system so we can handle t >= 1.0f
                    // jiv fixme: we shouldn't be progressing in linear t time, because t interpolates the line length
                    //            and this will result in higher velocity for longer lines.  We should progress in constant
                    //            arc length.
                    bucket.LinearT += currentDeltaTime * bucketSpeed;
                    position.coord = new float2(translation.Value.x, translation.Value.z);
                }
            }).Schedule();

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
