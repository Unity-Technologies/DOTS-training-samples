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
            const float MAX_AGE = 30.0f;
            float s = (sappling.age / MAX_AGE);
            scale.Value = new float3(s,1,s);
            if (sappling.age >= MAX_AGE)
            {
                ecb.AddComponent<Crop>(entityInQueryIndex, entity);
                ecb.RemoveComponent<Sappling>(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);

    }
}
