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

public class BucketVisualStateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach( (Entity Entities, ref NonUniformScale bucketScale, ref Translation t, ref Color col,  in Intensity bucketCurrentVolume, in Bucket bucketTag, in CarryableObject carryInfo) =>
        {
            bucketScale.Value = new float3( bucketCurrentVolume.Value, bucketCurrentVolume.Value, bucketCurrentVolume.Value) / 3.0f * 0.25f + new float3(0.2f,0.2f,0.2f);

            Color emptyColor = new Color { Value = new float4(1.0f, 0.0f, 0.0f, 1.0f) };
            //Color fullColor = new Color { Value = new float4(1.0f, 1.0f, 1.0f, 1.0f) };
            Color fullColor = new Color { Value = new float4(0.0f, 0.62f, 1.0f, 1.0f) };

            col.Value = math.lerp(emptyColor.Value, fullColor.Value, bucketCurrentVolume.Value / 3.0f);

            if (carryInfo.CarryingEntity == Entity.Null)
            {
                t.Value = new float3(t.Value.x, 0.75f, t.Value.z);
            }
        }).ScheduleParallel();
    }
}
