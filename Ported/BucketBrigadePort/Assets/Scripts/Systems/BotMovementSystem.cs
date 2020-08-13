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
            .ForEach((ref Translation translation, in Bot bot, in BotMovementSpeed_ForEach translationSpeed) =>
            {
                var maxMovement = translationSpeed.Value * deltaTime;
                var vector = bot.targetTranslation - translation.Value;
                var magnitude = Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
                var actualMovement = (float)(math.min(maxMovement, magnitude));
                translation.Value += vector*actualMovement;
            })
            .ScheduleParallel();
    }
}