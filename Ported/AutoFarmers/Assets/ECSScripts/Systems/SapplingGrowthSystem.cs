using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SapplingGrowthSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var gameState = GetSingletonEntity<GameState>();
        float simulationSpeed = GetComponent<GameState>(gameState).SimulationSpeed;
        
        float dt = Time.DeltaTime;
        
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .WithAll<Sappling>()
            .ForEach((
                Entity entity
                , int entityInQueryIndex
                , ref Sappling sappling
                , ref NonUniformScale scale
                ) =>
        {
            sappling.age += (dt * simulationSpeed);
            const float MAX_AGE = 5.0f;
            float s = (sappling.age / MAX_AGE) / 1.5f;
            scale.Value = new float3(s, s, s);
            float4 color;
            if (sappling.age >= MAX_AGE)
            {
                ecb.AddComponent<Crop>(entityInQueryIndex, entity);
                ecb.RemoveComponent<Sappling>(entityInQueryIndex, entity);
                color = new float4(1.0f, 0.75f, 0.0f, 1.0f);
                ecb.SetComponent(entityInQueryIndex, entity, new ECSMaterialOverride {Value = color});
            }
            else
            {
                color = new float4(0.3207547f, 0.2701775f, 0.1165005f, 1.0f);
                ecb.SetComponent(entityInQueryIndex, entity, new ECSMaterialOverride {Value = color});
            }
        }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);

    }
}

            //Entity tilledE = EntityManager.Instantiate(gameState.TilledPrefab); //NOTE(atheisen): farmers should add this, here to spawn crops while testing
            //EntityManager.AddComponentData(tileEntity, new Tilled {FertilityLeft = 0, TilledDisplayPrefab = tilledE}); //NOTE(atheisen): farmers should add this, here to spawn crops while testing
            //float3 offset = new float3(0.0f, 0.01f, 0.0f);
            //EntityManager.SetComponentData(tilledE, new Translation {Value = tilePos + offset}); //NOTE(atheisen): farmers should add this, here to spawn crops while testing
            //EntityManager.AddComponent<ECSMaterialOverride>(tilledE); //NOTE(atheisen): farmers should add this, here to spawn crops while testing
            //tilled.FertilityLeft = fertility; //NOTE(atheisen): set fertility when tiling
