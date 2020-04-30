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
        float dt = Time.DeltaTime;

        var spawnECB = m_spawnerECB.CreateCommandBuffer().ToConcurrent();
        var destroyEBC = m_spawnerECB.CreateCommandBuffer().ToConcurrent();
        
        Entities
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
                    float randRadius = spawner.rng.NextFloat(spawner.radiusRanges.x, spawner.radiusRanges.y);
                    
                    var rockEntity = spawnECB.Instantiate(entityInQueryIndex,spawner.prefab);
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
                        value = randRadius,
                    });
                    spawnECB.SetComponent(entityInQueryIndex,rockEntity,new Translation
                    {
                        Value = spawnPos
                    });
                    spawnECB.SetComponent(entityInQueryIndex,rockEntity,new NonUniformScale()
                    {
                        Value = randRadius
                    });

                    spawner.spawnTimeRemaining = spawner.spawnTime;
                }
            }).ScheduleParallel();
        
        Entities
            .WithName("RockBoundsJob")
            .ForEach((Entity entity,
                int entityInQueryIndex,
                ref Translation pos, in RockDestroyBounds bounds) =>
            {
                if(pos.Value.x < bounds.Value.x ||
                   pos.Value.x > bounds.Value.y)
                    destroyEBC.DestroyEntity(entityInQueryIndex,entity);
                
            }).ScheduleParallel();

    }
}
