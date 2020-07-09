using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Position pos, ref NormalisedMoveTime normalisedMoveTime, in MovementParabola movementParabola) =>
        {
            float distance = math.length(new float2(movementParabola.Origin.z - movementParabola.Origin.z, movementParabola.Target.x - movementParabola.Target.x));

            float duration = distance / CannonFireSystem.kCannonBallSpeed;

            pos.Value = ParabolaMath.GetSimulatedPosition(movementParabola.Origin, movementParabola.Target, 
                                                            movementParabola.Parabola.x, movementParabola.Parabola.y, movementParabola.Parabola.z, 
                                                            math.saturate(normalisedMoveTime.Value));

            normalisedMoveTime.Value += deltaTime / duration;

        }).ScheduleParallel();
    }
}