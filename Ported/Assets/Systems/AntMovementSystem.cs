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
        const float maxDirectionChangePerSecond = 5f;

        var maxFrameDirectionChange = maxDirectionChangePerSecond * deltaTime * simulationSpeed;
        var change = random.NextFloat(-maxFrameDirectionChange, maxFrameDirectionChange);
        direction.Radians += change;
    }

    private static float2 CalculateMovementStep(Direction direction, float deltaTime, float simulationSpeed)
    {
        var delta = new float2(math.cos(direction.Radians), math.sin(direction.Radians));
        delta *= deltaTime * simulationSpeed;
        return delta;
    }

    private static bool HandleWallCollisions(NativeArray<Entity> walls, ComponentDataFromEntity<Wall> wallComponentData,
        ref Translation translation, ref Direction direction, float2 movementStep,
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
                    if (math.distance(pos, translation.Value) < halfWallThickness)
                    {
                        shouldCollideFromCorner = true;
                        positionToBounce = pos;
                    }
                    pos = new float3(math.cos(math.radians(wall.Angles.y)) * wall.Radius, math.sin(math.radians(wall.Angles.y)) * wall.Radius, 0f);
                    if (math.distance(pos, translation.Value) < halfWallThickness)
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
                var antPos = translation.Value.xy;

                var nextPos = antPos + movementStep;
                if (math.length(nextPos - collisionPoint) >= math.length(antPos - collisionPoint))
                {
                    // Heading away from the wall
                }
                else
                {
                    direction.Radians += 0.1f * math.sign(direction.Radians - antAngle);
                    return true;
                }
            }
            else
            {
                var collisionPoint = new float2(math.cos(antAngle), math.sin(antAngle));
                var normal = float2.zero - collisionPoint;
                var newDirection = math.reflect(movementStep, normal);
                direction.Radians = math.atan2(newDirection.y, newDirection.x);
                translation.Value.x += newDirection.x;
                translation.Value.y += newDirection.y;
            }
            
        }
        else if (shouldCollideFromCorner)
        {
            var normal3 = positionToBounce - translation.Value;
            var normal = new float2(normal3.x, normal3.y);
            var newDirection = math.reflect(movementStep, normal);
            direction.Radians = math.atan2(newDirection.y, newDirection.x);
            translation.Value.x += newDirection.x;
            translation.Value.y += newDirection.y;
        }

        return false;
    }

    private static void HandleScreenBoundCollisions(ref Translation translation, ref Direction direction, float screenLowerBound, float screenUpperBound)
    {
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
        Translation translation, ref Direction direction, float2 antDirection, float halfScreenSize, float pheromoneMapFactor)
    {
        var antTextureCoord = PheromoneGridLocation(map, translation.Value.xy, halfScreenSize, pheromoneMapFactor);

        // Compute a "pull" direction from each pheromone point in a 5x5 grid around the ant.
        float2 pullDirection = float2.zero;
        const int radius = 2;
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
                if (index < 0)
                {
                    Debug.Log($"Coords: ({x}, {y})");
                }
            
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

        antDirection = pullDirection * 0.2f + antDirection * 0.90f;
        direction.Radians = math.atan2(antDirection.y, antDirection.x);
    }
    
    protected override void OnUpdate()
    {
        const float foodSourceRadius = 2.5f;
        const float antHillRadius = 2.5f;
        
        var random = new Random((uint)(Time.ElapsedTime * 10000000f + 1f));
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var deltaTime = Time.DeltaTime;

        var simulationSpeedEntity = GetSingletonEntity<SimulationSpeed>();
        var simulationSpeed = GetComponent<SimulationSpeed>(simulationSpeedEntity).Value * 2f;

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

        var movementJob = Entities
            .WithAll<Ant>()
            .WithReadOnly(walls)
            .WithReadOnly(wallComponentData)
            .WithReadOnly(pheromoneBuffer)
            .WithName("AntMovement")
            .ForEach((Entity entity, ref Translation translation, ref Direction direction,
                ref Rotation rotation) =>
            {
                RandomDirectionChange(ref random, ref direction, simulationSpeed, deltaTime);

                var movementStep = CalculateMovementStep(direction, deltaTime, simulationSpeed);
                var directionVec = movementStep / math.length(movementStep);


                bool collided = HandleWallCollisions(walls, wallComponentData, ref translation, ref direction, movementStep, ref random, simulationSpeed, deltaTime);
                HandleScreenBoundCollisions(ref translation, ref direction, screenLowerBound, screenUpperBound);
                if (!collided)
                    FollowPheromones(pheromoneMap, pheromoneBuffer, translation, ref direction, directionVec, halfScreenSize, pheromoneMapFactor);
                
                // Move ant a step forward in its direction
                translation.Value.x += movementStep.x;
                translation.Value.y += movementStep.y;
                
                rotation = new Rotation {Value = quaternion.RotateZ(direction.Radians)};
                
            }).ScheduleParallel(Dependency);

        var checkReachedFoodSourceJob = Entities
            .WithAll<Ant>()
            .WithNone<CarryingFood>()
            .WithName("CheckReachedFoodSourceJob")
            .WithReadOnly(walls)
            .WithReadOnly(wallComponentData)
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
                
                if(IsTargetVisible(translation.Value.xy, foodPos, walls, wallComponentData))
                {
                    var movementStep = CalculateMovementStep(direction, deltaTime, simulationSpeed);
                    var directionVec = movementStep / math.length(movementStep);
                    
                    float2 pullDirection = new float2(foodPos.x, foodPos.y) - translation.Value.xy;
                    directionVec = pullDirection * 0.001f + directionVec * 0.90f;
                    direction.Radians = math.atan2(directionVec.y, directionVec.x);
                }
                
            }).ScheduleParallel(movementJob);

        var checkReachedAntHillJob = Entities
            .WithAll<Ant,CarryingFood>()
            .WithName("CheckedReachedAntHill")
            .WithReadOnly(walls)
            .WithReadOnly(wallComponentData)
            .WithDisposeOnCompletion(walls)
            .ForEach((Entity entity, int entityInQueryIndex, ref URPMaterialPropertyBaseColor color,
                ref Direction direction, in Translation translation) =>
            {
                var pos = new Vector2(translation.Value.x, translation.Value.y);
                if (Vector2.Distance(pos, antHillPosition) < antHillRadius)
                {
                    color.Value = new float4(0.25f, 0.25f, 0.35f, 0);
                    direction.Radians += Mathf.PI;
                    
                    ecb.RemoveComponent<CarryingFood>(entityInQueryIndex, entity);
                }

                if (IsTargetVisible(translation.Value.xy, float2.zero, walls, wallComponentData))
                {var movementStep = CalculateMovementStep(direction, deltaTime, simulationSpeed);
                    var directionVec = movementStep / math.length(movementStep);
                    
                    float2 pullDirection = float2.zero - translation.Value.xy;
                    directionVec = pullDirection * 0.001f + directionVec * 0.90f;
                    direction.Radians = math.atan2(directionVec.y, directionVec.x);
                    
                }
                
            }).ScheduleParallel(checkReachedFoodSourceJob);
        
        Dependency = checkReachedAntHillJob;
        m_ECBSystem.AddJobHandleForProducer(Dependency);
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
}