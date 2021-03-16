using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        var applyAcceleration = Entities
            .WithAll<Falling>()
            .ForEach((ref Velocity velocity) =>
            {
                velocity.Value.y -= 9.8f * deltaTime;
            }).ScheduleParallel(Dependency);

        var move = Entities
            .ForEach((ref Translation translation, in Velocity velocity) =>
            {
                translation.Value += velocity.Value * deltaTime;
            }).ScheduleParallel(applyAcceleration);

        Dependency = move;
    }
}
