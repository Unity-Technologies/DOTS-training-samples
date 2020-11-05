#define USE_PRECOMPUTED_RAYCAST

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

public class AntMovementSystem : SystemBase
{
    struct DataInputs
    {

    }

    const float dt = 3.0f / 60;
    const float randomSteering = 0.1f;
    const float pheromoneSteerStrength = 0.008f; // Original code used 0.015f;
    const float wallSteerStrength = 0.12f;
    const float antSpeed = 0.2f;
    const float antAccel = 0.07f;
    const float outwardStrength = 0.001f; // Original code use 0.003f
    const float inwardStrength = 0.001f; // Original code use 0.003f

    public static readonly float2 bounds = new float2(5, 5);

    EntityCommandBufferSystem cmdBufferSystem;

    // RayCast precomputation
    const int numCells = 16;
    static NativeArray<bool> rayCastMapFood;
    static NativeArray<bool> rayCastMapColony;
    static bool isRayCastInitialized = false;
    static readonly float cellSize = 2 * bounds.x / numCells;
    static readonly float oneOverCellSize = 1.0f / cellSize;

    protected override void OnCreate()
    {
        cmdBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<TexSingleton>();
    }

    protected override void OnUpdate()
    {
        var cmd = cmdBufferSystem.CreateCommandBuffer().AsParallelWriter();

        //[TO DO] Improve this, its a crude way to "wait" for the texture initaliser to be done
        EntityQuery doesTextInitExist = GetEntityQuery(typeof(TexInitialiser));
        if (!doesTextInitExist.IsEmpty)
        {
            return;
        }


        var spawnerEntity = GetSingletonEntity<AntSpawner>();
        var pheromoneDataEntity = GetSingletonEntity<TexSingleton>();
        var spawner = GetComponent<AntSpawner>(spawnerEntity);
        var obstaclesPositions = GetBuffer<ObstaclePosition>(spawnerEntity);
        var pheromonesArray = GetBuffer<PheromonesBufferData>(pheromoneDataEntity).AsNativeArray();

        if (isRayCastInitialized == false)
        {
            PrecomputeRayCast(obstaclesPositions, spawner.ColonyPosition, spawner.FoodPosition, spawner.ColonyRadius, spawner.FoodRadius, spawner.ObstacleRadius);
            isRayCastInitialized = true;
        }

        var rayCastColonyArray = rayCastMapColony;
        var rayCastFoodArray = rayCastMapFood;

        Entities
        .WithReadOnly(obstaclesPositions)
        .WithReadOnly(pheromonesArray)
        .WithReadOnly(rayCastColonyArray)
        .WithReadOnly(rayCastFoodArray)
        .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand,
            ref HasTargetInSight hasTargetInSight, ref Speed speed, in Entity antEntity) =>
        {
            // Pseudo-random steering
            direction.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

            //pheromone steering
            float pheroSteering = PheremoneSteering(pheromonesArray, translation, direction, 1.0f);
            var wallSteering = WallSteering(translation.Value, direction.Value, 0.2f, obstaclesPositions);
            var targetSpeed = antSpeed;
            targetSpeed *= 1f - (Mathf.Abs(pheroSteering) + Mathf.Abs(wallSteering)) / 3f;
            direction.Value += pheroSteering * pheromoneSteerStrength;
            direction.Value += wallSteering * wallSteerStrength;

            speed.Value += (targetSpeed - speed.Value) * antAccel;


            // TODO: steer towards target

            var target3D = float3.zero;
            var targetRadius = 0f;
            var isLookingForFood = HasComponent<AntLookingForFood>(antEntity);
            var isLookingForNest = HasComponent<AntLookingForNest>(antEntity);

            if (isLookingForFood)
            {
                target3D = spawner.FoodPosition;
                targetRadius = spawner.FoodRadius;
            }
            else
            {
                target3D = spawner.ColonyPosition;
                targetRadius = spawner.ColonyRadius;
            }

            var target2D = new float2(target3D.x, target3D.z);
            SteeringTowardTarget(ref direction, ref hasTargetInSight, translation, target2D, 
                spawner, obstaclesPositions, isLookingForFood ? rayCastFoodArray : rayCastColonyArray);

            if (HasReachedTarget(translation.Value, target3D, targetRadius))
            {
                if (isLookingForFood)
                {
                    cmd.RemoveComponent<AntLookingForFood>(entityInQueryIndex, antEntity);
                    cmd.AddComponent<AntLookingForNest>(entityInQueryIndex, antEntity);
                    cmd.AddComponent<RequireColourUpdate>(entityInQueryIndex, antEntity);
                }
                else
                {
                    cmd.RemoveComponent<AntLookingForNest>(entityInQueryIndex, antEntity);
                    cmd.AddComponent<AntLookingForFood>(entityInQueryIndex, antEntity);
                    cmd.AddComponent<RequireColourUpdate>(entityInQueryIndex, antEntity);
                }

                // Uturn
                direction.Value += math.PI;
            }

            var d = float2.zero;
            d.x = Mathf.Cos(direction.Value) * speed.Value * dt;
            d.y = Mathf.Sin(direction.Value) * speed.Value * dt;

            direction.Value = (direction.Value >= 2 * Mathf.PI) ? direction.Value - 2 * Mathf.PI : direction.Value;

            ObstacleAvoid(ref translation, ref direction, spawner.ObstacleRadius, obstaclesPositions);

            SteerTowardColony(ref d, translation.Value, spawner.ColonyPosition, spawner.MapSize, isLookingForNest);

            // Bounce off the edges of the board (for now the ant bounces back, maybe improve later)
            if (Mathf.Abs(translation.Value.x + d.x) > bounds.x)
            {
                d.x = -d.x;
                direction.Value += Mathf.PI;
            }

            if (Mathf.Abs(translation.Value.z + d.y) > bounds.y)
            {
                d.y = -d.y;
                direction.Value += Mathf.PI;
            }

            translation.Value.x += (float)d.x;
            translation.Value.z += (float)d.y;

        })
        .ScheduleParallel();

        // Our jobs must finish before the EndSimulationEntityCommandBufferSystem execute the changements we recorded
        cmdBufferSystem.AddJobHandleForProducer(this.Dependency);
    }

    void PrecomputeRayCast(DynamicBuffer<ObstaclePosition> obstaclePositions, NativeArray<bool> map, float2 target, float targetRadius, float obstacleRadius)
    {
        float x0 = -bounds.x;

        for (int i = 0; i < numCells; i++)
        {
            for (int j = 0; j < numCells; j++)
            {
                float2 cellPosition = new float2(x0 + i * cellSize, x0 + j * cellSize);
                map[j * numCells + i] = RayCast(cellPosition, target, targetRadius, obstacleRadius + 0.2f, obstaclePositions);  // Note: the +0.2f was also in the runtime code
            }
        }
    }

    void PrecomputeRayCast(DynamicBuffer<ObstaclePosition> obstaclePositions, Vector3 colonyPosition, Vector3 foodPosition, float colonyRadius, float foodRadius, float obstacleRadius)
    {
        rayCastMapFood = new NativeArray<bool>(numCells * numCells, Allocator.Persistent);
        rayCastMapColony = new NativeArray<bool>(numCells * numCells, Allocator.Persistent);
        PrecomputeRayCast(obstaclePositions, rayCastMapFood, new float2(foodPosition.x, foodPosition.z), foodRadius, obstacleRadius);
        PrecomputeRayCast(obstaclePositions, rayCastMapColony, new float2(colonyPosition.x, colonyPosition.z), colonyRadius, obstacleRadius);
    }

    // Precomputed ray-cast for run-time
    public static bool RayCastPrecomputed(float2 from, NativeArray<bool> map)
    {
        int indexX = (int)((from.x + bounds.x) * oneOverCellSize);
        int indexY = (int)((from.y + bounds.y) * oneOverCellSize);
        return map[indexY * numCells + indexX];
    }

    static float WallSteering(float3 position3D, float direction, float distance, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        var position = position3D.xz;

        int nbClose = 0;
        for (int k = -1; k <= 1; k += 2)
        {
            float angle = direction + k * Mathf.PI * .25f;
            var test = float2.zero;
            test.x = position.x + Mathf.Cos(angle) * distance;
            test.y = position.y + Mathf.Sin(angle) * distance;

            for (int i = 0; i < obstaclePositions.Length; ++i)
            {
                var obstaclePos = obstaclePositions[i].Value.xz;
                if (math.distancesq(test, obstaclePos) < distance * distance)
                {
                    --nbClose;
                    break;
                }
            }
        }


        return (float)nbClose;
    }

    static void SteeringTowardTarget(ref Direction direction, ref HasTargetInSight hasTargetInSight, in Translation translation, float2 target, in AntSpawner spawner, in DynamicBuffer<ObstaclePosition> obstaclePositions, in NativeArray<bool> map)
    {
        var targetRadius = spawner.FoodRadius;

        var antPos = translation.Value.xz;
        var antDirection = direction.Value;

#if USE_PRECOMPUTED_RAYCAST
        bool raycast = RayCastPrecomputed(antPos, map); // precomputed version
#else
        bool raycast = RayCast(antPos, target, targetRadius, spawner.ObstacleRadius + 0.2f, obstaclePositions);
#endif

        if (!raycast)
        {
            float targetAngle = Mathf.Atan2(target.y - antPos.y, target.x - antPos.x);
            if (targetAngle - antDirection > Mathf.PI)
            {
                antDirection += Mathf.PI * 2f;
            }
            else if (targetAngle - antDirection < -Mathf.PI)
            {
                antDirection -= Mathf.PI * 2f;
            }
            else
            {
                if (Mathf.Abs(targetAngle - antDirection) < Mathf.PI * .5f)
                {
                    antDirection += (targetAngle - antDirection) * spawner.GoalSteerStrength;
                }
            }

            hasTargetInSight.Value = true;
        }
        else
        {
            hasTargetInSight.Value = false;
        }

        direction.Value = antDirection;
    }

    public static bool RayCast(float2 from, float2 to, float targetRadius, float obstacleRadius, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        var targetRadiusSq = targetRadius * targetRadius;
        var mainLine = to - from;
        var mainLengthSq = math.lengthsq(mainLine);

        // Skipping computations if the ant is in the target. Consider that the ray does not hit anything.
        if (mainLengthSq < targetRadiusSq)
        {
            return false;
        }

        var mainLength = math.sqrt(mainLengthSq);
        var mainDirection = mainLine / mainLength;

        // A 2D normal is easy to compute with this trick
        var mainNormal = new float2(-mainDirection.y, mainDirection.x);

        for (int i = 0; i < obstaclePositions.Length; ++i)
        {
            var obstaclePos = obstaclePositions[i].Value;
            var delta = obstaclePos.xz - from;

            // Do not take the result of the intersection into account if the obstacle is out of range
            var isRayInRange = math.lengthsq(delta) <= mainLengthSq;
            if (!isRayInRange) continue;

            // Ignore obstacles behind
            var isObstacleBehind = math.dot(delta, mainDirection) < 0;
            if (isObstacleBehind) continue;

            // Vector projection to get the distance between the main line and the obstacle position
            var dot = math.dot(delta, mainNormal);

            var isCrossingMainLine = math.abs(dot) < obstacleRadius;
            if (isCrossingMainLine) return true;
        }

        return false;
    }

    static bool HasReachedTarget(float3 position, float3 target, float targetRadius)
    {
        return math.distancesq(position, target) < (targetRadius * targetRadius);
    }


    private static float PheremoneSteering(in NativeArray<PheromonesBufferData> pheromonesData,
                                           in Translation antTranslation, in Direction antDirection, in float distance)
    {
        float output = 0f;

        for (int i = -1; i <= 1; i += 2)
        {
            float angle = antDirection.Value + i * Mathf.PI * 0.25f;
            float testX = antTranslation.Value.x + Mathf.Cos(angle) * distance;
            float testY = antTranslation.Value.z + Mathf.Sin(angle) * distance;

            if (TextureHelper.PositionWithInMapBounds(new float2(testX, testY)))
            {
                var testTranslation = new Translation() { Value = new float3(testX, 0, testY) };
                int index = TextureHelper.GetTextureArrayIndexFromTranslation(testTranslation);

                //Get value from the pheremones array array
                float value = pheromonesData[index].Value;
                output += value * i;
            }
        }
        return Mathf.Sign(output);
    }

    public static void SteerTowardColony(ref float2 outSpeed, float3 position3D, float3 colonyPosition, float mapSize, bool isLookingForNest)
    {
        var position = position3D.xz;

        mapSize = 10f;
        float inwardOrOutward = -outwardStrength;
        float pushRadius = mapSize * .4f;
        if (isLookingForNest)
        {
            inwardOrOutward = inwardStrength;
            pushRadius = mapSize;
        }
        var dx = colonyPosition.x - position.x;
        var dy = colonyPosition.y - position.y;
        var dist = Mathf.Sqrt(dx * dx + dy * dy);
        inwardOrOutward *= 1f - Mathf.Clamp01(dist / pushRadius);
        outSpeed.x += dx / dist * inwardOrOutward;
        outSpeed.y += dy / dist * inwardOrOutward;
    }

    static void ObstacleAvoid(ref Translation translation, ref Direction dir, float obstacleRadius, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        // Original code
        //for (int j=0;j<nearbyObstacles.Length;j++) 
        //{
        //    Obstacle obstacle = nearbyObstacles[j];
        //    dx = ant.position.x - obstacle.position.x;
        //    dy = ant.position.y - obstacle.position.y;
        //    float sqrDist = dx * dx + dy * dy;
        //
        //    if (sqrDist<obstacleRadius*obstacleRadius) 
        //    {
        //	    dist = Mathf.Sqrt(sqrDist);
        //	    dx /= dist;
        //	    dy /= dist;
        //	    ant.position.x = obstacle.position.x + dx * obstacleRadius;
        //	    ant.position.y = obstacle.position.y + dy * obstacleRadius;
        //
        //	    vx -= dx * (dx * vx + dy * vy) * 1.5f;
        //	    vy -= dy * (dx * vx + dy * vy) * 1.5f;
        //    }
        //}

        //Check this entity for collisions with all other entites
        float dx, dy, sqrDist, dist;
        for (int i = 0; i < obstaclePositions.Length; ++i)
        {
            //Get difference in x and y, calculate the sqrd distance to the 
            dx = translation.Value.x - obstaclePositions[i].Value.x;
            dy = translation.Value.z - obstaclePositions[i].Value.z;
            sqrDist = (dx * dx) + (dy * dy);

            //If we are less than the sqrd distance away from the obstacle then reflect the ant
            if (sqrDist < (obstacleRadius * obstacleRadius))
            {
                //Reflect
                dir.Value += Mathf.PI;
                dir.Value = (dir.Value >= 2 * Mathf.PI) ? dir.Value - 2 * Mathf.PI : dir.Value;


                //Move ant out of collision
                dist = Mathf.Sqrt(sqrDist);
                dx /= dist;
                dy /= dist;
                translation.Value.x = obstaclePositions[i].Value.x + dx * obstacleRadius;
                translation.Value.z = obstaclePositions[i].Value.z + dy * obstacleRadius;
            }
        }
    }
    protected override void OnDestroy()
    {
        rayCastMapFood.Dispose();
        rayCastMapColony.Dispose();
        isRayCastInitialized = false;
    }
}
