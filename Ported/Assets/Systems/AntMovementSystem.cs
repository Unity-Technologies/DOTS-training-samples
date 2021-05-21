using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(AntSpawnerSystem))]
[UpdateAfter(typeof(FoodSpawnerSystem))]
[UpdateAfter(typeof(PheromoneSpawnerSystem))]
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

    // Change ant direction by random amount
    private static void RandomDirectionChange(ref Random random, ref Direction direction, float simulationSpeed, float deltaTime)
    {
        const float maxDirectionChangePerSecond = 1f;

        var maxFrameDirectionChange = maxDirectionChangePerSecond * deltaTime * simulationSpeed;
        var angle = math.atan2(direction.Value.y, direction.Value.x);
        angle +=random.NextFloat(-maxFrameDirectionChange, maxFrameDirectionChange);
        direction.Value = new float2(math.cos(angle), math.sin(angle));
    }

    private static bool HandleWallCollisions(NativeArray<Entity> walls, ComponentDataFromEntity<Wall> wallComponentData,
        ref Translation translation, ref Direction direction, float2 directionVec,
        ref Random random, float simulationSpeed, float deltaTime)
    {
        var halfWallThickness = 1.5f;
        var startTurningDistance = 2f;
        var halfAnt = 0.5f;
        
        // Convert the ant location to polar coordinates
        var antRadius = math.distance(float3.zero, translation.Value);
        var antAngle = math.atan2(translation.Value.y, translation.Value.x);

        bool shouldCollide = false;
        bool shouldCollideFromCorner = false;
        float3 positionToBounce = float3.zero;
        float wallRadius = 0.0f;
        foreach (var wallEntity in walls)
        {
            var wall = wallComponentData[wallEntity];
            if (antRadius > wall.Radius - (startTurningDistance + halfAnt) && antRadius < wall.Radius + (startTurningDistance + halfAnt))
            {
                var antAngleDeg = antAngle * Mathf.Rad2Deg;
                while (antAngleDeg < 0)
                    antAngleDeg += 360;
                while (antAngleDeg > 360)
                    antAngleDeg -= 360;
                shouldCollide = true;
                wallRadius = wall.Radius;

                if ( wall.Angles.x <  wall.Angles.y)
                {
                    if (antAngleDeg > wall.Angles.x && antAngleDeg < wall.Angles.y)
                    {
                        // in gap, can't collide
                        shouldCollide = false;
                    }
                }
                else
                {
                    if (antAngleDeg > wall.Angles.x || antAngleDeg < wall.Angles.y)
                    {
                        // in gap, can't collide
                        shouldCollide = false;
                    }
                }

                if (!shouldCollide)
                {
                    var pos = new float3(math.cos(math.radians(wall.Angles.x)) * wall.Radius, math.sin(math.radians(wall.Angles.x)) * wall.Radius, 0f);
                    if (math.distance(pos, translation.Value) < halfWallThickness * 2f)
                    {
                        shouldCollideFromCorner = true;
                        positionToBounce = pos;
                    }
                    pos = new float3(math.cos(math.radians(wall.Angles.y)) * wall.Radius, math.sin(math.radians(wall.Angles.y)) * wall.Radius, 0f);
                    if (math.distance(pos, translation.Value) < halfWallThickness * 2f)
                    {
                        shouldCollideFromCorner = true;
                        positionToBounce = pos;
                    }
                    break;
                }
            }
        }
        
        if (shouldCollide)
        {
            if (true)
            {
                var collisionPoint = (new float2(math.cos(antAngle), math.sin(antAngle))) * wallRadius;
                var normal = float2.zero - collisionPoint;
                var reflected = math.reflect(directionVec, normal);
                reflected = reflected / math.length(reflected);
                var antPos = translation.Value.xy;

                var nextPos = antPos + directionVec * 0.01f;
                var distanceToWallCenter = math.length(antPos - collisionPoint);
                if (math.length(nextPos - collisionPoint) >= distanceToWallCenter)
                {
                    // Heading away from the wall
                }
                else
                {
                    var strength = 0.3f;
                    var targetDirection = directionVec * (1.0f - strength) + reflected * strength;
                    direction.Value = targetDirection;
                    var step = targetDirection * deltaTime * simulationSpeed;
                    translation.Value.x += step.x;
                    translation.Value.y += step.y;
                    return true;
                }
            }
            else
            {
                var collisionPoint = new float2(math.cos(antAngle), math.sin(antAngle));
                var normal = float2.zero - collisionPoint;
                var newDirection = math.reflect(directionVec * directionVec, normal);
                direction.Value = newDirection;
                translation.Value.x += newDirection.x;
                translation.Value.y += newDirection.y;
            }
            
        }
        else if (shouldCollideFromCorner)
        {
            var normal3 = positionToBounce - translation.Value;
            var normal = new float2(normal3.x, normal3.y);
            var newDirection = math.reflect(directionVec * deltaTime * simulationSpeed, normal);
            direction.Value = newDirection;
            translation.Value.x += newDirection.x;
            translation.Value.y += newDirection.y;
            return true;
        }

        return false;
    }

    private static void HandleScreenBoundCollisions(ref Translation translation, ref Direction direction, float screenLowerBound, float screenUpperBound)
    {
        const float border = 3f;
        var upperBound = screenUpperBound - border;
        var lowerBound = screenLowerBound + border;

        var angle = math.atan2(direction.Value.y, direction.Value.x);

        // Check if we're hitting the screen edge
        if (translation.Value.x > upperBound)
        {
            angle = Mathf.PI - angle;
            translation.Value.x = upperBound;
        }
        else if (translation.Value.x < lowerBound)
        {
            angle = Mathf.PI - angle;
            translation.Value.x = lowerBound;
        }

        if (translation.Value.y > upperBound)
        {
            angle = -angle;
            translation.Value.y = upperBound;
        }
        else if (translation.Value.y < lowerBound)
        {
            angle = -angle;
            translation.Value.y = lowerBound;
        }

        direction.Value = new float2(math.cos(angle), math.sin(angle));
    }

    private static int ClampPheromoneMap(PheromoneMap map, int pos)
    {
        return math.clamp(pos, 0, map.gridSize - 1);
    }

    private static int2 ClampPheromoneMap(PheromoneMap map, int2 pos)
    {
        return new int2(ClampPheromoneMap(map, pos.x), ClampPheromoneMap(map, pos.y));
    }

    private static int2 PheromoneGridLocation(PheromoneMap map, float2 translation, float halfScreenSize, float pheromoneMapFactor)
    {
        return new int2( (translation + halfScreenSize) * pheromoneMapFactor);
    }

    private static int PheromoneGridIndex(PheromoneMap map, int2 location)
    {
        var clampedLocation = ClampPheromoneMap(map, location);
        return clampedLocation.y * map.gridSize + clampedLocation.x;
    }

    private static void FollowPheromones(PheromoneMap map, DynamicBuffer<Pheromone> pheromoneBuffer,
        Translation translation, ref Direction direction, float2 antDirection, float halfScreenSize,
        float pheromoneMapFactor, float pheromoneStrength)
    {
        var antTextureCoord = PheromoneGridLocation(map, translation.Value.xy, halfScreenSize, pheromoneMapFactor);

        // Compute a "pull" direction from each pheromone point in a 5x5 grid around the ant.
        float2 pullDirection = float2.zero;
        const int radius = 3;
        const int side = radius + 1 + radius;
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                var textureOffset = new int2(x, y);
                if (x == 0 && y == 0)
                    continue;
                
                var textureCoord = antTextureCoord + textureOffset;
                var index = PheromoneGridIndex(map, textureCoord);
                float intensity = pheromoneBuffer[index].Value;
                float2 offset = new float2(x, y);

                // Is this point behind the ant?
                if (math.dot(offset, antDirection) < 0)
                    continue;
                
                float length = math.length(offset);
                pullDirection += offset / length / length * intensity;
            }
        }
        
        // Divide by how many points we checked so that we get a value between 0 and 1
        pullDirection /= side * side;

        antDirection = pullDirection * pheromoneStrength + antDirection;
        direction.Value = antDirection / math.length(direction.Value);
    }

    static void ApproachFoodSource(Translation translation, ref Direction direction, float2 foodPos,
        NativeArray<Entity> walls, ComponentDataFromEntity<Wall> wallComponentData,
        float deltaTime, float simulationSpeed)
    {
        if (IsTargetVisible(translation.Value.xy, foodPos, walls, wallComponentData))
        {
            float2 pullDirection = new float2(foodPos.x, foodPos.y) - translation.Value.xy;
            direction.Value = pullDirection * 0.001f + direction.Value * 0.90f;
            direction.Value = direction.Value / math.length(direction.Value);
        }
    }
    
    private static void ApproachAntHill(Translation translation, Direction direction,
         NativeArray<Entity> walls, ComponentDataFromEntity<Wall> wallComponentData,
         float deltaTime, float simulationSpeed)
    {
        if (IsTargetVisible(translation.Value.xy, float2.zero, walls, wallComponentData))
        {
            float2 pullDirection = float2.zero - translation.Value.xy;
            direction.Value = pullDirection * 0.001f + direction.Value * 0.90f;
            direction.Value = direction.Value / math.length(direction.Value);
        }
    }

    private static void CheckReachedFoodSource(Translation translation, Vector2 foodPos, float foodSourceRadius,
        Direction direction, EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, Entity entity,
        ref URPMaterialPropertyBaseColor color)
    {
        var pos = new Vector2(translation.Value.x, translation.Value.y);
        if (Vector2.Distance(pos, foodPos) < foodSourceRadius)
        {
            color.Value = new float4(1, 1, 0, 0);
            direction.Value = -direction.Value;

            ecb.AddComponent<CarryingFood>(entityInQueryIndex, entity);
        }
    }

    private static void CheckReachedAntHill(Translation translation, Vector2 antHillPosition, float antHillRadius,
        Direction direction, EntityCommandBuffer.ParallelWriter ecb, int entityInQueryIndex, Entity entity,
        ref URPMaterialPropertyBaseColor color)
    {
        var pos = new Vector2(translation.Value.x, translation.Value.y);
        if (Vector2.Distance(pos, antHillPosition) < antHillRadius)
        {
            color.Value = new float4(0.25f, 0.25f, 0.35f, 0);
            direction.Value = -direction.Value;

            ecb.RemoveComponent<CarryingFood>(entityInQueryIndex, entity);
        }
    }

    // Find the points of intersection.
    private static int FindLineCircleIntersections(
        float cx, float cy, float radius,
        float2 point1, float2 point2,
        ref float2 intersection1, ref float2 intersection2)
    {
        float dx, dy, A, B, C, det, t;
        dx = point2.x - point1.x;
        dy = point2.y - point1.y;
        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.x - cx) + dy * (point1.y - cy));
        C = (point1.x - cx) * (point1.x - cx) +
            (point1.y - cy) * (point1.y - cy) -
            radius * radius;
        det = B * B - 4 * A * C;
        if ((A <= 0.0000001) || (det < 0))
        {
            // No real solutions.
            intersection1 = new float2(float.NaN, float.NaN);
            intersection2 = new float2(float.NaN, float.NaN);
            return 0;
        }
        else if (det == 0)
        {
            var numOfIntersections = 1;
            // One solution.
            t = -B / (2 * A);
            intersection1 =
                new float2(point1.x + t * dx, point1.y + t * dy);
            if (!CheckIfIntersectionIsValid(intersection1, point1, point2))
            {
                intersection1 = new float2(float.NaN, float.NaN);
                numOfIntersections--;
            }
            intersection2 = new float2(float.NaN, float.NaN);
            return numOfIntersections;
        }
        else
        {
            var numOfIntersections = 2;
            // Two solutions.
            t = (float)((-B + Mathf.Sqrt(det)) / (2 * A));
            intersection1 =
                new float2(point1.x + t * dx, point1.y + t * dy);
            if (!CheckIfIntersectionIsValid(intersection1, point1, point2))
            {
                intersection1 = new float2(float.NaN, float.NaN);
                numOfIntersections--;
            }
            t = (float)((-B - Mathf.Sqrt(det)) / (2 * A));
            intersection2 =
                new float2(point1.x + t * dx, point1.y + t * dy);
            if (!CheckIfIntersectionIsValid(intersection2, point1, point2))
            {
                intersection2 = new float2(float.NaN, float.NaN);
                numOfIntersections--;
            }
            return numOfIntersections;
        }
    }
    private static bool CheckIfIntersectionIsValid(float2 intersection, float2 lineStart, float2 lineEnd)
    {
        return !((lineStart.x < intersection.x && lineEnd.x < intersection.x) ||
               (lineStart.x > intersection.x && lineEnd.x > intersection.x) ||
               (lineStart.y < intersection.y && lineEnd.y < intersection.y) ||
               (lineStart.y > intersection.y && lineEnd.y > intersection.y));
    }

    private static bool IsTargetVisible(float2 antPos, float2 targetPos, NativeArray<Entity> walls, ComponentDataFromEntity<Wall> wallComponentData)
    {
        bool isOpen = false;
        float lastRadius = -1f;
        foreach (var wall in walls)
        {
            
            var wallData = wallComponentData[wall];
            if (lastRadius < 0)
                lastRadius = wallData.Radius;

            if (Math.Abs(lastRadius - wallData.Radius) < 0.01f)
            {
                if (isOpen)
                    continue;
            }
            else
            {
                lastRadius = wallData.Radius;
                if (!isOpen)
                    return false;
            }

            float2 intersection1 = new float2(float.NaN, float.NaN), intersection2 = new float2(float.NaN, float.NaN);
            int intersectionAmount = FindLineCircleIntersections(0, 0, wallData.Radius, antPos, targetPos, ref intersection1, ref intersection2);
            if (intersectionAmount == 0)
            {
                isOpen = true;
                continue;
            }

            if (intersectionAmount == 1)
            {
                bool isOpening = false;
                if (!float.IsNaN(intersection1.x))
                {
                    float a = math.degrees(math.atan2(intersection1.y, intersection1.x));

                    a = a < 0 ? a + 360 : a % 360;
                    if (wallData.Angles.x < wallData.Angles.y)
                    {
                        if (a > wallData.Angles.x && a < wallData.Angles.y)
                            isOpening = true;
                    }
                    else
                    {
                        if (a > wallData.Angles.x || a < wallData.Angles.y)
                            isOpening = true;
                    }
                } else if (!float.IsNaN(intersection2.x))
                {
                    float a = math.degrees(math.atan2(intersection2.y, intersection2.x));
                    a = a < 0 ? a + 360 : a % 360;
                    if (wallData.Angles.x < wallData.Angles.y)
                    {
                        if (a > wallData.Angles.x && a < wallData.Angles.y)
                            isOpening = true;
                    }
                    else
                    {
                        if (a > wallData.Angles.x || a < wallData.Angles.y)
                            isOpening = true;
                    }
                }
                isOpen = isOpening;
            }else if (intersectionAmount == 2)
            {
                var isOpening1 = false;
                var isOpening2 = false;
                if (!float.IsNaN(intersection1.x))
                {
                    float a = math.degrees(math.atan2(intersection1.y, intersection1.x));
                    a = a < 0 ? a + 360 : a % 360;
                    if (wallData.Angles.x < wallData.Angles.y)
                    {
                        if (a > wallData.Angles.x && a < wallData.Angles.y)
                            isOpening1 = true;
                    }
                    else
                    {
                        if (a > wallData.Angles.x || a < wallData.Angles.y)
                            isOpening1 = true;
                    }
                } 
                if (!float.IsNaN(intersection2.x))
                {
                    float a = math.degrees(math.atan2(intersection2.y, intersection2.x));
                    a = a < 0 ? a + 360 : a % 360;
                    if (wallData.Angles.x < wallData.Angles.y)
                    {
                        if (a > wallData.Angles.x && a < wallData.Angles.y)
                            isOpening2 = true;
                    }
                    else
                    {
                        if (a > wallData.Angles.x || a < wallData.Angles.y)
                            isOpening2 = true;
                    }
                }

                isOpen = isOpening1 && isOpening2;
            }
        }

        return isOpen;
    }

    protected override void OnUpdate()
    {
        const float foodSourceRadius = 2.5f;
        const float antHillRadius = 2.5f;
        
        var random = new Random((uint)(Time.ElapsedTime * 10000000f + 1f));
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
        var halfScreenSize = screenSize / 2.0f;
        var screenUpperBound = (float) screenSize / 2f - 0.5f;
        var screenLowerBound = -screenSize / 2f + 0.5f;
        
        var walls = wallQuery.ToEntityArray(Allocator.TempJob);
        var wallComponentData = GetComponentDataFromEntity<Wall>(true);

       //float2 LineStart = new float2(1, 1);
       //float2 LineEnd = new float2(10, 10);
       //float2 CirclePos = new float2(5, 5);
       //float CircleRadius = 1.0f;
       //float2 intersection1, intersection2;
       //Debug.Log(FindLineCircleIntersections(CirclePos.x, CirclePos.y, CircleRadius, LineStart, LineEnd, ref intersection1, ref intersection2));
       //Debug.Log(intersection1);
       //Debug.Log(intersection2);
        
        var pheromoneMapEntity = GetSingletonEntity<PheromoneMap>();
        var pheromoneMap = GetComponent<PheromoneMap>(pheromoneMapEntity);
        var pheromoneMapFactor = (float)pheromoneMap.gridSize / (float)screenSize;
        var pheromoneBuffer = GetBuffer<Pheromone>(pheromoneMapEntity);

        bool splitJobs = true;
        if (splitJobs)
        {
            Dependency = Entities
                .WithAll<Ant>()
                .WithName("RandomDirectionChange")
                .ForEach((ref Direction direction) =>
                {
                    RandomDirectionChange(ref random, ref direction, simulationSpeed, deltaTime);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant>()
                .WithNone<CarryingFood>()
                .WithName("FollowPheromones_Blue")
                .WithReadOnly(pheromoneBuffer)
                .ForEach((ref Translation translation, ref Direction direction) =>
                {
                    float pheromoneStrength = 0.7f;
                    FollowPheromones(pheromoneMap, pheromoneBuffer, translation, ref direction, direction.Value,
                        halfScreenSize, pheromoneMapFactor, pheromoneStrength);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant, CarryingFood>()
                .WithName("FollowPheromones_Yellow")
                .WithReadOnly(pheromoneBuffer)
                .ForEach((ref Translation translation, ref Direction direction) =>
                {
                    float pheromoneStrength = 1.5f;
                    FollowPheromones(pheromoneMap, pheromoneBuffer, translation, ref direction, direction.Value,
                        halfScreenSize, pheromoneMapFactor, pheromoneStrength);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant>()
                .WithNone<CarryingFood>()
                .WithName("ApproachFoodSource")
                .WithReadOnly(walls)
                .WithReadOnly(wallComponentData)
                .ForEach((ref Translation translation, ref Direction direction) =>
                {
                    ApproachFoodSource(translation, ref direction, foodPos, walls, wallComponentData, deltaTime,
                        simulationSpeed);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant, CarryingFood>()
                .WithName("ApproachTarget_Yellow")
                .WithReadOnly(walls)
                .WithReadOnly(wallComponentData)
                .ForEach((ref Translation translation, ref Direction direction) =>
                {
                    ApproachAntHill(translation, direction, walls, wallComponentData, deltaTime, simulationSpeed);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant>()
                .WithName("WallCollisions")
                .WithReadOnly(walls)
                .WithReadOnly(wallComponentData)
                .WithDisposeOnCompletion(walls)
                .ForEach((ref Translation translation, ref Direction direction) =>
                {
                    HandleWallCollisions(walls, wallComponentData, ref translation, ref direction, direction.Value,
                        ref random, simulationSpeed, deltaTime);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant>()
                .WithNone<CarryingFood>()
                .WithName("CheckReachedFoodSource")
                .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Direction direction,
                    ref URPMaterialPropertyBaseColor color) =>
                {
                    CheckReachedFoodSource(translation, foodPos, foodSourceRadius, direction, ecb, entityInQueryIndex,
                        entity, ref color);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant, CarryingFood>()
                .WithName("CheckReachedAntHill")
                .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Direction direction,
                    ref URPMaterialPropertyBaseColor color) =>
                {
                    CheckReachedAntHill(translation, antHillPosition, antHillRadius, direction, ecb, entityInQueryIndex,
                        entity, ref color);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant>()
                .WithName("ScreenBoundCollisions")
                .ForEach((ref Translation translation, ref Direction direction) =>
                {
                    HandleScreenBoundCollisions(ref translation, ref direction, screenLowerBound, screenUpperBound);
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant>()
                .WithName("MoveForwards")
                .ForEach((ref Translation translation, ref Direction direction) =>
                {
                    // Move ant a step forward in its direction
                    var movementStep = direction.Value * deltaTime * simulationSpeed;
                    translation.Value.x += movementStep.x;
                    translation.Value.y += movementStep.y;
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant>()
                .WithName("UpdateRenderingState")
                .ForEach((ref Direction direction, ref Rotation rotation) =>
                {
                    var angle = math.atan2(direction.Value.y, direction.Value.x);
                    rotation = new Rotation {Value = quaternion.RotateZ(angle)};
                }).ScheduleParallel(Dependency);
        }
        else
        {
            Dependency = Entities
                .WithAll<Ant>()
                .WithNone<CarryingFood>()
                .WithName("BlueAnts")
                .WithReadOnly(pheromoneBuffer)
                .WithReadOnly(walls)
                .WithReadOnly(wallComponentData)
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref Translation translation, ref Direction direction,
                    ref URPMaterialPropertyBaseColor color,
                    ref Rotation rotation) =>
                {
                    float pheromoneStrength = 0.7f;

                    RandomDirectionChange(ref random, ref direction, simulationSpeed, deltaTime);

                    FollowPheromones(pheromoneMap, pheromoneBuffer, translation, ref direction, direction.Value,
                        halfScreenSize, pheromoneMapFactor, pheromoneStrength);
                    ApproachFoodSource(translation, ref direction, foodPos, walls, wallComponentData, deltaTime,
                        simulationSpeed);
                    HandleWallCollisions(walls, wallComponentData, ref translation, ref direction, direction.Value,
                        ref random, simulationSpeed, deltaTime);
                    CheckReachedFoodSource(translation, foodPos, foodSourceRadius, direction, ecb, entityInQueryIndex,
                        entity, ref color);
                    HandleScreenBoundCollisions(ref translation, ref direction, screenLowerBound, screenUpperBound);

                    // Move ant a step forward in its direction
                    var movementStep = direction.Value * deltaTime * simulationSpeed;
                    translation.Value.x += movementStep.x;
                    translation.Value.y += movementStep.y;

                    var angle = math.atan2(direction.Value.y, direction.Value.x);
                    rotation = new Rotation {Value = quaternion.RotateZ(angle)};
                }).ScheduleParallel(Dependency);

            Dependency = Entities
                .WithAll<Ant,CarryingFood>()
                .WithName("YallowAnts")
                .WithReadOnly(pheromoneBuffer)
                .WithReadOnly(walls)
                .WithReadOnly(wallComponentData)
                .WithDisposeOnCompletion(walls)
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref Translation translation, ref Direction direction,
                    ref URPMaterialPropertyBaseColor color,
                    ref Rotation rotation) =>
                {
                    float pheromoneStrength = 1.5f;
                    
                    RandomDirectionChange(ref random, ref direction, simulationSpeed, deltaTime);
                    
                    FollowPheromones(pheromoneMap, pheromoneBuffer, translation, ref direction, direction.Value,
                        halfScreenSize, pheromoneMapFactor, pheromoneStrength);
                    ApproachAntHill(translation, direction, walls, wallComponentData, deltaTime, simulationSpeed);
                    HandleWallCollisions(walls, wallComponentData, ref translation, ref direction, direction.Value,
                        ref random, simulationSpeed, deltaTime);
                    CheckReachedAntHill(translation, antHillPosition, antHillRadius, direction, ecb, entityInQueryIndex,
                        entity, ref color);
                    HandleScreenBoundCollisions(ref translation, ref direction, screenLowerBound, screenUpperBound);

                    // Move ant a step forward in its direction
                    var movementStep = direction.Value * deltaTime * simulationSpeed;
                    translation.Value.x += movementStep.x;
                    translation.Value.y += movementStep.y;

                    var angle = math.atan2(direction.Value.y, direction.Value.x);
                    rotation = new Rotation {Value = quaternion.RotateZ(angle)};
                }).ScheduleParallel(Dependency);
        }

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}