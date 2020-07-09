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
                    pos.Value.y = math.lerp(movement.Origin.y, movement.Parabola.y, normalisedMoveTime.Value);
                else // normalisedMoveTime > 0.5
                    pos.Value.y = math.lerp(movement.Parabola.y, movement.Origin.y, math.saturate(normalisedMoveTime.Value));
            }
            else
            {
                // TEMP: need to process
                distance = 5; // math.length(new float2(movement.Origin.z - movement.Origin.z, movement.Target.x - movement.Target.x));

                pos.Value = ParabolaMath.GetSimulatedPosition(  movement.Origin + 0.001f, movement.Target + 0.001f,
                                                                movement.Parabola.x, movement.Parabola.y, movement.Parabola.z,
                                                                math.saturate(normalisedMoveTime.Value));
            }

            float duration = movement.Speed == 0.0f ? 1.0f : distance / movement.Speed; // Safety in case we haven't yet init the player movement
            normalisedMoveTime.Value += deltaTime / duration;

        }).ScheduleParallel();
    }
}