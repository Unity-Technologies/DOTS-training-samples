using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;
using Unity.Transforms;

public class AntPheromoneDropSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<PheromoneStrength>();
        RequireSingletonForUpdate<Tuning>();
    }

    protected override void OnUpdate()
    {
        Tuning tuning = this.GetSingleton<Tuning>();

        Entity pheromoneEntity = GetSingletonEntity<PheromoneStrength>();

        DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

        var pheromoneRenderingRef = this.GetSingleton<GameObjectRefs>().PheromoneRenderingRef;

        Entities
            .WithAll<AntHeading>()
            .ForEach((Entity entity, in Translation translation) =>
            {
                int xIndex = (int)((translation.Value.x / tuning.WorldSize) + tuning.WorldOffset.x);
                int yIndex = (int)((translation.Value.y / tuning.WorldSize) + tuning.WorldOffset.y);
                int index = (int)math.clamp((yIndex * tuning.Resolution) + xIndex, 0, (tuning.Resolution * tuning.Resolution));

                // Set our randomly selected index value to something
                pheromoneBuffer[index] = 255f;
            }).Schedule();

    }
}
