using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class TargetingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var allTranslations = GetComponentDataFromEntity<Translation>(true);

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Targeting")
            .ForEach((ref TargetPosition targetPos, in MoveTarget t) =>
            {
                var targetTranslation = allTranslations[t.Value];
                targetPos.Value = targetTranslation.Value;

            }).Run();
    }
}