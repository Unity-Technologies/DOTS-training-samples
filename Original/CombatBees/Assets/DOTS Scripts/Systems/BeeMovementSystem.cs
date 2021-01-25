using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var allTranslations = GetComponentDataFromEntity<Translation>();
        var deltaTime = Time.DeltaTime;

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("BeeMovement")
            .WithAll<MoveTarget>()
            .ForEach((ref Translation translation, in MoveTarget t, in MoveSpeed speed) =>
            {
                var targetTranslation = allTranslations[t.Value];
                var destVector = math.normalize(targetTranslation.Value - translation.Value);
                translation.Value += destVector * deltaTime * speed.Value;
            }).Run();
    }
}