﻿using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BucketVisualSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!HasSingleton<TuningData>())
        {
            return;
        }
        var tuningData = GetSingleton<TuningData>();
 

        Entities.WithAll<Bucket>().WithChangeFilter<ValueComponent>()
            .WithName("Bucket_Visuals").ForEach((ref FireMaterialComponent mat, ref NonUniformScale scale, in ValueComponent val) =>
            {
                scale.Value.x = (1 + val.Value / tuningData.BucketCapacity ) * tuningData.BucketScale;
                scale.Value.y = (1 + val.Value / tuningData.BucketCapacity ) * tuningData.BucketScale;
                scale.Value.z = (1 + val.Value / tuningData.BucketCapacity ) * tuningData.BucketScale;
                mat.Amount =  val.Value / tuningData.BucketCapacity;

            }).ScheduleParallel();


    }
}