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
    private EntityQuery m_FireQuery;
    private EntityQuery m_LakeQuery;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_FireQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<OnFire>(),
            }
        });
        m_LakeQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<WaterAmount>(),
                ComponentType.ReadOnly<WaterRefill>()
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
                m_FireQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob,
                out var fireTranslationsHandle);
            var lakeTranslations = 
                m_LakeQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob,
                out var lakeTranslationsHandle);
            Dependency = JobHandle.CombineDependencies(Dependency, fireTranslationsHandle);
            Dependency = JobHandle.CombineDependencies(Dependency, lakeTranslationsHandle);
            var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
            
            // System updates End Point every 2s, sets to closest fire to the Line Beginning Point
            // IF the line end point changes, then the system needs to set new target positions of all of the 
            // bats on the line.
            // Inputs: entities with bucket tosser tag, target fire position

            Entities
                .WithName("TosserSeekFire")
                .WithDisposeOnCompletion(fireTranslations)
                .WithDisposeOnCompletion(lakeTranslations)
                .ForEach((
                    int entityInQueryIndex,
                    Entity tosserEntity, 
                    ref BotRoleTosser tosser, 
                    in Translation translation,
                    in LineId lineId) =>
                {
                    // Find the closest fire point.
                    float3 closestFirePosition = float3.zero;
                    float minDistance = 999999999f;
                    bool fireFound = false;
                    for (int fireIndex = 0; fireIndex < fireTranslations.Length; fireIndex++)
                    {
                        float distance = math.length(translation.Value.xz - fireTranslations[fireIndex].Value.xz);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestFirePosition = fireTranslations[fireIndex].Value;
                            fireFound = true;
                        }
                    }
                    
                    float3 closestLakePosition = float3.zero;
                    minDistance = 999999999f;
                    bool lakeFound = false;
                    // Debug.Log($"lakeTranslations.Length: {lakeTranslations.Length}");

                    for (int lakeIndex = 0; lakeIndex < lakeTranslations.Length; lakeIndex++)
                    {
                        float distance = math.length(translation.Value.xz - lakeTranslations[lakeIndex].Value.xz);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestLakePosition = lakeTranslations[lakeIndex].Value;
                            lakeFound = true;
                        }
                    }

                    // Debug.Log($"closestLakePosition: {closestLakePosition}");

                    // If the closest fire point is different than the current fire point, then set a new 
                    // targetPosition.
                    // TODO: This is an optimization for later.
                    
                    if (fireFound && tosser.BotFiller != Entity.Null
                        && lakeFound)
                    {
                        // var fillerTranslation = GetComponent<Translation>(tosser.BotFiller);
                        var eventEntity = ecb.CreateEntity(entityInQueryIndex);
                        ecb.AddComponent(entityInQueryIndex, eventEntity, component: new LineModify
                        {
                            lineId = lineId.Value,
                            tossTranslation = closestFirePosition,
                            fillTranslation = closestLakePosition
                        });
                    }

                }).ScheduleParallel();
            
            // Register a dependency for the EntityCommandBufferSystem.
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}