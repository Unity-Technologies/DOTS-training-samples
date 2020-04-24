﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(ArmSystem))]
public class RockSystem: SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_spawnerECB;
    protected override void OnCreate()
    {
        m_spawnerECB = World.GetExistingSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float t = (float)Time.ElapsedTime;
        float dt = Time.DeltaTime;
        float3 centerPos = new float3(0,0,1.5f);

        var spawnECB = m_spawnerECB.CreateCommandBuffer().ToConcurrent();
        
        Entities
            .WithAll<RockTag>()
            .WithNone<RockGrabbedTag>()
            .WithName("RockMove")
            .ForEach((ref Translation pos, in RockVelocityComponentData rockVel) =>
            {
                pos.Value += rockVel.value * dt; 
                
            }).ScheduleParallel();

        Entities
            .WithName("RockSpawnJob")
            .ForEach((int entityInQueryIndex, ref RockSpawnComponent spawner, in RockDestroyBounds killBounds, in RockSpawnerBounds spawnBounds) =>
            {
                spawner.spawnTimeRemaining -= dt;
                if (spawner.spawnTimeRemaining < 0f)
                {
                    float3 spawnPos = new float3(spawner.rng.NextFloat(spawnBounds.Value.x,spawnBounds.Value.y),0,1.5f);
                    
                    var rockEntity = spawnECB.Instantiate(entityInQueryIndex,spawner.prefab);
                    spawnECB.AddComponent<RockTag>(entityInQueryIndex,rockEntity);
                    spawnECB.AddComponent(entityInQueryIndex,rockEntity, new RockVelocityComponentData
                    {
                        value = spawner.spawnVelocity
                    });
                    spawnECB.AddComponent(entityInQueryIndex,rockEntity, new RockDestroyBounds()
                    {
                        Value = new float2(killBounds.Value.x,killBounds.Value.y)
                    });
                    spawnECB.AddComponent(entityInQueryIndex,rockEntity, new RockRadiusComponentData
                    {
                        value = 0.5f,
                    });
                    spawnECB.SetComponent(entityInQueryIndex,rockEntity,new Translation
                    {
                        Value = spawnPos
                    });

                    spawner.spawnTimeRemaining = spawner.spawnTime;
                }
            }).ScheduleParallel();
        
        Entities
            .WithAll<RockTag>()
            .WithStructuralChanges()
            .WithName("RockBoundsJob")
            .ForEach((Entity entity, ref Translation pos, in RockDestroyBounds bounds) =>
            {
                if(pos.Value.x < bounds.Value.x ||
                   pos.Value.x > bounds.Value.y)
                    EntityManager.DestroyEntity(entity);
                
            }).Run();
    }
}
