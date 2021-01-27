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
        ///////////////////////////////
        // Remove Bee's components.
        ///////////////////////////////
        var ecb1 = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithName("Putdown")
            .WithoutBurst()
            .WithAll<BeeTag, CarriedFood>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation selfTranslation, ref TargetPosition targetPos, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, targetPos.Value, selfTranslation.Value))
                {
                    ecb1.RemoveComponent<CarriedFood>(entityInQueryIndex, e);
                    ecb1.RemoveComponent<MoveTarget>(entityInQueryIndex, e);
                    targetPos.Value = float3.zero;
                }
            }).ScheduleParallel();



        ///////////////////////////////
        // Remove food's tag component...
        ///////////////////////////////
        var ecb2 = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithName("Putdown2")
            .WithoutBurst()
            .WithAll<FoodTag, CarrierBee>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation selfTranslation, ref TargetPosition targetPos, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, targetPos.Value, selfTranslation.Value))
                {
                    ecb2.RemoveComponent<CarrierBee>(entityInQueryIndex, e);
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