using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

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
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .WithAll<HandGrabbingRock>()
            .ForEach((Entity entity, ref TargetRock targetRock) =>
            {
                ecb.RemoveComponent<HandGrabbingRock>(entity);
                
                if (availableRocks.HasComponent(targetRock.RockEntity))
                {
                    // grab successful
                    ecb.RemoveComponent<Available>(targetRock.RockEntity);
                    
                    // just a hack for now : we just stop the rock
                    ecb.RemoveComponent<Velocity>(targetRock.RockEntity);
                }
                else
                {
                    // grab failed, we must try again
                    ecb.AddComponent<HandIdle>(entity);
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}