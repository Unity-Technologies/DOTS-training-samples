using System;
using System.Transactions;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class BotMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithName("TranslationSpeedSystem_ForEach")
            .ForEach((ref Translation translation, ref TargetPosition targetPosition, in Bot bot, in BotMovementSpeed_ForEach translationSpeed) =>
            {
                var maxMovement = translationSpeed.Value * deltaTime;
                var vector = targetPosition.Value - translation.Value;
                var magnitude = math.distance(targetPosition.Value, translation.Value);
                var actualMovement = math.min(maxMovement, magnitude);
                translation.Value += vector*actualMovement;
            })
            .ScheduleParallel();
    }
}