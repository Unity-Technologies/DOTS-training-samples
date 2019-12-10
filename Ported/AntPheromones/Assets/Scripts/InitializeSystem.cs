using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class InitializeSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        inputDeps.Complete();

        AntSettings settings = GetSingleton<AntSettings>();

        return new JobHandle();
    }

}
