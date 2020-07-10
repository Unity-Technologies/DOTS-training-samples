using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Position pos, ref NormalisedMoveTime normalisedMoveTime, in MovementParabola movement) =>
        {
            float distance;

            if (math.all(movement.Origin == movement.Target)) // Idle state
            {
                distance = (movement.Parabola.y - movement.Origin.y) * 2.0f;

                // TODO: do a better pendul movement
                if (normalisedMoveTime.Value <= 0.5)
                    pos.Value.y = math.lerp(movement.Origin.y, movement.Parabola.y, math.saturate(normalisedMoveTime.Value));
                else // normalisedMoveTime > 0.5
                    pos.Value.y = math.lerp(movement.Parabola.y, movement.Origin.y, math.saturate(normalisedMoveTime.Value));
            }
            else
            {
                const float kStepCount = 10.0f;
                float step = 1.0f / kStepCount;
                float normalizedArcLength = 0.0f;
                for (float f = 0; f <= 1.0f; f += step)
                {
                    normalizedArcLength += math.sqrt(1 + (movement.Parabola.x * f + movement.Parabola.y) * (movement.Parabola.x * f + movement.Parabola.y));
                }

                distance = (normalizedArcLength / (kStepCount + 1)) * math.length(movement.Target.xz - movement.Origin.xz);

                pos.Value = ParabolaMath.GetSimulatedPosition(  movement.Origin + 0.01f, movement.Target + 0.01f,
                                                                movement.Parabola.x, movement.Parabola.y, movement.Parabola.z,
                                                                math.saturate(normalisedMoveTime.Value));
            }

            float duration = movement.Speed == 0.0f ? 1.0f : math.abs(distance) / movement.Speed; // Safety in case we haven't yet init the player movement
            normalisedMoveTime.Value += deltaTime / duration;

        }).ScheduleParallel();
    }
}