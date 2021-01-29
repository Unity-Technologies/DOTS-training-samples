using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class PheromoneRenderingSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<PheromoneStrength>();
    }

    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
        DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

        var pheromoneRenderingRef = this.GetSingleton<GameObjectRefs>().PheromoneRenderingRef;

        pheromoneRenderingRef.SetPheromoneArray(pheromoneBuffer);
    }
}
