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
        var allPhysics = GetComponentDataFromEntity<PhysicsData>(false);

        Entities
            .WithName("Putdown")
            .WithoutBurst()
            .WithAll<BeeTag>()
            .ForEach((Entity e, ref Translation selfTranslation, ref TargetPosition targetPos, in PhysicsData physics, in CarriedFood food) =>
            {
                if (MathUtil.IsWithinDistance(2.0f, targetPos.Value, selfTranslation.Value))
                {
                    //Clear bee and food links to each other
                    ecb.RemoveComponent<CarriedFood>(e);
                    ecb.RemoveComponent<CarrierBee>(food.Value);
                    
                    //Remove targeting info from bee so that target acquisition system picks up this bee. 
                    ecb.RemoveComponent<MoveTarget>(e);
                    ecb.RemoveComponent<TargetPosition>(e);

                    //We're checking here, since the food entity could have been destroyed by the FoodForBee system. 
                    //Had to disable burst for that reason here, though.  
                    if (EntityManager.Exists(food.Value))
                    {
                        //Mark food as intentionally dropped, so that no other bees are told to pick it up. 
                        //ecb.AddComponent<IntentionallyDroppedFoodTag>(food.Value);
                        if (allPhysics.HasComponent(food.Value))
                        {
                            var foodPhysics = allPhysics[food.Value];
                            foodPhysics.v = physics.v;
                            allPhysics[food.Value] = foodPhysics;
                        }
                    }
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