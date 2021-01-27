using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PutdownSystem : SystemBase
{
    protected override void OnUpdate()
    {
        ///////////////////////////////
        // Remove Bee's components.
        ///////////////////////////////
        var origECB = new EntityCommandBuffer(Allocator.TempJob);
        var ecb = origECB.AsParallelWriter();
        
        Entities
            .WithName("Putdown")
            .WithoutBurst()
            .WithAll<BeeTag>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation selfTranslation, ref TargetPosition targetPos, in CarriedFood food) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, targetPos.Value, selfTranslation.Value))
                {
                    //Clear bee and food links to each other
                    ecb.RemoveComponent<CarriedFood>(entityInQueryIndex, e);
                    ecb.RemoveComponent<CarrierBee>(entityInQueryIndex, food.Value);
                    
                    //Remove targeting info from bee so that target acquisition system picks up this bee. 
                    ecb.RemoveComponent<MoveTarget>(entityInQueryIndex, e);
                    ecb.RemoveComponent<TargetPosition>(entityInQueryIndex, e);
                    
                    ecb.DestroyEntity(entityInQueryIndex, food.Value);
                }
            }).Schedule();
        
        Dependency.Complete();
        origECB.Playback(EntityManager);
        origECB.Dispose();
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