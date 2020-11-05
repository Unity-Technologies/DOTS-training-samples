using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class TexUpdaterSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TexSingleton>();
        GetEntityQuery(typeof(PheromonesBufferData));
    }
    
    protected override void OnUpdate()
    {
        int TexSize = RefsAuthoring.TexSize;

        var pheromoneDataEntity = GetSingletonEntity<TexSingleton>();
        var pheromonesBuffer = GetBuffer<PheromonesBufferData>(pheromoneDataEntity);

        for (int i = 0; i < pheromonesBuffer.Length; ++i)
        {
            UpdatePheromoneSystem.colors[i].r = pheromonesBuffer[i];
            UpdatePheromoneSystem.colors[i].g = 0f;
            UpdatePheromoneSystem.colors[i].b = 0f;
        }
        
        var map = this.GetSingleton<Refs>().PheromoneMap;
        map.SetPixels(0, 0, TexSize, TexSize, UpdatePheromoneSystem.colors);
        map.Apply();
    }
}
