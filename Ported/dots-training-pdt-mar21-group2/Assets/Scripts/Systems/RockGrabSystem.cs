using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// System that will try to attribute a rock to every hand asking for it
/// </summary>
[UpdateAfter(typeof(ProjectileSelectionSystem))]
public class RockGrabSystem : SystemBase
{
    private EntityQuery m_AvailableRocksQuery;

    protected override void OnCreate()
    {
        m_AvailableRocksQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Rock>(),
            ComponentType.ReadOnly<Available>());
    }
    
    protected override void OnUpdate()
    {
        var availableRocks = GetComponentDataFromEntity<Available>();
        var translations = GetComponentDataFromEntity<Translation>();

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        var ecb = sys.CreateCommandBuffer();
        
        // Job can't be in parallel! Since it needs to solve race conditions between
        // arms trying to reach for the same rock
        Dependency = Entities
            .WithAll<HandGrabbingRock>()
            .ForEach((Entity entity, in TargetRock targetRock, in Timer timer) =>
            {
                if (!availableRocks.HasComponent(targetRock.RockEntity) ||
                    availableRocks[targetRock.RockEntity].JustPicked)
                {
                    // grab failed, we must try again : go to Idle
                    Utils.GoToState<HandGrabbingRock, HandIdle>(ecb, entity);
                    
                    // reset target
                    ecb.SetComponent<TargetRock>(entity, new TargetRock());
                }
                else if (Utils.DidAnimJustFinished(timer))
                {
                    // grab successful, time to look for a can to throw that rock
                    Utils.GoToState<HandGrabbingRock, HandLookingForACan>(ecb, entity);

                    // exclusive ownership of the rock, it can't be grabbed anymore
                    // by other arms...
                    // ...in the current foreach loop
                    availableRocks[targetRock.RockEntity] = new Available()
                    {
                        JustPicked = true
                    };
                    // ...and in the future
                    ecb.RemoveComponent<Available>(targetRock.RockEntity);
                    
                    // stop the rock
                    ecb.AddComponent<Grabbed>(targetRock.RockEntity);
                }

            }).Schedule(Dependency);

        sys.AddJobHandleForProducer(Dependency);
    }
}