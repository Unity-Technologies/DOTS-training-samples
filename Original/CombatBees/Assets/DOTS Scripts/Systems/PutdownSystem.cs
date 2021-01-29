using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(PickupSystem))]
public class PutdownSystem : SystemBase
{
    private ComponentTypes typesToRemoveFromCarrier;

    protected override void OnCreate()
    {
        ComponentType[] types = new ComponentType[]
        {
            ComponentType.ReadOnly<MoveTarget>(),
            ComponentType.ReadOnly<TargetPosition>(),
            ComponentType.ReadOnly<CarriedFood>(),
        };

        typesToRemoveFromCarrier = new ComponentTypes(types);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var allPhysics = GetComponentDataFromEntity<PhysicsData>(false);
        var typesToRemoveFromCarrier = this.typesToRemoveFromCarrier;

        Entities
            .WithName("Putdown")
            .WithAll<BeeTag>()
            .ForEach((Entity e, ref Translation selfTranslation, ref TargetPosition targetPos, in PhysicsData physics, in CarriedFood food) =>
            {
                if (MathUtil.IsWithinDistance(2.0f, targetPos.Value, selfTranslation.Value))
                {
                    //Clear link from food to bee
                    ecb.RemoveComponent<CarrierBee>(food.Value);

                    //Remove targeting info from bee so that target acquisition system picks up this bee. 
                    ecb.RemoveComponent(e, typesToRemoveFromCarrier);

                    //We're checking here, since the food entity could have been destroyed by the FoodForBee system. 
                    //Had to disable burst for that reason here, though.  
                    if (HasComponent<FoodTag>(food.Value))
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