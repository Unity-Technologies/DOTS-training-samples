using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AntMovementSystem : SystemBase
{
    struct DataInputs
    {

    }

    const float dt = 1.0f / 60;
    const float randomSteering = 0.1f;

    public static readonly Vector2 bounds = new Vector2(5, 5);

    EntityCommandBufferSystem cmdBufferSystem;

    protected override void OnCreate()
    {
        cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var cmd = cmdBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var spawnerEntity = GetSingletonEntity<AntSpawner>();
        var spawner = GetComponent<AntSpawner>(spawnerEntity);
        var obstaclesPositions = GetBuffer<ObstaclePosition>(spawnerEntity);

        Entities
        .WithReadOnly(obstaclesPositions)
        .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand, in Entity antEntity, in Speed speed) =>
        {
            float dx = Mathf.Cos(direction.Value) * speed.Value * dt;
            float dy = Mathf.Sin(direction.Value) * speed.Value * dt;

            // Pseudo-random steering
            direction.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

            // TODO: pheromone steering



            // TODO: steer towards target

            var target3D = float3.zero;
            var targetRadius = 0f;
            if (HasComponent<AntLookingForFood>(antEntity))
            {
                target3D = spawner.FoodPosition;
                targetRadius = spawner.FoodRadius;

                if (HasReachedTarget(translation.Value, target3D, targetRadius))
                {
                    cmd.RemoveComponent<AntLookingForFood>(entityInQueryIndex, antEntity);
                    cmd.AddComponent<AntLookingForNest>(entityInQueryIndex, antEntity);
                }
            }
            else if (HasComponent<AntLookingForNest>(antEntity))
            {
                target3D = spawner.ColonyPosition;
                targetRadius = spawner.ColonyRadius;

                if (HasReachedTarget(translation.Value, target3D, targetRadius))
                {
                    cmd.RemoveComponent<AntLookingForNest>(entityInQueryIndex, antEntity);
                    cmd.AddComponent<AntLookingForFood>(entityInQueryIndex, antEntity);
                }
            }

            var target2D = new float2(target3D.x, target3D.z);
            SteeringTowardTarget(ref translation, ref direction, target2D, spawner, obstaclesPositions);

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

        // Our jobs must finish before the EndSimulationEntityCommandBufferSystem execute the changements we recorded
        cmdBufferSystem.AddJobHandleForProducer(this.Dependency);
    }

    static void SteeringTowardTarget(ref Translation translation, ref Direction direction, float2 target, in AntSpawner spawner, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        var targetRadius = spawner.FoodRadius;

        var antPos = translation.Value.xz;
        var antDirection = direction.Value;

        if (!RayCast(antPos, target, targetRadius, spawner.ObstacleRadius, obstaclePositions))
        {
            float targetAngle = Mathf.Atan2(target.y - antPos.y, target.x - antPos.x);
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

    public static bool RayCast(float2 from, float2 to, float targetRadius, float obstacleRadius, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        var targetRadiusSq = targetRadius * targetRadius;
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

    static bool HasReachedTarget(float3 position, float3 target, float targetRadius)
    {
        return math.distancesq(position, target) < (targetRadius * targetRadius);
    }
}
