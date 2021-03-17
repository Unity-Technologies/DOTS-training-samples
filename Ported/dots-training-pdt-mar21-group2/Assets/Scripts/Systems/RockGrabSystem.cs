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
        
        // TODO write a IJobEntityJob with readonly translations
        var updateTargetPositionsJob = Entities
            .WithAll<HandGrabbingRock>()
            .ForEach((Entity entity, ref TargetPosition targetPosition, in TargetRock targetRock, in Timer timer) =>
            {
                targetPosition.Value = translations[targetRock.RockEntity].Value;
            }).Schedule(Dependency);
        
        // Job can't be in parallel! Since it needs to solve race conditions between
        // arms trying to reach for the same rock
        var grabRocksJob = Entities
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
                        ecb.AddComponent<HandThrowingRock>(entity);

                        // exclusive ownership of the rock, it can't be grabbed anymore
                        // by other arms...
                        // ...in the current foreach loop
                        availableRocks[targetRock.RockEntity] = new Available()
                        {
                            JustPicked = true
                        };
                        // ...and in the future
                        ecb.RemoveComponent<Available>(targetRock.RockEntity);

                        // just a hack for now : we just stop the rock
                        ecb.RemoveComponent<Velocity>(targetRock.RockEntity);
                    }
                    else
                    {
                        // grab failed, we must try again
                        ecb.AddComponent<HandIdle>(entity);
                    }
                }

            }).Schedule(Dependency);

        Dependency = Unity.Jobs.JobHandle.CombineDependencies(updateTargetPositionsJob, grabRocksJob);
        sys.AddJobHandleForProducer(Dependency);
    }
}