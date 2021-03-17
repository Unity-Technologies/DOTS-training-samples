using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// System that will attribute a rock to an hand
/// </summary>
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
                if (timer.Value <= 0.0f)
                {
                    // leave grabbing state
                    ecb.RemoveComponent<HandGrabbingRock>(entity);

                    if (availableRocks.HasComponent(targetRock.RockEntity) &&
                        !availableRocks[targetRock.RockEntity].JustPicked)
                    {
                        // grab successful, time to throw
                        ecb.AddComponent<HandWindingUp>(entity);
                        ecb.SetComponent(entity, new Timer() {Value = 1.0f});
                        ecb.SetComponent(entity, new TimerDuration(){ Value = 1.0f});

                        // exclusive ownership of the rock, it can't be grabbed anymore
                        // by other arms...
                        // ...in the current foreach loop
                        availableRocks[targetRock.RockEntity] = new Available()
                        {
                            JustPicked = true
                        };
                        // ...and in the future
                        ecb.RemoveComponent<Available>(targetRock.RockEntity);
                    }
                    else
                    {
                        // grab failed, we must try again
                        ecb.AddComponent<HandIdle>(entity);
                    }
                }

            }).Schedule(Dependency);

        sys.AddJobHandleForProducer(Dependency);
    }
}