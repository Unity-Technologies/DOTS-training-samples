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
            pos.Value = math.lerp(movementParabola.Origin, movementParabola.Target, math.saturate(normalisedMoveTime.Value));
            normalisedMoveTime.Value += deltaTime;

        }).ScheduleParallel();
    }
}