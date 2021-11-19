using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FollowTargetSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem sys;

    protected override void OnCreate()
    {
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var lookUpTranslation = GetComponentDataFromEntity<Translation>(true);
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();

        //Move all Foods to TargetBy translation
        Entities
            .WithNativeDisableContainerSafetyRestriction(lookUpTranslation)
            .WithAll<Food>()
            .WithAll<IsCarried>()
            .WithNone<Ballistic>()
            .WithReadOnly(lookUpTranslation)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation position, ref TargetedBy targetedBy) => 
            {
                // Stop carrying the food if our TargetedBy Bee is dead
                if (targetedBy.Value == Entity.Null || HasComponent<Ballistic>(targetedBy.Value) || HasComponent<Decay>(targetedBy.Value))
                {
                    targetedBy.Value = Entity.Null;
                    ecb.AddComponent<Ballistic>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<IsCarried>(entityInQueryIndex, entity);
                    return;
                }
                
                // Stop carrying our food if our TargetedBy Bee isn't in hunt mode
                if (!HasComponent<BeeCarryFoodMode>(targetedBy.Value))
                {
                    ecb.AddComponent<InHive>(entityInQueryIndex, entity);
                    var teamID = GetComponent<TeamID>(targetedBy.Value);
                    ecb.AddComponent<TeamID>(entityInQueryIndex, entity, new TeamID { Value = teamID.Value });
                    ecb.AddComponent<Ballistic>(entityInQueryIndex, entity);
                    ecb.RemoveComponent<IsCarried>(entityInQueryIndex, entity);
                    return;
                }

                // Follow the bee :)
                var beeLocation = lookUpTranslation[targetedBy.Value].Value;
                beeLocation.y -= 1.5f; // Add an offset so we can see the bee carrying the food instead of the food being in the bee
                position.Value = beeLocation;

            }).ScheduleParallel();
        sys.AddJobHandleForProducer(Dependency);
    }
}
