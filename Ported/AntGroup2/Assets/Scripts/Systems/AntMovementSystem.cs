using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(AntSpawningSystem))]
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
        foreach (var (ant, tranform) in SystemAPI.Query<DirectionAspect, TransformAspect>().WithAll<Ant>())
        {
            float newDirection = ant.CurrentDirection;
            
            newDirection += ant.TargetDirection;
            newDirection += ant.WallDirection;
            newDirection += ant.PheromoneDirection;

            float2 normalizedDir = new float2(math.sin(newDirection), math.cos(newDirection));
            tranform.WorldPosition += new float3(normalizedDir.x, 0, normalizedDir.y);
            ant.CurrentDirection = newDirection;
        }
    }
}
