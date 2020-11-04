using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntMovementSystem : SystemBase
{
    const float dt = 1.0f / 60;
    const float randomSteering = 0.1f;

    public static readonly Vector2 bounds = new Vector2(5, 5);

    protected override void OnUpdate()
    {
        var spawnerEntity = GetSingletonEntity<AntSpawner>();
        var spawner = GetComponent<AntSpawner>(spawnerEntity);
        var obstaclesPositions = GetBuffer<ObstaclePosition>(spawnerEntity);

        Entities
        .WithReadOnly(obstaclesPositions)
        .ForEach((ref Translation translation, ref Direction direction, ref RandState rand, in Speed speed) =>
        {
            float dx = Mathf.Cos(direction.Value) * speed.Value * dt;
            float dy = Mathf.Sin(direction.Value) * speed.Value * dt;

            // Pseudo-random steering
            direction.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

            // TODO: pheromone steering

            // TODO: steer towards target
            SteeringTowardTarget(ref translation, ref direction, spawner, obstaclesPositions);

            // Bounce off the edges of the board (for now the ant bounces back, maybe improve later)
            if (Mathf.Abs(translation.Value.x + dx) > bounds.x)
            {
                dx = -dx;
                direction.Value += Mathf.PI;
            }

            if (Mathf.Abs(translation.Value.z + dy) > bounds.y)
            {
                dy = -dy;
                direction.Value += Mathf.PI;
            }

            direction.Value = (direction.Value >= 2 * Mathf.PI) ? direction.Value - 2 * Mathf.PI : direction.Value;

            translation.Value.x += (float)dx;
            translation.Value.z += (float)dy;

        })
        .ScheduleParallel();
    }

    static bool RayCast(float2 antPos, float2 targetPos, float targetRadius, float obstacleRadius, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        var targetRadiusSq = targetRadius * targetRadius;
        var to = targetPos;
        var from = antPos;
        var mainLine = to - from;
        var mainLengthSq = math.lengthsq(mainLine);

        // Skipping computations if the ant is in the target. Consider that the ray does not hit anything.
        if (mainLengthSq < targetRadiusSq)
        {
            return false;
        }

        var mainLength = math.sqrt(mainLengthSq);
        var mainDirection = mainLine / mainLength;

        // A 2D normal is easy to compute with this trick
        var mainNormal = new float2(-mainDirection.y, mainDirection.x);

        for (int i = 0; i < obstaclePositions.Length; ++i)
        {
            var obstaclePos = obstaclePositions[i].Value;
            var delta = obstaclePos.xz - from;

            // Do not take the result of the intersection into account if the obstacle is out of range
            var isRayInRange = math.lengthsq(delta) <= mainLengthSq;
            if (!isRayInRange) continue;

            // Ignore obstacles behind
            var isObstacleBehind = math.dot(delta, mainDirection) < 0;
            if (isObstacleBehind) continue;

            // Vector projection to get the distance between the main line and the obstacle position
            var dot = math.dot(delta, mainNormal);

            var isCrossingMainLine = math.abs(dot) < obstacleRadius;
            if (isCrossingMainLine) return true;
        }

        return false;
    }

    static void SteeringTowardTarget(ref Translation translation, ref Direction direction, in AntSpawner spawner, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        // TODO : Use Food or Colony as target depending on the game state
        var targetRadius = spawner.FoodRadius;
        var targetPos = new float2(spawner.FoodPosition.x, spawner.FoodPosition.z);

        var antPos = translation.Value.xz;
        var antDirection = direction.Value;

        if (!RayCast(antPos, targetPos, targetRadius, spawner.ObstacleRadius, obstaclePositions))
        {
            float targetAngle = Mathf.Atan2(targetPos.y - antPos.y, targetPos.x - antPos.x);
            if (targetAngle - antDirection > Mathf.PI)
            {
                antDirection += Mathf.PI * 2f;
            }
            else if (targetAngle - antDirection < -Mathf.PI)
            {
                antDirection -= Mathf.PI * 2f;
            }
            else
            {
                if (Mathf.Abs(targetAngle - antDirection) < Mathf.PI * .5f)
                {
                    antDirection += (targetAngle - antDirection) * spawner.GoalSteerStrength;
                }
            }
        }

        translation.Value.x = antPos.x;
        translation.Value.z = antPos.y;
        direction.Value = antDirection;
    }
}
