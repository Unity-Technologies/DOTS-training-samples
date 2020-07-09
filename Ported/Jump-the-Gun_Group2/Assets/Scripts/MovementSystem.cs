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
            float distance = math.length(new float2(movement.Origin.z - movement.Origin.z, movement.Target.x - movement.Target.x));

            float duration = distance / movement.Speed;

            if (math.all(movement.Origin.x == movement.Target)) // Idle state
            {
                if (normalisedMoveTime.Value <= 0.5)
                    pos.Value = math.lerp(movement.Origin, movement.Origin + movement.Parabola.y, normalisedMoveTime.Value);
                else // normalisedMoveTime > 0.5
                    pos.Value = math.lerp(movement.Origin + movement.Parabola.y,movement.Origin, normalisedMoveTime.Value);
            }
            else
            {
                pos.Value = ParabolaMath.GetSimulatedPosition(  movement.Origin, movement.Target,
                                                                movement.Parabola.x, movement.Parabola.y, movement.Parabola.z,
                                                                math.saturate(normalisedMoveTime.Value));
            }

            normalisedMoveTime.Value += deltaTime / duration;

        }).ScheduleParallel();
    }
}