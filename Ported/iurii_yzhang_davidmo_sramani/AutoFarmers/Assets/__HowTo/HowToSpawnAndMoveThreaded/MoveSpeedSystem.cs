using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

public class MoveSpeedSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var dt = Time.deltaTime;
        return Entities.ForEach((ref MoveSpeed move, ref Translation translation) =>
        {
            translation.Value += move.Value * dt;
        }).Schedule(inputDeps);
    }   
} 
