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
        foreach (var ant in SystemAPI.Query<TransformAspect, TargetDirection>().WithAll<Ant>())
        {
            float2 normalizedDir = ant.Item2.Direction;
            Debug.Log(normalizedDir);
            ant.Item1.WorldPosition += new float3(normalizedDir.x, 0, normalizedDir.y);
        }
    }
}
