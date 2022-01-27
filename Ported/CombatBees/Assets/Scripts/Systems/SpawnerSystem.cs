using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using static Unity.Mathematics.math;

// Make sure this runs after the destroyer, so it reinitializes after everything has been destroyed
[UpdateAfter(typeof(DestroyerSystem))]
[UpdateBefore(typeof(MovementSystem))]
[UpdateBefore(typeof(UpdateStateSystem))]
public partial class SpawnerSystem : SystemBase
{
    private int resetCount = 0;

    public static int3 GetBeeMinBounds()
    {
        return new int3(Spawner.ArenaExtents.x + 1, Spawner.ArenaHeight / 4, -Spawner.ArenaExtents.y + 1);
    }

    public static int3 GetBeeMaxBounds(int3 minBeeBounds)
    {
        return new int3(Spawner.ArenaExtents.x + Spawner.GoalDepth - 1, minBeeBounds.y * 3 + 1, Spawner.ArenaExtents.y);
    }

    public static float GetRandomYellowBeeX(ref Random random, int3 minBeeBounds, int3 maxBeeBounds)
    {
        return random.NextInt(minBeeBounds.x, maxBeeBounds.x + 1);
    }

    public static float GetRandomBlueBeeX(ref Random random, int3 minBeeBounds, int3 maxBeeBounds)
    {
        return random.NextInt(-maxBeeBounds.x, -minBeeBounds.x + 1);
    }

    public static float GetRandomBeeY(ref Random random, int3 minBeeBounds, int3 maxBeeBounds)
    {
        return random.NextInt(minBeeBounds.y, maxBeeBounds.y);
    }

    public static float GetRandomBeeZ(ref Random random, int3 minBeeBounds, int3 maxBeeBounds)
    {
        return random.NextInt(minBeeBounds.z, maxBeeBounds.z);
    }

    public static float3 GetRandomGoalTarget(ref Random random, TeamValue team)
    {
        var minBeeBounds = SpawnerSystem.GetBeeMinBounds();
        var maxBeeBounds = SpawnerSystem.GetBeeMaxBounds(minBeeBounds);

        var beeRandomY = SpawnerSystem.GetRandomBeeY(ref random, minBeeBounds, maxBeeBounds);
        var beeRandomZ = SpawnerSystem.GetRandomBeeZ(ref random, minBeeBounds, maxBeeBounds);

        float beeRandomX = (team == TeamValue.Yellow)
            ? GetRandomYellowBeeX(ref random, minBeeBounds, maxBeeBounds)
            : GetRandomBlueBeeX(ref random, minBeeBounds, maxBeeBounds);

        return float3(beeRandomX, beeRandomY, beeRandomZ);;
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        #region Initial_Spawns

        // Initialization/Reinitialization
        if (Input.GetKeyDown(KeyCode.R) || resetCount == 0)
        {
            resetCount++;

            var randomSeed = (uint) max(1,
                DateTime.Now.Millisecond +
                DateTime.Now.Second +
                DateTime.Now.Minute +
                DateTime.Now.Day +
                DateTime.Now.Month +
                DateTime.Now.Year);

            // Note: looks like getting a singleton reference and then querying for the same object results in an error
            // var spawnerSingleton = GetSingleton<Spawner>();
            // var foodRowsAndColumns = spawnerSingleton.ArenaExtents.y * 2 - 2;
            // var planarOffset = foodRowsAndColumns / 2;
            // var halfArenaHeight = spawnerSingleton.ArenaHeight / 2;

            Entities
                .ForEach((in Spawner spawner) =>
                {
                    var foodRowsAndColumns = Spawner.ArenaExtents.y * 2 - 2;
                    var planarOffset = foodRowsAndColumns / 2;
                    var halfArenaHeight = Spawner.ArenaHeight / 2;

                    var random = new Random(randomSeed);

                    #region Food_Init

                    var spawnedResources = 0;
                    if (spawner.StartingResources > 0)
                    {
                        for (var y = 0; y < halfArenaHeight && spawnedResources < spawner.StartingResources; y++)
                        {
                            for (var z = 0; z < foodRowsAndColumns && spawnedResources < spawner.StartingResources; z++)
                            {
                                for (var x = 0;
                                     x < foodRowsAndColumns && spawnedResources < spawner.StartingResources;
                                     x++)
                                {
                                    if (random.NextInt(halfArenaHeight) == 0)
                                    {
                                        var spawnPosition = new float3(
                                            x - planarOffset,
                                            y + halfArenaHeight,
                                            z - planarOffset);

                                        var fallTargetPosition = spawnPosition;
                                        fallTargetPosition.y = 0.5f;

                                        BufferEntityInstantiation(
                                            spawner.ResourcePrefab,
                                            spawnPosition,
                                            fallTargetPosition,
                                            ref ecb);

                                        spawnedResources++;
                                    }
                                }
                            }
                        }
                    }

                    #endregion Food_Init

                    #region Bee_Init

                    var minBeeBounds = GetBeeMinBounds();
                    var maxBeeBounds = GetBeeMaxBounds(minBeeBounds);

                    for (var i = 0; i < spawner.StartingBees * 2; i++)
                    {
                        var beeRandomY = GetRandomBeeY(ref random, minBeeBounds, maxBeeBounds);
                        var beeRandomZ = GetRandomBeeZ(ref random, minBeeBounds, maxBeeBounds);

                        if (i < spawner.StartingBees)
                        {
                            // Yellow Bees
                            var beeRandomX = GetRandomYellowBeeX(ref random, minBeeBounds, maxBeeBounds);

                            BufferEntityInstantiation(spawner.YellowBeePrefab,
                                new float3(beeRandomX, beeRandomY, beeRandomZ),
                                ref ecb);
                        }
                        else
                        {
                            // Blue Bees
                            var beeRandomX = GetRandomBlueBeeX(ref random, minBeeBounds, maxBeeBounds);

                            BufferEntityInstantiation(spawner.BlueBeePrefab,
                                new float3(beeRandomX, beeRandomY, beeRandomZ),
                                ref ecb);
                        }
                    }

                    #endregion Bee_Init
                }).Run();
        }

        #endregion Initial_Spawns

        #region Click_To_Spawn

        // Click-to-Spawn Resource
        // TODO: use GetMouseButtonDown(0) with an interval timer to spawn while holding the mouse
        if (Input.GetMouseButtonDown(0))
        {
            var camera = this.GetSingleton<GameObjectRefs>().Camera;
            var cursorRay = camera.ScreenPointToRay(Input.mousePosition);
            var groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(cursorRay, out var distanceToHit))
            {
                var hitPoint = (float3) cursorRay.GetPoint(distanceToHit);

                Entities
                    .ForEach((in Spawner spawner) =>
                    {
                        if (abs(hitPoint.x) < Spawner.ArenaExtents.x + Spawner.GoalDepth - 1 &&
                            abs(hitPoint.z) < Spawner.ArenaExtents.y - 1)
                        {
                            BufferEntityInstantiation(
                                spawner.ResourcePrefab,
                                hitPoint + new float3(0f, 0.5f, 0f),
                                ref ecb);
                        }
                    }).Run();
            }
        }

        #endregion Click_To_Spawn

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    public static void BufferEntityInstantiation(
        Entity prefabEntity,
        float3 position,
        ref EntityCommandBuffer ecb)
    {
        var instance = ecb.Instantiate(prefabEntity);

        var translation = new Translation
        {
            Value = position
        };
        ecb.SetComponent(instance, translation);

        var movement = PP_Movement.Create(position, position);

        ecb.SetComponent(instance, movement);
    }

    public static void BufferEntityInstantiation(
        Entity prefabEntity,
        float3 startPosition,
        float3 endPosition,
        ref EntityCommandBuffer ecb)
    {
        var instance = ecb.Instantiate(prefabEntity);

        var translation = new Translation
        {
            Value = startPosition
        };
        ecb.SetComponent(instance, translation);

        var movement = PP_Movement.Create(startPosition, endPosition);

        ecb.SetComponent(instance, movement);
    }
}