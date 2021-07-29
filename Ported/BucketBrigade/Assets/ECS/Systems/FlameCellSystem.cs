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
        float4 defaultColor = ColorToFloat4.Cast(config.FlameDefaultColor);
        float4 coldColor = ColorToFloat4.Cast(config.FlameColdColor);
        float4 flameColor = ColorToFloat4.Cast(config.FlameBurnColor);
        Entity e =  GetSingletonEntity<HeatMapElement>();
        var buffer = GetBuffer<HeatMapElement>(e);
        var time = UnityEngine.Time.time;
        var flickerRate = config.FlameFlickerRate;
        var flickerRange = config.FlameFlickerRange;

        Entities.WithReadOnly(buffer).ForEach((ref NonUniformScale scale, ref URPMaterialPropertyBaseColor col, in HeatMapIndex index, in FlameCellTagComponent tag 
            ) =>
        {
            var t = buffer[index.index].temperature;
            var temp_s = scale.Value;
            if (t >= flashpoint)
            {
                var relativeBurn = (t - flashpoint) / (1f - flashpoint);
                var height = math.lerp(0, maxScale, relativeBurn);
                var flicker = Mathf.PerlinNoise((time - index.index) * flickerRate - t, t) * flickerRange; 
                temp_s.y = height + flicker;
                col.Value = math.lerp(coldColor, flameColor, relativeBurn);
            }
            else
            {
                col.Value = defaultColor;
                temp_s.y = 0.05f;
            }
            scale.Value = temp_s;
        }).ScheduleParallel();
    }
}
