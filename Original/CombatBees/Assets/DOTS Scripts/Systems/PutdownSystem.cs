using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PickupSystem))]
public class PutdownSystem : SystemBase
{
    protected override void OnUpdate()
    {
        ///////////////////////////////
        // Remove Bee's components.
        ///////////////////////////////
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .WithName("Putdown")
            .WithAll<BeeTag>()
            .ForEach((Entity e, ref Translation selfTranslation, ref TargetPosition targetPos, in CarriedFood food) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, targetPos.Value, selfTranslation.Value))
                {
                    //Clear bee and food links to each other
                    ecb.RemoveComponent<CarriedFood>( e);
                    ecb.RemoveComponent<CarrierBee>( food.Value);
                    
                    //Remove targeting info from bee so that target acquisition system picks up this bee. 
                    ecb.RemoveComponent<MoveTarget>( e);
                    ecb.RemoveComponent<TargetPosition>( e);
                    
                    ecb.DestroyEntity(food.Value);
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
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