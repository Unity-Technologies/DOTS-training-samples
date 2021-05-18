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
        
        var random = new Random((uint)(Time.ElapsedTime * 1000f + 1f));
        var deltaTime = Time.DeltaTime;
        var simulationSpeedEntity = GetSingletonEntity<SimulationSpeed>();
        var simulationSpeed = GetComponent<SimulationSpeed>(simulationSpeedEntity).Value;

        Dependency = Entities
            .WithAll<Ant>()
            .ForEach((Entity entity, ref Translation translation, ref Direction direction, ref Rotation rotation) =>
            {
                var delta = new float3(Mathf.Cos(direction.Radians), Mathf.Sin(direction.Radians), 0);
                translation.Value += delta * deltaTime * simulationSpeed;

                direction.Radians += random.NextFloat(-maxDirectionChangePerSecond * deltaTime * simulationSpeed,
                                                       maxDirectionChangePerSecond * deltaTime * simulationSpeed);
                
                rotation = new Rotation {Value = quaternion.RotateZ(direction.Radians)};
            }).ScheduleParallel(Dependency);

        Dependency.Complete();
        
        var foodSource = GetSingletonEntity<FoodSource>();
        var foodSourceTranslation = GetComponent<Translation>(foodSource);
        var foodPos = new Vector2(foodSourceTranslation.Value.x, foodSourceTranslation.Value.y);
        
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        Dependency = Entities
            .WithAll<Ant>()
            .WithNone<CarryingFood>()
            .ForEach((Entity entity, int entityInQueryIndex, ref URPMaterialPropertyBaseColor color, in Translation translation) =>
            {
                var pos = new Vector2(translation.Value.x, translation.Value.y);
                if (Vector2.Distance(pos, foodPos) < foodSourceRadius)
                {
                    color.Value = new float4(1, 1, 0, 0);
                    
                    ecb.AddComponent<CarryingFood>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel(Dependency);
        
        Dependency.Complete();
    }
}