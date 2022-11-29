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
            float newDirection = ant.CurrentDirection;
            
            newDirection += ant.TargetDirection;
            newDirection += ant.WallDirection;
            newDirection += ant.PheromoneDirection;
            newDirection += random.NextFloat(-config.RandomSteeringAmount, config.RandomSteeringAmount);

            float2 normalizedDir = new float2(math.sin(newDirection), math.cos(newDirection));
            normalizedDir *= config.TimeScale * SystemAPI.Time.DeltaTime;
            transform.WorldPosition += new float3(normalizedDir.x, 0, normalizedDir.y);
            Quaternion rotation = quaternion.RotateY(newDirection);
            transform.WorldRotation = rotation;
            ant.CurrentDirection = newDirection;
        }
    }
}
