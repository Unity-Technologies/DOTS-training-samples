using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

public class AntPheromoneDropSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<PheromoneStrength>();
    }

    protected override void OnUpdate()
    {
        Entity pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
        DynamicBuffer<PheromoneStrength> pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);

        var random = new Unity.Mathematics.Random(math.max((uint)DateTime.Now.Millisecond, 1) * 1000); //remove the * 1000

        Entities
            .WithAll<AntHeading>()
            .ForEach((Entity entity) =>
            {
                // Pick a random index (this will eventually be the index associated with the ant's position)
                int randomIndex = random.NextInt(0, pheromoneBuffer.Length);

                // Set our randomly selected index value to something
                pheromoneBuffer[randomIndex] = 255f;
            }).Schedule();

    }
}
