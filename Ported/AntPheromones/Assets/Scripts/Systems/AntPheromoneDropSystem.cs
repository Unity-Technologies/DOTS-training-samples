using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;
using Unity.Transforms;

public class AntPheromoneDropSystem : SystemBase
{
    float _timeElapsed = 0;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<PheromoneStrength>();
        RequireSingletonForUpdate<Tuning>();
        RequireSingletonForUpdate<GameTime>();
    }

    protected override void OnUpdate()
    {
        _timeElapsed += GetSingleton<GameTime>().DeltaTime;;

        var tuning = GetSingleton<Tuning>();

        if (_timeElapsed < tuning.PheromoneDropPeriod)
        {
            return;
        }

        _timeElapsed = 0;

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
                int xIndex = (int)math.floor(((translation.Value.x / tuning.WorldSize) + tuning.WorldOffset.x));
                int yIndex = (int)math.ceil(((translation.Value.y / tuning.WorldSize) + tuning.WorldOffset.y));
                int index = (int)math.clamp((yIndex * tuning.Resolution) + xIndex, 0, (tuning.Resolution * tuning.Resolution) - 1);

                // Set our randomly selected index value to something 
                int pVal = pheromoneBuffer[index].Value;
                int newValue = pVal + tuning.PheromoneDropValue;
                newValue = Math.Min(newValue, 255);
                pheromoneBuffer[index] = (byte)newValue;
            }).ScheduleParallel();

    }
}
