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
    private EntityQuery wallQuery;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var desc = new EntityQueryDesc {All = new []{new ComponentType(typeof(Wall))}};
        wallQuery = GetEntityQuery(desc);
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

        var screenSizeEntity = GetSingletonEntity<ScreenSize>();
        var screenSize = GetComponent<ScreenSize>(screenSizeEntity).Value;
        var screenUpperBound = (float) screenSize / 2f - 0.5f;
        var screenLowerBound = -screenSize / 2f + 0.5f;
        
        var walls = wallQuery.ToEntityArray(Allocator.TempJob);
        var wallComponentData = GetComponentDataFromEntity<Wall>(true);

        var wall = new Wall {Angles = new float2(0, math.PI / 2f), Radius = 25};
        var halfWallThickness = 2.5f;

        var movementJob = Entities
            .WithAll<Ant>()
            .WithReadOnly(wallComponentData)
            .WithDisposeOnCompletion(walls)
            .ForEach((Entity entity, ref Translation translation, ref Direction direction,
                ref Rotation rotation) =>
            {
                // Change ant direction by random amount
                var maxFrameDirectionChange = maxDirectionChangePerSecond * deltaTime * simulationSpeed;
                direction.Radians += random.NextFloat(-maxFrameDirectionChange, maxFrameDirectionChange);

                var prevAntAngle = math.atan2(translation.Value.y, translation.Value.x);

                // Move ant a step forward in its direction
                var delta = new float2(math.cos(direction.Radians), math.sin(direction.Radians));
                delta *= deltaTime * simulationSpeed;
                translation.Value.x += delta.x;
                translation.Value.y += delta.y;
                
                // Check for wall collisions
                foreach (var wallEntity in walls)
                {
                    var wall = wallComponentData[wallEntity];
                }
                
                // Convert the ant location to polar coordinates
                var antRadius = math.distance(float3.zero, translation.Value);
                var antAngle = math.atan2(translation.Value.y, translation.Value.x);

                if (antRadius > wall.Radius - halfWallThickness && antRadius < wall.Radius + halfWallThickness)
                {
                    if (antAngle > wall.Angles.x && antAngle < wall.Angles.y)
                    {
                        var collisionPoint = new float2(math.cos(antAngle), math.sin(antAngle));
                        var normal = float2.zero - collisionPoint;
                        var newDirection = math.reflect(delta, normal);
                        direction.Radians = math.atan2(newDirection.y, newDirection.x);
                        translation.Value.x += newDirection.x;
                        translation.Value.y += newDirection.y;
                    }
                }

                // Check if we're hitting the screen edge
                if (translation.Value.x > screenUpperBound)
                {
                    direction.Radians = Mathf.PI - direction.Radians;
                    translation.Value.x = screenUpperBound;
                }
                else if (translation.Value.x < screenLowerBound)
                {
                    direction.Radians = Mathf.PI - direction.Radians;
                    translation.Value.x = screenLowerBound;
                }

                if (translation.Value.y > screenUpperBound)
                {
                    direction.Radians = -direction.Radians;
                    translation.Value.y = screenUpperBound;
                }
                else if (translation.Value.y < screenLowerBound)
                {
                    direction.Radians = -direction.Radians;
                    translation.Value.y = screenLowerBound;
                }

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