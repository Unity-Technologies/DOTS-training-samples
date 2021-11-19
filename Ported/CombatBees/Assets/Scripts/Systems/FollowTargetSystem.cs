using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FollowTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var lookUpTranslation = GetComponentDataFromEntity<Translation>(true);
        var sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();

        //Move all Foods to TargetBy translation
        Entities
            .WithNativeDisableContainerSafetyRestriction(lookUpTranslation)
            .WithAll<Food>()
            .WithAll<IsCarried>()
            .WithNone<Ballistic>()
            .ForEach((Entity entity, ref Translation position, ref TargetedBy targetedBy) => 
            {
                // Stop carrying the food if our TargetedBy Bee is dead
                if (HasComponent<Ballistic>(targetedBy.Value) || HasComponent<Decay>(targetedBy.Value))
                {
                    targetedBy.Value = Entity.Null;
                    ecb.AddComponent<Ballistic>(entity);
                    ecb.RemoveComponent<IsCarried>(entity);
                    return;
                }
                
                // Stop carrying our food if our TargetedBy Bee isn't in hunt mode
                if (!HasComponent<BeeCarryFoodMode>(targetedBy.Value))
                {
                    ecb.AddComponent<InHive>(entity);
                    var teamID = GetComponent<TeamID>(targetedBy.Value);
                    ecb.AddComponent<TeamID>(entity, new TeamID { Value = teamID.Value });
                    ecb.AddComponent<Ballistic>(entity);
                    ecb.RemoveComponent<IsCarried>(entity);
                    return;
                }

                // Follow the bee :)
                var beeLocation = lookUpTranslation[targetedBy.Value];
                beeLocation.Value.y -= 1.5f; // Add an offset so we can see the bee carrying the food instead of the food being in the bee
                position.Value = beeLocation.Value;

            }).Schedule();
        sys.AddJobHandleForProducer(Dependency);
    }
}
