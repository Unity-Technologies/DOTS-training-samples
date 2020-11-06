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
        var pheromoneDataEntity = GetSingletonEntity<TexSingleton>();
        var pheromonesBuffer = GetBuffer<PheromonesBufferData>(pheromoneDataEntity);
        
        var map = this.GetSingleton<Refs>().PheromoneMap;
        map.SetPixelData(pheromonesBuffer.AsNativeArray(), 0);
        map.Apply();
    }
}
