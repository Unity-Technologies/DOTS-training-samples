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
    }
    
    protected override void OnUpdate()
    {
        int TexSize = RefsAuthoring.TexSize;

        UpdatePheromoneSystem.LastJob.Complete();

        Entities
        .ForEach((Refs map) =>
        {
            map.PheromoneMap.SetPixels(0, 0, TexSize, TexSize, UpdatePheromoneSystem.colors);
            map.PheromoneMap.Apply();
        })
        .WithoutBurst()
        .Run();
    }

    
    
}
