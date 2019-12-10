using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(ConvertToEntitySystem))]
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
