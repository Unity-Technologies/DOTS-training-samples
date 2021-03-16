using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class HeatRenderingSystem : SystemBase
{
    private static readonly float4 RED = new float4(1.0f, 0.0f, 0.0f, 1.0f);
    private static readonly float4 GREEN = new float4(0.0f, 1.0f, 0.0f, 1.0f);
        
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<HeatMapTag>();
    }

    protected override void OnUpdate()
    {
        Entity heatMapEntity = GetSingletonEntity<HeatMapTag>();
        DynamicBuffer<HeatMap> heatMap = GetBuffer<HeatMap>(heatMapEntity);
        int counter = 0;
        
        Entities
            .WithAll<Heat>()
            .WithReadOnly(heatMap)
            .ForEach((Entity entity, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color) =>
            {
                var heat = heatMap[counter].Value;

                scale.Value.y =  heat + 0.01f;
                color = new URPMaterialPropertyBaseColor { Value = math.lerp(GREEN, RED, heat)};

                counter++;
            }).Run();
    }
}