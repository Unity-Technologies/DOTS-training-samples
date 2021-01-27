using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class BeeMoveToTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("BeeMovementToTarget")
            .WithoutBurst()
            .WithAll<BeeTag>()
            .ForEach((Entity e, int entityInQueryIndex, ref PhysicsData physicsData, in Translation translation, in TargetPosition t, in MoveSpeed speed) =>
            {
                var directionVector = t.Value - translation.Value;
                var destVector = math.normalize(directionVector);
                physicsData.a += destVector * speed.Value;
            }).ScheduleParallel(); //TODO: Change to schedule parallel (EntityQueryIndex)
    }
}