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
    private EntityQuery m_IdleHandsQuery;
    private EntityQuery m_AvailableRocksQuery;

    protected override void OnCreate()
    {
        m_IdleHandsQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<HandIdle>(),
            ComponentType.ReadWrite<TargetRock>(),
            ComponentType.ReadOnly<Translation>());
        
        m_AvailableRocksQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<Rock>(),
            ComponentType.ReadOnly<Available>());
    }
    
    protected override void OnUpdate()
    {
        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer ecb = sys.CreateCommandBuffer();
        
        var availableRocks = m_AvailableRocksQuery.ToEntityArray(Allocator.TempJob);
        var translations = GetComponentDataFromEntity<Translation>();

        Dependency = new ProjectileSelectionJob()
        {
            Ecb = ecb.AsParallelWriter(),
            EntityTypeHandle = GetEntityTypeHandle(),
            TranslationTypeHandle = GetComponentTypeHandle<Translation>(),
            TargetRockTypeHandle = GetComponentTypeHandle<TargetRock>(),
            Translations = GetComponentDataFromEntity<Translation>(),
            AvailableRocks = availableRocks
        }.ScheduleParallel(m_IdleHandsQuery, 1, Dependency);
        
        sys.AddJobHandleForProducer(Dependency);
    }
}


[BurstCompile]
public struct ProjectileSelectionJob : IJobEntityBatch
{
    //[NativeDisableParallelForRestriction]
    public EntityCommandBuffer.ParallelWriter Ecb;
    
    // handles for entities in the chunk only
    [ReadOnly] public EntityTypeHandle EntityTypeHandle;
    [ReadOnly] public ComponentTypeHandle<Translation> TranslationTypeHandle;
    public ComponentTypeHandle<TargetRock> TargetRockTypeHandle;
    
    // Translations of all entities in the world
    [ReadOnly] public ComponentDataFromEntity<Translation> Translations;
    
    [DeallocateOnJobCompletion, ReadOnly] 
    public NativeArray<Entity> AvailableRocks;

    public void Execute(ArchetypeChunk batchInChunk, int batchIndex)
    {
        var entitiesInBatch = batchInChunk.GetNativeArray(EntityTypeHandle);
        var translationsInBatch = batchInChunk.GetNativeArray(TranslationTypeHandle);
        var targetRocksInBatch = batchInChunk.GetNativeArray(TargetRockTypeHandle);

        for (var i = 0; i < batchInChunk.Count; ++i)
        {
            var entity = entitiesInBatch[i];
            var translation = translationsInBatch[i];

            if (Utils.FindNearestRock(translation, AvailableRocks, Translations, out Entity nearestRock))
            {
                targetRocksInBatch[i] = new TargetRock()
                {
                    RockEntity = nearestRock
                };
                
                Ecb.RemoveComponent<HandIdle>(batchIndex, entity);
                Ecb.AddComponent(batchIndex, entity, new HandGrabbingRock());
                Ecb.SetComponent(batchIndex, entity, new Timer() {Value = 1.0f});
                Ecb.SetComponent(batchIndex, entity, new TimerDuration() {Value = 1.0f});
            }
        }
    }
}
