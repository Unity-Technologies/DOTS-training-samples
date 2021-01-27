using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PutdownSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;
    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb1 = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.

        ///////////////////////////////
        // Remove Bee's components.
        ///////////////////////////////
        Entities
            .WithName("Putdown")
            .WithoutBurst()
            .WithAll<BeeTag, CarriedResource>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation selfTranslation, ref TargetPosition targetPos, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, targetPos.Value, selfTranslation.Value))
                {
                    ecb1.RemoveComponent<CarriedResource>(entityInQueryIndex, e);
                    ecb1.RemoveComponent<MoveTarget>(entityInQueryIndex, e);
                    targetPos.Value = float3.zero;
                }
            }).ScheduleParallel();


        var ecb2 = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        ///////////////////////////////
        // Remove food's tag component...
        ///////////////////////////////
        Entities
            .WithName("Putdown2")
            .WithoutBurst()
            .WithAll<FoodTag, CarrierBee>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation selfTranslation, ref TargetPosition targetPos, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, targetPos.Value, selfTranslation.Value))
                {
                    ecb2.RemoveComponent<CarrierBee>(entityInQueryIndex, e);
                    //ecb2.AddComponent(entityInQueryIndex, e, new Gravity());
                }
            }).ScheduleParallel();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

public static class MathUtil
{
    public static bool IsWithinDistance(float distance, float3 p1, float3 p2)
    {
        var directionVector = p1 - p2;
        var distanceSquared = math.lengthsq(directionVector);
        return distanceSquared < distance;
    }
}