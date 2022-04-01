using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

//Reads the Heatmap Buffer to Update all Flame entities visuals (Color and 'Scale')
public partial class FireRenderingSystem : SystemBase
{
  const float flickerRate = 2f;
  const float flickerRange = 0.1f;
  protected override void OnUpdate()
  {
    var heatmapData = GetSingleton<HeatMapData>(); 
    var heatmap = GetSingletonEntity<HeatMapTemperature>();
    
    float time = (float) Time.ElapsedTime;
    float tileHalfHeight = heatmapData.maxTileHeight * 0.5f;
        
    var heatmapBufferReadonly = EntityManager.GetBuffer<HeatMapTemperature>(heatmap, true);

    Entities
      .WithAll<FireIndex>()
      .WithReadOnly(heatmapBufferReadonly)
      .ForEach( (ref URPMaterialPropertyBaseColor colorComponent, ref Translation translation, in FireIndex fireIndex) =>
      {
        float intensity = heatmapBufferReadonly[fireIndex.index];

        if (intensity < FirePropagationSystem.HEAT_THRESHOLD)
        {
          colorComponent.Value = heatmapData.colorNeutral;
          translation.Value.y = -tileHalfHeight;
        }
        else
        {
          float colorIntensity = (intensity * 1.25f - 0.2f);
          
          colorComponent.Value = math.lerp(heatmapData.colorCool, heatmapData.colorHot, colorIntensity );
          translation.Value.y = math.lerp(-tileHalfHeight, tileHalfHeight, intensity);
                     
          //Apply flicker noise
          float2 sample = new float2((time - fireIndex.index) * flickerRate - colorIntensity, colorIntensity);
          translation.Value.y += flickerRange * 0.5f + (noise.cnoise(sample) * flickerRange);
        }

      })
      .ScheduleParallel();
  }
}
