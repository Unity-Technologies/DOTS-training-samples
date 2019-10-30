using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

public class IdleSystem : JobComponentSystem
{
    protected override void OnCreate()
    {
        
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        return Entities.WithoutBurst()
            .ForEach((ref Velocity velocity, ref State state, in Aggressiveness aggressiveness, in Translation translation) =>
            {
                if (state.Value == State.StateType.Idle)
                {
                    if (((aggressiveness.Value)/100f)*2f  > noise.cnoise(translation.Value))
                    {
                        state.Value = State.StateType.Chasing;
                    }
                    else
                    {
                        state.Value = State.StateType.Collecting;
                    }
                }
                Debug.Log(noise.cnoise(translation.Value));
            })
            .Schedule(inputDependencies);
    }
}