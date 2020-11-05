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

    const float dt = 1.0f / 60;
    const float randomSteering = 0.1f;
    const float pheromoneSteerStrength = 0.001f; // Original code used 0.015f;
    const float wallSteerStrength = 0.12f;
    const float antSpeed = 0.2f;
    const float antAccel = 0.07f;

    public static readonly Vector2 bounds = new Vector2(5, 5);

    EntityCommandBufferSystem cmdBufferSystem;

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


        Entities
        .WithReadOnly(obstaclesPositions)
        .WithReadOnly(pheromonesArray)
        .ForEach((int entityInQueryIndex, ref Translation translation, ref Direction direction, ref RandState rand, ref Speed speed, in Entity antEntity) =>
        {
            float dx = Mathf.Cos(direction.Value) * speed.Value * dt;
            float dy = Mathf.Sin(direction.Value) * speed.Value * dt;

            // Pseudo-random steering
            direction.Value += rand.Random.NextFloat(-randomSteering, randomSteering);

            //pheromone steering
            float pheroSteering = PheremoneSteering(pheromonesArray, translation, direction, 0.3f);
            var wallSteering = WallSteering(translation.Value, direction.Value, 0.15f, obstaclesPositions);
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
            else if (isLookingForNest)
            {
                target3D = spawner.ColonyPosition;
                targetRadius = spawner.ColonyRadius;
            }

            var target2D = new float2(target3D.x, target3D.z);
            SteeringTowardTarget(ref translation, ref direction, target2D, spawner, obstaclesPositions);

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

            // Bounce off the edges of the board (for now the ant bounces back, maybe improve later)
            if (Mathf.Abs(translation.Value.x + dx) > bounds.x)
            {
                dx = -dx;
                direction.Value += Mathf.PI;
            }

            if (Mathf.Abs(translation.Value.z + dy) > bounds.y)
            {
                dy = -dy;
                direction.Value += Mathf.PI;
            }

            direction.Value = (direction.Value >= 2 * Mathf.PI) ? direction.Value - 2 * Mathf.PI : direction.Value;

            ObstacleAvoid(ref translation, ref direction, spawner.ObstacleRadius, obstaclesPositions);

            translation.Value.x += (float)dx;
            translation.Value.z += (float)dy;

        })
        .ScheduleParallel();

        // Our jobs must finish before the EndSimulationEntityCommandBufferSystem execute the changements we recorded
        cmdBufferSystem.AddJobHandleForProducer(this.Dependency);
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

    static void SteeringTowardTarget(ref Translation translation, ref Direction direction, float2 target, in AntSpawner spawner, in DynamicBuffer<ObstaclePosition> obstaclePositions)
    {
        var targetRadius = spawner.FoodRadius;

        var antPos = translation.Value.xz;
        var antDirection = direction.Value;

        if (!RayCast(antPos, target, targetRadius, spawner.ObstacleRadius, obstaclePositions))
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
        }

        translation.Value.x = antPos.x;
        translation.Value.z = antPos.y;
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

            if(TextureHelper.PositionWithInMapBounds(new Vector2(testX, testY)))
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
}
