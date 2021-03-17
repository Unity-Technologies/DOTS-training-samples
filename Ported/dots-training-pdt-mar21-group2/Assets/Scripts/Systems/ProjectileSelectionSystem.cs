using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// For each arm looking to grab a rock (i.e. having a HandleIdle component)
///     For each rock that is available
///         Find the nearest rock.
///         If any, the hand go to grabbing state (HandleIdle -> HandGrabbingRock)
///         The rock is unchanged, meaning several hands can try to reach for the same rock in parallel
/// </summary>
public class ProjectileSelectionSystem : SystemBase
{
    private EntityQuery m_AvailableRocksQuery;

    protected override void OnCreate()
    {
        m_AvailableRocksQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Rock>(),
            ComponentType.ReadOnly<Available>());
    }
    
    protected override void OnUpdate()
    {
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();
        
        var availableRocks = m_AvailableRocksQuery.ToEntityArray(Allocator.TempJob);
        var translations = GetComponentDataFromEntity<Translation>();
        
        Dependency = Entities
            .WithDisposeOnCompletion(availableRocks)
            .WithReadOnly(availableRocks)
            .WithReadOnly(translations)
            .WithAll<HandIdle>()
            .ForEach((Entity entity, int entityInQueryIndex,
                ref TargetRock targetRock,
                in Translation translation) =>
        {
            if (Utils.FindNearestRock(translation, availableRocks, translations, out Entity nearestRock))
            {
                targetRock = new TargetRock()
                {
                    RockEntity = nearestRock
                };
                
                // Go to grab rock state
                ecb.RemoveComponent<HandIdle>(entityInQueryIndex, entity);
                ecb.AddComponent<HandGrabbingRock>(entityInQueryIndex, entity);
                ecb.SetComponent(entityInQueryIndex, entity, new Timer() {Value = 1.0f});
                ecb.SetComponent(entityInQueryIndex, entity, new TimerDuration() {Value = 1.0f});
            }
        }).ScheduleParallel(Dependency);
        
        sys.AddJobHandleForProducer(Dependency);
    }
}
