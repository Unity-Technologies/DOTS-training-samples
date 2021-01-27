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

        // Warning - We disabled the safety check!
        // Two ants on the same position will not be able to write to the same index on the buffer
        Entities
            .WithAll<AntHeading>()
            .WithNativeDisableParallelForRestriction(pheromoneBuffer)
            .ForEach((Entity entity, in Translation translation) =>
            {
                int xIndex = (int)((translation.Value.x / tuning.WorldSize) + tuning.WorldOffset.x);
                int yIndex = (int)((translation.Value.y / tuning.WorldSize) + tuning.WorldOffset.y);
                int index = (int)math.clamp((yIndex * tuning.Resolution) + xIndex, 0, (tuning.Resolution * tuning.Resolution) - 1);

                // Set our randomly selected index value to something
                float currentAmount = math.max(pheromoneBuffer[index] + tuning.AntPheromoneStrength, 255);
                pheromoneBuffer[index] = (byte)currentAmount;// tuning.AntPheromoneStrength;
            }).ScheduleParallel();

    }
}
