using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor.Overlays;

partial struct WallAvoidanceSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var ant in SystemAPI.Query<DirectionAspect, TransformAspect>().WithAll<Ant>())
        {
            bool wallBounce = false;
            foreach (var wall in SystemAPI.Query<TransformAspect>().WithAll<Obstacle>())
            {
                for (float antStepSize = 0.5f; antStepSize < 3.0f; antStepSize += 0.5f)
                {
                    float3 antStep = ant.Item2.WorldPosition + new float3(math.sin(ant.Item1.CurrentDirection),0, math.cos(ant.Item1.CurrentDirection)) * antStepSize;

                    if (antStep.x - wall.WorldPosition.x <= 2.0f || antStep.z - wall.WorldPosition.z <= 2.0f)
                    {
                        var dx = antStep.x - wall.WorldPosition.x;
                        var dy = antStep.z - wall.WorldPosition.z;
                        float sqrDist = dx * dx + dy * dy;
                        
                        if(sqrDist < 2f * 2f)
                            ant.Item1.WallDirection = (math.PI / 4.0f) * math.pow((antStepSize / 3.0f), 3.0f);
                    }
                }

                if (ant.Item2.WorldPosition.x - wall.WorldPosition.x <= 2f || ant.Item2.WorldPosition.z - wall.WorldPosition.z <= 2f)
                {
                    var dx = ant.Item2.WorldPosition.x - wall.WorldPosition.x;
                    var dy = ant.Item2.WorldPosition.z - wall.WorldPosition.z;
                    float sqrDist = dx * dx + dy * dy;
                    if(sqrDist < /*ObstacleRadius*/ 2f * 2f)
                    {
                        var dist = math.sqrt(sqrDist);
                        dx /= dist;
                        dy /= dist;
                        wallBounce = true;
                    }
                }

                ant.Item1.WallBounce = wallBounce;
            }
        }
    }
}
