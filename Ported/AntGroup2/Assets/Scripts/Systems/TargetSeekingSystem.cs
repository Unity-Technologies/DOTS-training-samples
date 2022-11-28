using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial struct TargetSeekingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var ant in SystemAPI.Query<TargetDirectionAspect, TransformAspect>().WithAll<Ant>())
        {
            float3 newDirection;
            newDirection = math.mul(float3x3.RotateY(0.3f),
                new float3(ant.Item1.Direction.x, 0, ant.Item1.Direction.y));

            ant.Item1.Direction = new float2(newDirection.x,newDirection.z);
        }
    }
}
