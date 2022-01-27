using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
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
                    velocity.Value.y -= 9.8f * deltaTime;
                    math.clamp(velocity.Value.y, -100f, 100f); // clamp for terminal velocity
                    translation.Value += velocity.Value * deltaTime;

                    // Ground Collision (do this here so that no below-ground food positions are cached)
                    if (translation.Value.y < 0.5f)
                    {
                        translation.Value.y = 0.5f;
                        ppMovement.startLocation = ppMovement.endLocation = translation.Value;
                    }
                }

                foodPositions[entityInQueryIndex] = translation.Value;
                foodEntities[entityInQueryIndex] = e;
            }).WithName("GetFoodPositions")
            .ScheduleParallel();

        // TODO: Can we parallelize movement by scheduling the translation updates
        //       and distance checks separately?  Should let us ues ScheduleParallel()
        //       and WithNativeDisableParallelForRestriction(), yeah?

        //bits
        Entities
            .WithAll<BeeBitsTag>()
            .ForEach((Entity e, ref Translation translation, ref Velocity velocity) =>
            {
                // Falling with gravity
                velocity.Value.y -= 9.8f * deltaTime;
                math.clamp(velocity.Value.y, -100f, 100f); // clamp for terminal velocity
                translation.Value += velocity.Value * deltaTime;
            }).Schedule();

        // Scale: Blood
        // TODO: that^
        // Update: Bees
        // TODO: update bee "end position" to match its target food in "foodPositionData"

        // Move: anything that hasn't already moved
        Entities
            .WithNone<BeeBitsTag, BloodTag, Food>()
            .ForEach((Entity e, ref Translation translation, ref Rotation rotation, ref PP_Movement ppMovement) =>
            {
                // do bee movement
                translation.Value = ppMovement.Progress(deltaTime, MotionType.BeeBumble);

                float futureT = Mathf.Clamp(ppMovement.t + 0.01f, 0, 1f);

                float3 fwd =
                    ppMovement.GetTransAtProgress(ppMovement.t + 0.01f / ppMovement.timeToTravel,
                        MotionType.BeeBumble) - translation.Value;

                var newRot = quaternion.identity;

                newRot = math.mul(newRot, quaternion.RotateZ(math.radians(90)));
                newRot = math.mul(newRot, quaternion.LookRotation(fwd, new float3(0, 1, 0)));


                rotation.Value = newRot;
            }).Schedule();

        var ecb = sys.CreateCommandBuffer();
        // Collision: Food
        Entities
            .ForEach((Entity e, int entityInQueryIndex, ref Translation translation, ref Velocity velocity,
                in Food food) =>
            {
                // In a goal area
                if (math.abs(translation.Value.x) >= spawner.ArenaExtents.x)
                {
                    // Collided with Ground
                    if (translation.Value.y <= 0.5f)
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
                    var hasMoved = false;
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
                            if (collisionHeightOverlap > 0f &&
                                collisionHeightOverlap <= 1f)
                            {
                                translation.Value.y += collisionHeightOverlap + math.EPSILON;
                                
                                velocity.Value = float3.zero;

                                hasMoved = true;
                            }
                        }
                    }
                }
            }).WithDisposeOnCompletion(foodPositions)
            .WithDisposeOnCompletion(foodEntities)
            .Schedule();

        // Collision: Bee Bits
        Entities
            .WithAll<BeeBitsTag>()
            .ForEach((Entity e, ref Translation translation) =>
            {
                // Ground Collision
                if (translation.Value.y < 0)
                {
                    // TODO: Spawn blood
                }
            }).Schedule();

        sys.AddJobHandleForProducer(Dependency);
    }
}