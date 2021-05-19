using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;


[UpdateAfter(typeof(FoodSpawnerSystem))]
public class AntMovementSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        const float maxDirectionChangePerSecond = 5.0f;
        const float foodSourceRadius = 2.5f;
        const float antHillRadius = 2.5f;
        
        var random = new Random((uint)(Time.ElapsedTime * 1000f + 1f));
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var deltaTime = Time.DeltaTime;
        var simulationSpeedEntity = GetSingletonEntity<SimulationSpeed>();
        var simulationSpeed = GetComponent<SimulationSpeed>(simulationSpeedEntity).Value;

        var foodSource = GetSingletonEntity<FoodSource>();
        var foodSourceTranslation = GetComponent<Translation>(foodSource);
        var foodPos = new Vector2(foodSourceTranslation.Value.x, foodSourceTranslation.Value.y);
        var antHillPosition = new Vector2(0, 0);

        var movementJob = Entities
            .WithAll<Ant>()
            .ForEach((Entity entity, ref Translation translation, ref Direction direction,
                ref Rotation rotation) =>
            {
                var delta = new float3(Mathf.Cos(direction.Radians), Mathf.Sin(direction.Radians), 0);
                translation.Value += delta * deltaTime * simulationSpeed;

                direction.Radians += random.NextFloat(-maxDirectionChangePerSecond * deltaTime * simulationSpeed,
                                                       maxDirectionChangePerSecond * deltaTime * simulationSpeed);
                
                rotation = new Rotation {Value = quaternion.RotateZ(direction.Radians)};
            }).ScheduleParallel(Dependency);
        
        var checkReachedFoodSourceJob = Entities
            .WithAll<Ant>()
            .WithNone<CarryingFood>()
            .ForEach((Entity entity, int entityInQueryIndex, ref URPMaterialPropertyBaseColor color,
                ref Direction direction, in Translation translation) =>
            {
                var pos = new Vector2(translation.Value.x, translation.Value.y);
                if (Vector2.Distance(pos, foodPos) < foodSourceRadius)
                {
                    color.Value = new float4(1, 1, 0, 0);
                    direction.Radians += Mathf.PI;
                    
                    ecb.AddComponent<CarryingFood>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(movementJob);

        var checkReachedAntHillJob = Entities
            .WithAll<Ant,CarryingFood>()
            .ForEach((Entity entity, int entityInQueryIndex, ref URPMaterialPropertyBaseColor color,
                ref Direction direction, in Translation translation) =>
            {
                var pos = new Vector2(translation.Value.x, translation.Value.y);
                if (Vector2.Distance(pos, antHillPosition) < antHillRadius)
                {
                    color.Value = new float4(0.19f, 0.21f, 0.35f, 0);
                    direction.Radians += Mathf.PI;
                    
                    ecb.RemoveComponent<CarryingFood>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(checkReachedFoodSourceJob);
        
        Dependency = checkReachedAntHillJob;
    }
}