using Unity.Entities;
using Unity.Transforms;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
[UpdateAfter(typeof(TargetAcquisitionSystem))]

public class TargetingTrackingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("TargetTracking")
            .ForEach((ref TargetPosition targetPos, in MoveTarget t) =>
            {
                if(HasComponent<Translation>(t.Value))
                    targetPos.Value = GetComponent<Translation>(t.Value).Value;
                
            }).ScheduleParallel();
    }
}