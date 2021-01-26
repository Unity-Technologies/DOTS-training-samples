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
        var allTranslations = GetComponentDataFromEntity<Translation>(true);
        var deltaTime = Time.DeltaTime;
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("BeeMovement")
            .WithAll<BeeTag>()
            .ForEach((Entity e, ref Translation translation, in MoveTarget moveTarget, in TargetPosition t, in MoveSpeed speed) =>
            {
                var directionVector = t.Value - translation.Value;
                var distanceSquared = math.lengthsq(directionVector);
                if (distanceSquared < 1)
                {
                    ecb.SetComponent(e, new CarriedResource() {Value = moveTarget.Value});
                    ecb.SetComponent(moveTarget.Value, new CarrierBee() {Value = e});
                }
                else
                {
                    var destVector = math.normalize(directionVector);
                    translation.Value += destVector * deltaTime * speed.Value;
                }

            }).Run();
    }
}