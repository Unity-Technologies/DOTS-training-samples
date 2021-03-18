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

    private static readonly float4 CELL_NEUTRAL_COLOUR = new float4(0.4888542f, 0.9884242f, 0.514151f, 1.0f);
    private static readonly float4 FIRE_WARM_COLOUR = new float4(1.0f, 0.9884242f, 0.514151f, 1.0f);
    private static readonly float4 FIRE_HOT_COLOUR = new float4(1.0f, 0.0f, 0.0f, 1.0f);
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<HeatMapTag>();
    }

    protected override void OnUpdate()
    {
        Entity heatMapEntity = GetSingletonEntity<HeatMapTag>();
        DynamicBuffer<HeatMap> heatMap = GetBuffer<HeatMap>(heatMapEntity);
        int counter = 0;
        float flashPoint = 0.2f;

        Entities
            .WithAll<Cell>()
            .WithReadOnly(heatMap)
            .ForEach((Entity entity, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color) =>
            {
                var heat = heatMap[counter].Value;
                
                if (heat < flashPoint)
                {
                    color = new URPMaterialPropertyBaseColor { Value = CELL_NEUTRAL_COLOUR };
                }
                else if (heat == flashPoint) 
                {
                    scale.Value.y = heat + 0.01f;
                    color = new URPMaterialPropertyBaseColor { Value = FIRE_WARM_COLOUR };
                }
                else if (heat > flashPoint && heat <= 1)
                {
                    scale.Value.y = heat + 0.01f;
                    color = new URPMaterialPropertyBaseColor { Value = math.lerp(FIRE_WARM_COLOUR, FIRE_HOT_COLOUR, heat) };
                }

                counter++;
            }).Run();

    }
}