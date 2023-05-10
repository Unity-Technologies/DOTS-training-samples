using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Ant))]
public partial struct ObstacleDetection : IJobEntity
{
    public float distance;
    public float obstacleSize;
    public float steeringStrength;
    public NativeArray<LocalTransform> obstacles;

    public void Execute(Entity entity, ref Ant ant, in Position position, in Direction direction)
    {
        int output = 0;

        var directionInRadians = direction.direction / 180f * (float) Math.PI;

        // this for loop makes us check the direction * -1 and * 1
        for (int i = -1; i <= 1; i += 2)
        {
            float angle = directionInRadians + i * Mathf.PI * 0.25f;
            float testX = position.position.x + Mathf.Cos(angle) * distance;
            float testY = position.position.y + Mathf.Sin(angle) * distance;

            foreach (var transform in obstacles)
            {
                float circleX = transform.Position.x;
                float circleY = transform.Position.y;
                if ((testX - circleX) * (testX - circleX) + (testY - circleY) * (testY - circleY) <= obstacleSize)
                {
                    output -= i;
                    break;
                }
            }
        }

        ant.wallSteering = output * steeringStrength / (float) Math.PI * 180f;
    }
}