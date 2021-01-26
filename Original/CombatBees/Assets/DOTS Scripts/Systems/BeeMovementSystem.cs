using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
public class BeeMoveToTargetSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;
    
    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("BeeMovementToTarget")
            .WithoutBurst()
            .WithAll<BeeTag>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, in TargetPosition t, in MoveSpeed speed) =>
            {
                var directionVector = t.Value - translation.Value;
                var destVector = math.normalize(directionVector);
                translation.Value += destVector * deltaTime * speed.Value;

            }).ScheduleParallel(); //TODO: Change to schedule parallel (EntityQueryIndex)

        //Entities
        //    .WithName("BeeMovementToBase")
        //    .WithoutBurst()
        //    .WithAll<BeeTag, CarriedResource>()
        //    .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, in TargetPosition t, in MoveSpeed speed) =>
        //    {
        //        var directionVector = float3.zero - translation.Value;
        //        var destVector = math.normalize(directionVector);
        //        translation.Value += destVector * deltaTime * speed.Value;

        //    }).ScheduleParallel(); //TODO: Change to schedule parallel (EntityQueryIndex)
    }
}