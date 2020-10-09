using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(AgentUpdateSystem))]
public class BucketVisualStateSystem : SystemBase
{
    private static readonly Color k_emptyColor = new Color { Value = new float4(1.0f, 0.0f, 0.0f, 1.0f) };
    private static readonly Color k_fullColor = new Color { Value = new float4(0.5f, 0.9f, 1.0f, 1.0f) };
    
    protected override void OnUpdate()
    {
        Entities.ForEach((ref NonUniformScale bucketScale, ref Translation t, ref Color col, in Intensity bucketCurrentVolume, in Bucket bucketTag, in CarryableObject carryInfo) =>
        {
            col.Value = math.lerp(k_emptyColor.Value, k_fullColor.Value, bucketCurrentVolume.Value / Bucket.MaxVolume);
            bucketScale.Value = new float3(bucketCurrentVolume.Value, bucketCurrentVolume.Value, bucketCurrentVolume.Value) / Bucket.MaxVolume * 0.25f + new float3(0.2f,0.2f,0.2f);
            if (carryInfo.CarryingEntity == Entity.Null)
            {
                t.Value = new float3(t.Value.x, bucketScale.Value.y / 2.0f, t.Value.z);
            }
        }).ScheduleParallel(); // smoother with run visually
    }
}
