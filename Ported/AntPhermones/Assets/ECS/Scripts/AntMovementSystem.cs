#define USE_PRECOMPUTED_RAYCAST
#define USE_PRECOMPUTED_DISTANCE_FIELD
//#define USE_OBSTACLE_AVOIDANCE
#define WALL_STEERING_EARLY_EXIT

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;

public struct PheromoneDecayPassJob : IJobParallelFor
{
    public const float decay = 0.9995f; // Original code used 0.9985f;
    public const float TexSize = RefsAuthoring.TexSize; // Original code used 0.9985f;

    public NativeArray<PheromonesBufferData> localPheromones;

    public void Execute(int index)
    {
        localPheromones[index] *= decay;
    }
}

public class AntMovementSystem : SystemBase
{
    const float dt = 3.0f / 60;
    const float randomSteering = 0.1f;
    const float pheromoneSteerStrength = 0.008f; // Original code used 0.015f;
    const float wallSteerStrength = 0.12f;
    const float antSpeed = 0.2f;
    const float antAccel = 0.07f;
    const float outwardStrength = 0.001f; // Original code use 0.003f
    const float inwardStrength = 0.001f; // Original code use 0.003f
    const float trailAddSpeed = 0.3f;
    const float excitementWhenWandering = 0.3f;
    const float excitementWithTargetInSight = 1.0f;

    public static readonly float2 bounds = new float2(5, 5);

    EntityCommandBufferSystem cmdBufferSystem;

    // RayCast precomputation
    const int numCells = 16;
    static NativeArray<bool> rayCastMapFood;
    static NativeArray<bool> rayCastMapColony;
    static bool isRayCastInitialized = false;
    static readonly float cellSize = 2 * bounds.x / numCells;
    static readonly float oneOverCellSize = 1.0f / cellSize;

    // distance-field precomputation
    static NativeArray<float> distanceField;
    const int numCellsDF = 256;
    static readonly float cellSizeDF = 2 * bounds.x / numCellsDF;
    static readonly float oneOverCellSizeDF = 1.0f / cellSizeDF;

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
        var pheromonesArray = GetBuffer<PheromonesBufferData>(pheromoneDataEntity).AsNativeArray();
        var spawner = GetComponent<AntSpawner>(spawnerEntity);
        var obstaclesPositions = GetBuffer<ObstaclePosition>(spawnerEntity);

        if (isRayCastInitialized == false)
        {
            PrecomputeRayCast(obstaclesPositions, spawner.ColonyPosition, spawner.FoodPosition, spawner.ColonyRadius, spawner.FoodRadius, spawner.ObstacleRadius);
            PrecomputeDistanceField(obstaclesPositions, spawner.ObstacleRadius);
            isRayCastInitialized = true;
        }

        var rayCastColonyArray = rayCastMapColony;
        var rayCastFoodArray = rayCastMapFood;
        var distanceFieldArray = distanceField;

        // First process the ants looking for food

        var target3D = spawner.FoodPosition;
        var targetRadius = spawner.FoodRadius;
        var target2D = new float2(target3D.x, target3D.z);

        Entities
        .WithNativeDisableParallelForRestriction(pheromonesArray)
        .WithReadOnly(obstaclesPositions)
        .WithReadOnly(rayCastFoodArray)
        .WithReadOnly(distanceFieldArray)
        .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand,
            ref HasTargetInSight hasTargetInSight, ref Speed speed, in Entity antEntity, in AntLookingForFood dummy) =>
        {
            // Pseudo-random steering
            direction.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

            //pheromone steering
            float pheroSteering = PheremoneSteering(pheromonesArray, translation, direction, 1.0f);
            var wallSteering = WallSteering(translation.Value, direction.Value, 0.2f, obstaclesPositions, distanceFieldArray);
            var targetSpeed = antSpeed;
            targetSpeed *= 1f - (math.abs(pheroSteering) + math.abs(wallSteering)) / 3f;
            direction.Value += pheroSteering * pheromoneSteerStrength;
            direction.Value += wallSteering * wallSteerStrength;

            speed.Value += (targetSpeed - speed.Value) * antAccel;

            var excitement = hasTargetInSight.Value ? excitementWithTargetInSight : excitementWhenWandering;
            DropPheromones(translation.Value.x, translation.Value.z, bounds, pheromonesArray, speed.Value, dt, RefsAuthoring.TexSize, excitement);

            // TODO: steer towards target

            SteeringTowardTarget(ref direction, ref hasTargetInSight, translation, target2D, 
                spawner, obstaclesPositions, rayCastFoodArray);

            if (HasReachedTarget(translation.Value, target3D, targetRadius))
            {
                cmd.RemoveComponent<AntLookingForFood>(entityInQueryIndex, antEntity);
                cmd.AddComponent<AntLookingForNest>(entityInQueryIndex, antEntity);
                cmd.AddComponent<RequireColourUpdate>(entityInQueryIndex, antEntity);

                // Uturn
                direction.Value += math.PI;
            }

            var d = float2.zero;
            d.x = math.cos(direction.Value) * speed.Value * dt;
            d.y = math.sin(direction.Value) * speed.Value * dt;

            direction.Value = (direction.Value >= 2 * math.PI) ? direction.Value - 2 * math.PI : direction.Value;

#if USE_OBSTACLE_AVOIDANCE
            ObstacleAvoid(ref translation, ref direction, spawner.ObstacleRadius, obstaclesPositions);
#endif
            SteerTowardColony(ref d, translation.Value, spawner.ColonyPosition, spawner.MapSize, false);

            // Bounce off the edges of the board (for now the ant bounces back, maybe improve later)
            if (math.abs(translation.Value.x + d.x) > bounds.x)
            {
                d.x = -d.x;
                direction.Value += math.PI;
            }

            if (math.abs(translation.Value.z + d.y) > bounds.y)
            {
                d.y = -d.y;
                direction.Value += math.PI;
            }

            translation.Value.x += (float)d.x;
            translation.Value.z += (float)d.y;

        })
        .ScheduleParallel();


        target3D = spawner.ColonyPosition;
        targetRadius = spawner.ColonyRadius;
        target2D = new float2(target3D.x, target3D.z);

        Entities
       .WithNativeDisableParallelForRestriction(pheromonesArray)
       .WithReadOnly(obstaclesPositions)
       .WithReadOnly(rayCastColonyArray)
       .WithReadOnly(distanceFieldArray)
       .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand,
           ref HasTargetInSight hasTargetInSight, ref Speed speed, in Entity antEntity, in AntLookingForNest dummy) =>
       {
           // Pseudo-random steering
           direction.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

           //pheromone steering
           float pheroSteering = PheremoneSteering(pheromonesArray, translation, direction, 1.0f);
           var wallSteering = WallSteering(translation.Value, direction.Value, 0.2f, obstaclesPositions, distanceFieldArray);
           var targetSpeed = antSpeed;
           targetSpeed *= 1f - (math.abs(pheroSteering) + math.abs(wallSteering)) / 3f;
           direction.Value += pheroSteering * pheromoneSteerStrength;
           direction.Value += wallSteering * wallSteerStrength;

           speed.Value += (targetSpeed - speed.Value) * antAccel;

           var excitement = hasTargetInSight.Value ? excitementWithTargetInSight : excitementWhenWandering;
           DropPheromones(translation.Value.x, translation.Value.z, bounds, pheromonesArray, speed.Value, dt, RefsAuthoring.TexSize, excitement);

           SteeringTowardTarget(ref direction, ref hasTargetInSight, translation, target2D,
               spawner, obstaclesPositions, rayCastColonyArray);

           if (HasReachedTarget(translation.Value, target3D, targetRadius))
           {
               cmd.RemoveComponent<AntLookingForNest>(entityInQueryIndex, antEntity);
               cmd.AddComponent<AntLookingForFood>(entityInQueryIndex, antEntity);
               cmd.AddComponent<RequireColourUpdate>(entityInQueryIndex, antEntity);
               // Uturn
               direction.Value += math.PI;
           }

           var d = float2.zero;
           d.x = math.cos(direction.Value) * speed.Value * dt;
           d.y = math.sin(direction.Value) * speed.Value * dt;

           direction.Value = (direction.Value >= 2 * math.PI) ? direction.Value - 2 * math.PI : direction.Value;

#if USE_OBSTACLE_AVOIDANCE
            ObstacleAvoid(ref translation, ref direction, spawner.ObstacleRadius, obstaclesPositions);
#endif
           SteerTowardColony(ref d, translation.Value, spawner.ColonyPosition, spawner.MapSize, true);

           // Bounce off the edges of the board (for now the ant bounces back, maybe improve later)
           if (math.abs(translation.Value.x + d.x) > bounds.x)
           {
               d.x = -d.x;
               direction.Value += math.PI;
           }

           if (math.abs(translation.Value.z + d.y) > bounds.y)
           {
               d.y = -d.y;
               direction.Value += math.PI;
           }

           translation.Value.x += (float)d.x;
           translation.Value.z += (float)d.y;

       })
       .ScheduleParallel();

        var decayJob = new PheromoneDecayPassJob
        {
            localPheromones = pheromonesArray
        }
        .Schedule(pheromonesArray.Length, 128, this.Dependency);

        // Our jobs must finish before the EndSimulationEntityCommandBufferSystem execute the changements we recorded
        cmdBufferSystem.AddJobHandleForProducer(decayJob);
    }

    void PrecomputeRayCast(DynamicBuffer<ObstaclePosition> obstaclePositions, NativeArray<bool> map, float2 target, float targetRadius, float obstacleRadius)
    {
        float x0 = -bounds.x;

        for (int i = 0; i < numCells; i++)
        {
            for (int j = 0; j < numCells; j++)
            {
                float2 cellPosition = new float2(x0 + (i + 0.5f) * cellSize, x0 + (j + 0.5f) * cellSize);
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

    void PrecomputeDistanceField(DynamicBuffer<ObstaclePosition> obstaclePositions, float obstacleRadius)
    {
        distanceField = new NativeArray<float>(numCellsDF * numCellsDF, Allocator.Persistent);
        float x0 = -bounds.x;

        for (int i = 0; i < numCellsDF; i++)
        {
            for (int j = 0; j < numCellsDF; j++)
            {
                float2 cellPosition = new float2(x0 + (i + 0.5f) * cellSizeDF, x0 + (j + 0.5f) * cellSizeDF);
                float shortestDistance = 1000000000f;
                for (int k = 0; k < obstaclePositions.Length; ++k)
                {
                    float2 obstaclePosition = obstaclePositions[k].Value.xz;
                    float distance = math.distancesq(cellPosition, obstaclePosition);
                    shortestDistance = distance < shortestDistance ? distance : shortestDistance;
                }
                distanceField[j * numCellsDF + i] = shortestDistance; 
            }
        }
    }

    public static float FetchDistance(float2 from, NativeArray<float> distanceField)
    {
        int indexX = (int)((from.x + bounds.x) * oneOverCellSizeDF);
        int indexY = (int)((from.y + bounds.y) * oneOverCellSizeDF);
        return distanceField[indexY * numCellsDF + indexX];
    }

    // Precomputed ray-cast for run-time
    public static bool RayCastPrecomputed(float2 from, NativeArray<bool> map)
    {
        int indexX = (int)((from.x + bounds.x) * oneOverCellSize);
        int indexY = (int)((from.y + bounds.y) * oneOverCellSize);
        return map[indexY * numCells + indexX];
    }

    static float WallSteering(float3 position3D, float direction, float distance, in DynamicBuffer<ObstaclePosition> obstaclePositions, in NativeArray<float> distancefield)
    {
        var position = position3D.xz;

#if WALL_STEERING_EARLY_EXIT
        // There are no wall at the middle and at the outer region of the map
        const float innerRadiusSq = 0.5f;
        const float outerRadiusSq = 16f;
        float lengthSq = math.lengthsq(position);
        if (lengthSq < innerRadiusSq || lengthSq > outerRadiusSq)
        {
            return 0;
        }
#endif

        int nbClose = 0;
        for (int k = -1; k <= 1; k += 2)
        {
            float angle = direction + k * math.PI * .25f;
            var test = float2.zero;
            test.x = position.x + math.cos(angle) * distance;
            test.y = position.y + math.sin(angle) * distance;

#if USE_PRECOMPUTED_DISTANCE_FIELD
            var distanceSq = FetchDistance(test, distancefield);
            {
                if (distanceSq < distance * distance)
#else
            for (int i = 0; i < obstaclePositions.Length; ++i)
            {
                var obstaclePos = obstaclePositions[i].Value.xz;
                if (math.distancesq(test, obstaclePos) < distance * distance)
#endif
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
            if (targetAngle - antDirection > math.PI)
            {
                antDirection += math.PI * 2f;
            }
            else if (targetAngle - antDirection < -math.PI)
            {
                antDirection -= math.PI * 2f;
            }
            else
            {
                if (math.abs(targetAngle - antDirection) < math.PI * .5f)
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
            float angle = antDirection.Value + i * math.PI * 0.25f;
            float testX = antTranslation.Value.x + math.cos(angle) * distance;
            float testY = antTranslation.Value.z + math.sin(angle) * distance;

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

        float inwardOrOutward = -outwardStrength;
        float pushRadius = mapSize * .4f;
        if (isLookingForNest)
        {
            inwardOrOutward = inwardStrength;
            pushRadius = mapSize;
        }
        var dx = colonyPosition.x - position.x;
        var dy = colonyPosition.y - position.y;
        var dist = math.sqrt(dx * dx + dy * dy);
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
                dir.Value += math.PI;
                dir.Value = (dir.Value >= 2 * math.PI) ? dir.Value - 2 * math.PI : dir.Value;


                //Move ant out of collision
                dist = math.sqrt(sqrDist);
                dx /= dist;
                dy /= dist;
                translation.Value.x = obstaclePositions[i].Value.x + dx * obstacleRadius;
                translation.Value.z = obstaclePositions[i].Value.z + dy * obstacleRadius;
            }
        }
    }

    public static void DropPheromones(float x, float y, float2 bounds, NativeArray<PheromonesBufferData> localPheromones, float speed, float dt, int TexSize, float excitement)
    {
        float2 texelCoord = new float2(0.5f * (-x / bounds.x) + 0.5f, 0.5f * (-y / bounds.y) + 0.5f);
        int index = (int)(texelCoord.y * TexSize) * TexSize + (int)(texelCoord.x * TexSize);

        if (index >= localPheromones.Length || index < 0) return;

        excitement *= speed / antSpeed;

        var pheromone = (float)localPheromones[index];
        pheromone += (trailAddSpeed * excitement * dt) * (1f - pheromone);
        if (pheromone > 1f)
        {
            pheromone = 1f;
        }

        localPheromones[index] = pheromone;
    }

    protected override void OnDestroy()
    {
        rayCastMapFood.Dispose();
        rayCastMapColony.Dispose();
        isRayCastInitialized = false;

        distanceField.Dispose();
    }
}
