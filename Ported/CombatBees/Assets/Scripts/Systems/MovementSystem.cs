using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Utils;
using Random = Unity.Mathematics.Random;


public partial class MovementSystem : SystemBase
{
    private EntityCommandBufferSystem sys;
    private EntityQuery foodQuery;

    protected override void OnCreate()
    {
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<Spawner>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = UnityEngine.Time.deltaTime;
        var tNow = UnityEngine.Time.timeSinceLevelLoad;
        var spawner = GetSingleton<Spawner>();
        var ecb = sys.CreateCommandBuffer();
        var parallelWriter = ecb.AsParallelWriter();

        var randomSeed = (uint) math.max(1,
            DateTime.Now.Millisecond +
            DateTime.Now.Second +
            DateTime.Now.Minute +
            DateTime.Now.Day +
            DateTime.Now.Month +
            DateTime.Now.Year);

        var random = new Random(randomSeed);

        // Move: Food (move food before bees since bees need to follow food)
        //       - also store all the moved food positions for cross-reference further down
        var foodCount = foodQuery.CalculateEntityCount();
        var foodPositions = new NativeArray<float3>(foodCount, Allocator.TempJob);
        var foodEntities = new NativeArray<Entity>(foodCount, Allocator.TempJob);
        Entities
            .WithStoreEntityQueryInField(ref foodQuery)
            .WithAll<Food>()
            .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref PP_Movement ppMovement,
                ref Velocity velocity,
                in Food food) =>
            {
                if (food.isBeeingCarried)
                {
                    velocity.Value = new float3();
                    translation.Value = ppMovement.Progress(deltaTime, MotionType.BeeBumble);
                }
                else
                {
                    // Falling with gravity
                    ApplyGravityToVelocity(ref velocity, deltaTime);
                    ApplyVelocityToTranslation(ref translation, velocity, deltaTime);

                    // Ground Collision (do this here so that no below-ground food positions are cached)
                    if (GroundCollisionTest(translation))
                    {
                        translation.Value = translation.Value.Floored();
                        velocity.Value = float3.zero;
                        ppMovement.startLocation = ppMovement.endLocation = translation.Value;
                    }
                }

                foodPositions[entityInQueryIndex] = translation.Value;
                foodEntities[entityInQueryIndex] = e;
            }).WithName("GetFoodPositions")
            .ScheduleParallel();

        //bits
        Entities
            .WithAll<BeeBitsTag>()
            .ForEach((Entity e, ref Translation translation, ref Velocity velocity) =>
            {
                // Falling with gravity
                ApplyGravityToVelocity(ref velocity, deltaTime);
                ApplyVelocityToTranslation(ref translation, velocity, deltaTime);
            }).Schedule();

        // Move: anything that hasn't already moved
        Entities
            .WithAll<BeeTag>()
            .ForEach((Entity e, ref Translation translation, ref Rotation rotation, ref PP_Movement ppMovement) =>
            {
                // do bee movement
                translation.Value = ppMovement.Progress(deltaTime, MotionType.BeeBumble);

                float futureT = Mathf.Clamp(ppMovement.t + 0.01f, 0, 1f);

                float3 up = new float3(0, 1f, 0);
                float3 fwd =
                    ppMovement.GetTransAtProgress(ppMovement.t + 0.01f,
                        MotionType.BeeBumble) - translation.Value;

                if (math.distancesq(float3.zero, fwd) < 0.001f)
                    fwd = new float3(-1f, 0, 0);
                else
                    fwd = math.normalize(fwd);

                if ( math.abs(math.dot(fwd, up)) > 0.95f )
                    up = new float3(0, 0, 1);


                var newRot = quaternion.identity;

                newRot = math.mul(newRot, quaternion.RotateZ(math.radians(90)));
                newRot = math.mul(newRot, quaternion.LookRotation(fwd, up));


                rotation.Value = newRot;
            }).Schedule();


        // Collision: Food
        Entities
            .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref Velocity velocity,
                in Food food) =>
            {
                // In a goal area
                if (math.abs(translation.Value.x) >= spawner.ArenaExtents.x)
                {
                    // Collided with Ground
                    if (GroundCollisionTest(translation) && food.isBeeingCarried == false)
                    {
                        // Destroy this food entity because it hit the ground in a goal area
                        ecb.DestroyEntity(e);

                        // Spawn some beeeeeees for the appropriate team!
                        for (var i = 0; i < 3; i++)
                        {
                            var minBeeBounds = SpawnerSystem.GetBeeMinBounds(spawner);
                            var maxBeeBounds = SpawnerSystem.GetBeeMaxBounds(spawner, minBeeBounds);

                            var beeRandomY = SpawnerSystem.GetRandomBeeY(ref random, minBeeBounds, maxBeeBounds);
                            var beeRandomZ = SpawnerSystem.GetRandomBeeZ(ref random, minBeeBounds, maxBeeBounds);

                            if (translation.Value.x > 0)
                            {
                                // Yellow Bees
                                var beeRandomX =
                                    SpawnerSystem.GetRandomYellowBeeX(ref random, minBeeBounds, maxBeeBounds);

                                SpawnerSystem.BufferEntityInstantiation(spawner.YellowBeePrefab,
                                    new float3(beeRandomX, beeRandomY, beeRandomZ),
                                    ref ecb);
                            }
                            else
                            {
                                // Blue Bees
                                var beeRandomX =
                                    SpawnerSystem.GetRandomBlueBeeX(ref random, minBeeBounds, maxBeeBounds);

                                SpawnerSystem.BufferEntityInstantiation(spawner.BlueBeePrefab,
                                    new float3(beeRandomX, beeRandomY, beeRandomZ),
                                    ref ecb);
                            }
                        }
                    }
                }
                // Not in a goal: check for inter-food collisions for stacking
                else
                {
                    for (var i = 0; i < foodCount; i++)
                    {
                        // Check if this food is being checked against itself
                        if (e.Index == foodEntities[i].Index &&
                            e.Version == foodEntities[i].Version)
                        {
                            continue;
                        }

                        // Check if radii overlap first
                        var planarDiffVector = new float2(
                            translation.Value.x - foodPositions[i].x,
                            translation.Value.z - foodPositions[i].z);
                        if (planarDiffVector.x * planarDiffVector.x + planarDiffVector.y * planarDiffVector.y < 1f)
                        {
                            var collisionHeightOverlap = 1f - (translation.Value.y - foodPositions[i].y);

                            // If the height overlaps above the other food, move this one up enough to sit on top
                            if (collisionHeightOverlap > 0.001f &&
                                collisionHeightOverlap <= 0.999f)
                            {
                                translation.Value.y += collisionHeightOverlap + math.EPSILON;
                                velocity.Value = float3.zero;

                                // Once this food moves up to fix a collision, it's done;
                                // if this food is still colliding with a different food,
                                // then that collision can be handled either when it's iterated
                                // or on the next frame.
                                // Meh - not being super picky about clean collision here.
                                break;
                            }
                        }
                    }
                }
            }).WithReadOnly(foodPositions)
            .WithReadOnly(foodEntities)
            .WithDisposeOnCompletion(foodPositions)
            .WithDisposeOnCompletion(foodEntities)
            .Schedule();

        // Collision: Bee Bits
        Entities
            .WithAll<BeeBitsTag>()
            .ForEach((Entity e, int entityInQueryIndex, Translation translation) =>
            {
                // Ground Collision
                if (translation.Value.y < 0.5)
                {
                    //destroy enemy bee
                    parallelWriter.DestroyEntity(entityInQueryIndex, e);
                    Debug.Log("Killed A Bee!");

                    //spawn a blood prefab
                    var bloodEntity = parallelWriter.Instantiate(entityInQueryIndex, spawner.BloodPrefab);

                    parallelWriter.SetComponent(entityInQueryIndex, bloodEntity,
                        new Translation {Value = translation.Value.Floored()});
                }
            }).ScheduleParallel();
        sys.AddJobHandleForProducer(Dependency);
    }

    private static void ApplyGravityToVelocity(ref Velocity velocity, float deltaTime)
    {
        velocity.Value.y -= 9.8f * deltaTime;
        math.clamp(velocity.Value.y, -100f, 100f); // clamp for terminal velocity
    }

    private static void ApplyVelocityToTranslation(ref Translation translation, Velocity velocity, float deltaTime)
    {
        translation.Value += velocity.Value * deltaTime;
    }

    private static bool GroundCollisionTest(Translation translation)
    {
        return translation.Value.y <= 0.5f;
    }
}