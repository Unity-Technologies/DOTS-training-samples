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
        float orbitRadius = 0.75f;

        var spawnECB = m_spawnerECB.CreateCommandBuffer().ToConcurrent();
        
        Entities.
            WithNone<DebugRockGrabbedTag>()
            .WithName("DebugRockCircleMove")
            .ForEach((ref Translation pos, in RockReservedTag _) =>
        {
            float x = orbitRadius * math.cos(t);
            float z = orbitRadius * math.sin(t);
            
            pos.Value = new float3(x,pos.Value.y,z) + centerPos;
            
        }).ScheduleParallel();
        
        Entities
            .WithAll<RockTag>()
            .WithName("RockMove")
            .ForEach((ref Translation pos, in RockVelocityComponentData rockVel) =>
            {
                pos.Value += rockVel.value * dt; 
                
            }).ScheduleParallel();

        Entities
            .WithName("RockSpawnJob")
            .ForEach((int entityInQueryIndex, ref RockSpawnComponent spawner, in RockBounds bounds) =>
            {
                spawner.spawnTimeRemaining -= dt;
                if (spawner.spawnTimeRemaining < 0f)
                {
                    float3 spawnPos = new float3(spawner.rng.NextFloat(bounds.range.x,bounds.range.y),0,2);
                    
                    var rockEntity = spawnECB.Instantiate(entityInQueryIndex,spawner.prefab);
                    spawnECB.AddComponent<RockTag>(entityInQueryIndex,rockEntity);
                    spawnECB.AddComponent(entityInQueryIndex,rockEntity, new RockVelocityComponentData
                    {
                        value = spawner.spawnVelocity
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
            .ForEach((Entity entity, ref Translation pos, in RockBounds bounds) =>
            {
                if(pos.Value.x < bounds.range.x ||
                   pos.Value.x > bounds.range.y)
                    EntityManager.DestroyEntity(entity);
                
            }).Run();
    }
}
