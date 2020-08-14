using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class TosserSeekFireSystem : SystemBase
{
    private float m_AssesLineTimer = 0f;
    private const float m_AssesLineRate = 2f;
    private EntityQuery m_FireTranslationsQuery;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_FireTranslationsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<OnFire>(),
            }
        });
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        m_AssesLineTimer -= deltaTime;
        if (m_AssesLineTimer <= 0)
        {
            m_AssesLineTimer = m_AssesLineRate;
            
            // Get the positions of all of the fire.
            var fireTranslations =
                m_FireTranslationsQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob,
                    out var fireTranslationsHandle);
            
            Dependency = JobHandle.CombineDependencies(Dependency, fireTranslationsHandle);
            var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
            
            // System updates End Point every 2s, sets to closest fire to the Line Beginning Point
            // IF the line end point changes, then the system needs to set new target positions of all of the 
            // bats on the line.
            // Inputs: entities with bucket tosser tag, target fire position

            Entities
                .WithName("TosserSeekFire")
                .WithDisposeOnCompletion(fireTranslations)
                .ForEach((
                    int entityInQueryIndex,
                    Entity tosserEntity, 
                    ref BotRoleTosser tosser, 
                    in Translation translation,
                    in LineId lineId) =>
                {
                    // Find the closest fire point.
                    float3 minPosition = float3.zero;
                    float minDistance = 999999999f;
                    bool fireFound = false;
                    for (int fireIndex = 0; fireIndex < fireTranslations.Length; fireIndex++)
                    {
                        float distance = math.length(translation.Value.xz - fireTranslations[fireIndex].Value.xz);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minPosition = fireTranslations[fireIndex].Value;
                            fireFound = true;
                        }
                    }

                    // If the closest fire point is different than the current fire point, then set a new 
                    // targetPosition.
                    // TODO: This is an optimization for later.
                    
                    if (fireFound && tosser.BotFiller != Entity.Null)
                    {
                        var fillerTranslation = GetComponent<Translation>(tosser.BotFiller);
                        var eventEntity = ecb.CreateEntity(entityInQueryIndex);
                        ecb.AddComponent(entityInQueryIndex, eventEntity, component: new LineModify
                        {
                            lineId = lineId.Value,
                            tossTranslation = minPosition,
                            fillTranslation = fillerTranslation.Value
                        });
                    }

                }).ScheduleParallel();
            
            // Register a dependency for the EntityCommandBufferSystem.
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}