using System;
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
    private Unity.Mathematics.Random random;

    public void OnCreate(ref SystemState state)
    {
        random = new Unity.Mathematics.Random(12345);
    }


    public void OnDestroy(ref SystemState state)
    {
    }


    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        foreach (var (ant, transform) in SystemAPI.Query<DirectionAspect, TransformAspect>().WithAll<Ant>())
        {
            float pheromoneWeight = 0.0f;
            float WallWeight = 1.0f;
            float TargetWeight = 0.0f;
            float RandomWeight = .8f;
            
            
            float newDirection = 0;

            if (ant.WallBounce)
                newDirection = ant.CurrentDirection;
            else
            {
                newDirection += ant.WallDirection * WallWeight;
                newDirection += ant.TargetDirection * TargetWeight;
                newDirection += ant.PheromoneDirection * pheromoneWeight;
                newDirection += random.NextFloat(-config.RandomSteeringAmount, config.RandomSteeringAmount) * RandomWeight;

                newDirection /= pheromoneWeight + WallWeight + TargetWeight + RandomWeight;
            }

            newDirection += ant.CurrentDirection;

            float2 normalizedDir = new float2(math.sin(newDirection), math.cos(newDirection));
            normalizedDir *= config.TimeScale * SystemAPI.Time.DeltaTime;
            transform.WorldPosition += new float3(normalizedDir.x, 0, normalizedDir.y);
            Quaternion rotation = quaternion.RotateY(newDirection);
            transform.WorldRotation = rotation;
            ant.CurrentDirection = newDirection;
            if (ant.CurrentDirection > Math.PI * 2.0f)
                ant.CurrentDirection -= (float)(Math.PI * 2.0f);
            if (ant.CurrentDirection < 0)
                ant.CurrentDirection -= (float)(math.PI * 2.0f);
            
            ant.WallDirection = 0;
        }
    }
}
