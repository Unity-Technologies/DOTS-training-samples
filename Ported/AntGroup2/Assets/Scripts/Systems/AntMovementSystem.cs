using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial struct AntMovementSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
    }


    public void OnDestroy(ref SystemState state)
    {
    }


    public void OnUpdate(ref SystemState state)
    {
        foreach (var (tranform, direction, target) in SystemAPI.Query<TransformAspect, RefRW<CurrentDirection>, TargetDirection>().WithAll<Ant>())
        {
            float2 normalizedDir = math.normalize(direction.ValueRO.Direction + target.Direction);
            tranform.WorldPosition += new float3(normalizedDir.x, 0, normalizedDir.y);
            direction.ValueRW.Direction = normalizedDir;
        }
    }
}
