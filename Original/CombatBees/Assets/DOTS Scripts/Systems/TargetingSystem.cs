using Unity.Entities;
using Unity.Transforms;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
[UpdateBefore(typeof(PickupSystem))]
[UpdateBefore(typeof(BeeMoveToTargetSystem))]
public class TargetingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Targeting")
            .ForEach((ref TargetPosition targetPos, in MoveTarget t) =>
            {
                targetPos.Value = GetComponent<Translation>(t.Value).Value;
            }).ScheduleParallel();
    }
}