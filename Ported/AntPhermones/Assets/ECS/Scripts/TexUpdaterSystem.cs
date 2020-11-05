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
    Color[] colorCache = new Color[RefsAuthoring.TexSize * RefsAuthoring.TexSize];

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<TexSingleton>();
        GetEntityQuery(typeof(PheromonesBufferData));

        for (int i = 0; i < colorCache.Length; ++i)
        {
            colorCache[i] = new Color(0, 0, 0, 0);
        }
    }
    
    protected override void OnUpdate()
    {
        var pheromoneDataEntity = GetSingletonEntity<TexSingleton>();
        var pheromonesBuffer = GetBuffer<PheromonesBufferData>(pheromoneDataEntity);
        
        var map = this.GetSingleton<Refs>().PheromoneMap;
        map.SetPixelData(pheromonesBuffer.AsNativeArray(), 0);
        map.Apply();
    }
}
