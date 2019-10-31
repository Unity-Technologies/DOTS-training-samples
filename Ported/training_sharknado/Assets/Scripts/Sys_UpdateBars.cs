using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

public class Sys_UpdateBars : JobComponentSystem
{
    protected override JobHandle OnUpdate( JobHandle inputDeps )
    {
        return inputDeps;
    }
}