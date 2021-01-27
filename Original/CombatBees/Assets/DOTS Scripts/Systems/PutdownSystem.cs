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
            .WithAll<BeeTag>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation selfTranslation, ref TargetPosition targetPos, in CarriedFood food) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, targetPos.Value, selfTranslation.Value))
                {
                    //Clear bee and food links to each other
                    ecb1.RemoveComponent<CarriedFood>(entityInQueryIndex, e);
                    ecb1.RemoveComponent<CarrierBee>(entityInQueryIndex, food.Value);
                    
                    //Remove targeting info from bee so that target acquisition system picks up this bee. 
                    ecb1.RemoveComponent<MoveTarget>(entityInQueryIndex, e);
                    ecb1.RemoveComponent<TargetPosition>(entityInQueryIndex, e);
                    
                    ecb1.DestroyEntity(entityInQueryIndex, food.Value);
                }
            }).Schedule();

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