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
            //Checks if there is anything between the ant and the goal
            if(!Physics.Linecast(ant.Item2.WorldPosition, new float3(7.4f, 0f, 10f)))
            {
                float angle = Vector3.Angle(new float3(7.4f, 0f, 10f) - ant.Item2.WorldPosition, Vector3.right);
                Debug.Log(angle);
                float3 newDirection;
                newDirection = math.mul(float3x3.RotateY(angle),
                    new float3(ant.Item1.Direction.x, 0, ant.Item1.Direction.y));

                ant.Item1.Direction = new float2(newDirection.x, newDirection.z);
            }
            
            
        }
    }
}
