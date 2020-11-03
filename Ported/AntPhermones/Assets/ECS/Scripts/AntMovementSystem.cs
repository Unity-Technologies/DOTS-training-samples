using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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
            .ForEach((ref Translation translation, ref Direction direction, ref RandState rand, in Speed speed) =>
            {
                float dx = Mathf.Cos(direction.Value) * speed.Value * dt;
                float dy = Mathf.Sin(direction.Value) * speed.Value * dt;

                // Pseudo-random steering
                direction.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

                // TODO: pheromone steering

                // TODO: steer towards target

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

            }).ScheduleParallel();
    }
}
