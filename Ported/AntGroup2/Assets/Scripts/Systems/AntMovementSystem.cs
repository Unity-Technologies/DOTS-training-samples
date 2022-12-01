using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(AntSpawningSystem))]
partial struct AntMovementSystem : ISystem
{
    private Unity.Mathematics.Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Unity.Mathematics.Random(12345);   
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var job = new AntUpdate { config = config, seed = random.NextUInt() , deltaTime = SystemAPI.Time.DeltaTime };
        job.ScheduleParallel();
    }


    [BurstCompile]
    [WithAll(typeof(Ant))]
    public partial struct AntUpdate : IJobEntity
    {
        public Config config;
        public uint seed;
        public float deltaTime;

        public void Execute([EntityIndexInQuery]int entityIndex, DirectionAspect direction, in HasResource hasResource, TransformAspect transform)
        {
            var random = Unity.Mathematics.Random.CreateFromIndex(seed+(uint)entityIndex);
            
            float PheromoneWeight = config.MovePheromoneWeight;
            float WallWeight = config.MoveWallWeight;
            float TargetWeight = config.MoveTargetWeight;
            float RandomWeight = config.MoveRandomWeight;

            float antSpeed = 1.0f;
            float newDirection = 0;

            if (direction.WallBounceDirection != 0)
            {
                float2 forward = new float2(transform.Forward.x, transform.Forward.z);
                float2 wallNormal = math.normalize(new float2(transform.WorldPosition.x, transform.WorldPosition.z)) * direction.WallBounceDirection;
                float2 reflect = math.reflect(forward, wallNormal);
                newDirection = math.atan2(reflect.x, reflect.y);
            }
            else if (hasResource.Trigger)
            {
                newDirection = direction.CurrentDirection + math.PI;
            }
            else
            {
                float randomDir = random.NextFloat(-config.RandomSteeringAmount, config.RandomSteeringAmount);
                if (true)
                {
                    float3 dir = new float3(math.sin(direction.CurrentDirection + direction.WallDirection), 0,
                       math.cos(direction.CurrentDirection + direction.WallDirection));
                    
                    //Debug.DrawLine(transform.WorldPosition, transform.WorldPosition + dir * 3.0f, Color.yellow);
                    
                    
                    
                    //dir = new float3(math.sin(direction.CurrentDirection + direction.TargetDirection), 0,
                    //    math.cos(direction.CurrentDirection + direction.TargetDirection));
                    //Debug.DrawLine(transform.WorldPosition, transform.WorldPosition + dir * 3.0f, Color.green);
                    
                    dir = new float3(math.sin(direction.CurrentDirection + direction.PheromoneDirection), 0,
                        math.cos(direction.CurrentDirection + direction.PheromoneDirection));
                    Debug.DrawLine(transform.WorldPosition, transform.WorldPosition + dir * 3.0f, Color.blue);
                    
                    //dir = new float3(math.sin(direction.CurrentDirection + randomDir), 0,
                    //    math.cos(direction.CurrentDirection + randomDir));
                    //Debug.DrawLine(transform.WorldPosition, transform.WorldPosition + dir * 3.0f, Color.red);
                    
                    
                }
                
                
                newDirection += direction.WallDirection * WallWeight;
                newDirection += direction.TargetDirection * TargetWeight;
                newDirection += direction.PheromoneDirection * PheromoneWeight;
                newDirection += randomDir *
                                RandomWeight;

                newDirection /= PheromoneWeight + WallWeight + TargetWeight + RandomWeight;
                newDirection = math.clamp(newDirection, -math.PI / 2.0f, math.PI / 2.0f);

                antSpeed -= math.abs(newDirection / (math.PI / 2.0f));
                newDirection += direction.CurrentDirection;

            }

            float2 normalizedDir = new float2(math.sin(newDirection), math.cos(newDirection));
            normalizedDir *= config.TimeScale * deltaTime * antSpeed;

            transform.WorldPosition += new float3(normalizedDir.x, 0, normalizedDir.y);
            transform.LookAt(transform.WorldPosition + new float3(normalizedDir.x, 0, normalizedDir.y));

            direction.PreviousDirection = direction.CurrentDirection;
            direction.CurrentDirection = math.atan2(transform.Forward.x, transform.Forward.z);
            //if (direction.CurrentDirection > math.PI * 2.0f)
            //    direction.CurrentDirection -= (float)(math.PI * 2.0f);
            //if (direction.CurrentDirection < 0)
            //    direction.CurrentDirection += (float)(math.PI * 2.0f);
            
            float3 disr = new float3(math.sin(direction.CurrentDirection), 0,
                math.cos(direction.CurrentDirection));
            Debug.DrawLine(transform.WorldPosition, transform.WorldPosition + disr * 3.0f, Color.magenta);

            direction.WallDirection = 0;
            direction.TargetDirection = 0;
            direction.PheromoneDirection = 0;
        }
    }
}