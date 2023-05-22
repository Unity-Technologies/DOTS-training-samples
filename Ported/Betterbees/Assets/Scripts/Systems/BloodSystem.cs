using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct BloodSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {



    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // falling down
        // shrinking over time
        // destroy after time
    }
}
