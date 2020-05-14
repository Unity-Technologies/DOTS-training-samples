﻿using Unity.Collections;
 using Unity.Entities;
using Unity.Mathematics;
 using Unity.Rendering;
 using Unity.Transforms;
 using UnityEngine.UIElements;

 [UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(ArmIKSystem))]
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
        var collisionECB = m_spawnerECB.CreateCommandBuffer().ToConcurrent();
        
        Entities
            .WithName("RockCollision")
            .WithoutBurst()
            .ForEach((int entityInQueryIndex, ref Velocity rockVel, ref RockCollisionRNG rng,in WorldRenderBounds rockBounds, in RockReservedCan reservedCan) =>
            {
                WorldRenderBounds CanWorldRenderBounds = GetComponent<WorldRenderBounds>(reservedCan);
                AABB canBounds = CanWorldRenderBounds.Value;
                
                if (canBounds.Contains(rockBounds.Value))
                {
                    collisionECB.SetComponent(entityInQueryIndex,reservedCan,new Velocity
                    {
                        Value = rockVel
                    });
                    
                    // todo can.angularVelocity = Random.onUnitSphere * velocity.magnitude * 40f;

                    //todo simulate insideUnitSphere?
                    rockVel =  rng.Value.NextFloat3() * 3f;
                    
                    collisionECB.AddComponent(entityInQueryIndex,reservedCan, new Acceleration
                    {
                        Value = new float3(0,-AnimUtils.gravityStrength,0)
                    });
                }
                
            }).ScheduleParallel();
        

        Entities
            .WithName("RockSpawnJob")
            .ForEach((int entityInQueryIndex, ref RockSpawnComponent spawner, in DestroyBoundsX killBounds, in SpawnerBoundsX spawnBounds) =>
            {
                spawner.spawnTimeRemaining -= dt;
                if (spawner.spawnTimeRemaining < 0f)
                {
                    float3 spawnPos = new float3(spawner.rng.NextFloat(spawnBounds.Value.x,spawnBounds.Value.y),0,1.5f);
                    float randRadius = spawner.rng.NextFloat(spawner.radiusRanges.x, spawner.radiusRanges.y);
                    
                    var rockEntity = spawnECB.Instantiate(entityInQueryIndex,spawner.prefab);
                    spawnECB.AddComponent(entityInQueryIndex,rockEntity, new Velocity()
                    {
                        Value = spawner.spawnVelocity
                    });
                    spawnECB.AddComponent(entityInQueryIndex,rockEntity, new DestroyBoundsX()
                    {
                        Value = new float2(killBounds.Value.x,killBounds.Value.y)
                    });
                    spawnECB.AddComponent(entityInQueryIndex,rockEntity, new RockRadiusComponentData
                    {
                        Value = randRadius,
                    });
                    spawnECB.SetComponent(entityInQueryIndex,rockEntity,new Translation
                    {
                        Value = spawnPos
                    });
                    spawnECB.SetComponent(entityInQueryIndex,rockEntity,new NonUniformScale()
                    {
                        Value = randRadius
                    });
                    
                    uint seed = 0x2048;
                    seed <<= ((rockEntity.Index + 1) % 19);
                    spawnECB.AddComponent(entityInQueryIndex, rockEntity, new RockCollisionRNG()
                    {
                        Value = new Random(seed)
                    });

                    spawner.spawnTimeRemaining = spawner.spawnTime;
                }
            }).ScheduleParallel();

    }
}
