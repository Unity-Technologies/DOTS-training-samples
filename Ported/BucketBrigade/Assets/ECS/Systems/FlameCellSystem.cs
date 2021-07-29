using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class FlameCellSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<HeatMapElement>();
    }

    protected override void OnUpdate()
    {
        float3 one = new float3(1F, 1F, 1F);
        GameConfigComponent config = GetSingleton<GameConfigComponent>();
        var flashpoint = config.FlashPoint;
        float maxScale = config.FlameScaleMax;
        Color _defaultColor = config.FlameDefaultColor;
        Color _coldColor = config.FlameColdColor;
        Color _flameColor = config.FlameBurnColor;
        float4 defultColor = new float4(_defaultColor.r, _defaultColor.g, _defaultColor.b, 1f);
        float4 coldColor = new float4(_coldColor.r, _coldColor.g, _coldColor.b, 1f);
        float4 flameColor = new float4(_flameColor.r, _flameColor.g, _flameColor.b, 1f);
        Entity e =  GetSingletonEntity<HeatMapElement>();
        var buffer = GetBuffer<HeatMapElement>(e);
        Entities.WithReadOnly(buffer).ForEach((ref NonUniformScale scale, ref URPMaterialPropertyBaseColor col, in HeatMapIndex index, in FlameCellTagComponent tag 
            ) =>
        {
            var t = buffer[index.index].temperature;
            var temp_s = scale.Value;
            if (t >= flashpoint)
            {
                var relativeBurn = (t - flashpoint) / (1f - flashpoint);
                var y = math.lerp(0, maxScale, relativeBurn);
                temp_s.y = y;
                col.Value = math.lerp(coldColor, flameColor, relativeBurn);
            }
            else
            {
                col.Value = defultColor;
                temp_s.y = 0.05f;
            }
            scale.Value = temp_s;
            
            
            
        }).Schedule();
    }
}
