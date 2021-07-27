using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class BucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float3 one = new float3(1F, 1F, 1F);
        GameConfigComponent config = GetSingleton<GameConfigComponent>();
        float minBucketScale = config.MinBucketScale;
        float maxBucketScale = config.MaxBucketScale;
        Color32 empty = config.EmptyBucketColor;
        float4 emptyBucketColor = new float4(empty.r / 255F, empty.g / 255F, empty.b / 255F, 1F);
        Color32 full = config.FullBucketColor;
        float4 fullBucketColor = new float4(full.r / 255F, full.g / 255F, full.b / 255F, 1F);

        Entities.WithAll<BucketFullComponent>().ForEach((ref NonUniformScale scale, ref URPMaterialPropertyBaseColor col, 
            in WaterVolumeComponent v, in WaterCapacityComponent cap) =>
        {
            var ratio = v.Volume / cap.Capacity;
            scale.Value = math.lerp(minBucketScale, maxBucketScale, ratio) * one;
            col.Value = math.lerp(emptyBucketColor, fullBucketColor, ratio);
        }).Schedule();
    }
}
