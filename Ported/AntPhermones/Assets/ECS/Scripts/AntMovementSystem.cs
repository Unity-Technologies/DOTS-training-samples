using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class AntMovementSystem : SystemBase
{
    const float dt = 1.0f / 60;
    const float randomSteering = 0.1f;

    protected override void OnUpdate()
    {

        Vector2 bounds = new Vector2(5, 5);

        Entities
            .ForEach((ref Translation translation, ref Direction antData, ref RandState rand, in Speed speed) =>
            {
                float dx = Mathf.Cos(antData.Value) * speed.Value * dt;
                float dy = Mathf.Sin(antData.Value) * speed.Value * dt;

                // Pseudo-random steering
                antData.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

                // TODO: pheromone steering

                // TODO: steer towards target

                // Bounce off the edges of the board (for now the ant bounces back, maybe improve later)
                if (Mathf.Abs(translation.Value.x + dx) > bounds.x)
                {
                    dx = -dx;
                    antData.Value += Mathf.PI;
                }

                if (Mathf.Abs(translation.Value.z + dy) > bounds.y)
                {
                    dy = -dy;
                    antData.Value += Mathf.PI;
                }

                antData.Value = (antData.Value >= 2 * Mathf.PI) ? antData.Value - 2 * Mathf.PI : antData.Value;

                translation.Value.x += (float)dx;
                translation.Value.z += (float)dy;

            }).ScheduleParallel();
    }
}
