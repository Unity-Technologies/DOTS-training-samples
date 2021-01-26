using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PheromoneRenderingSystem : SystemBase
{
    //OnCreate requirecomponent

    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
        DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

        var pheromoneRenderingRef = this.GetSingleton<GameObjectRefs>().PheromoneRenderingRef;

        pheromoneRenderingRef.SetPheromoneArray(pheromoneBuffer);
    }
}
