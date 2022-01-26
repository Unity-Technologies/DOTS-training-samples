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
    public static int3 GetBeeMinBounds(Spawner spawner)
    {
        return new int3(spawner.ArenaExtents.x + 1, spawner.ArenaHeight / 4, -spawner.ArenaExtents.y + 1);
    }

    public static int3 GetBeeMaxBounds(Spawner spawner, int3 minBeeBounds)
    {
        return new int3(spawner.ArenaExtents.x + spawner.GoalDepth - 1, minBeeBounds.y * 3 + 1, spawner.ArenaExtents.y);
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

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        #region Initial_Spawns

        // Initialization/Reinitialization
        if (Input.GetKeyDown(KeyCode.R))
        {
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
                    var foodRowsAndColumns = spawner.ArenaExtents.y * 2 - 2;
                    var planarOffset = foodRowsAndColumns / 2;
                    var halfArenaHeight = spawner.ArenaHeight / 2;
                    
                    var random = new Random(randomSeed);

                    #region Food_Init

                    var spawnedResources = 0;
                    if (spawner.StartingResources > 0)
                    {
                        for (var y = 0; y < halfArenaHeight && spawnedResources < spawner.StartingResources; y++)
                        {
                            for (var z = 0; z < foodRowsAndColumns && spawnedResources < spawner.StartingResources; z++)
                            {
                                for (var x = 0; x < foodRowsAndColumns && spawnedResources < spawner.StartingResources; x++)
                                {
                                    if (random.NextInt(halfArenaHeight) == 0)
                                    {
                                        var spawnPosition = new float3(
                                            x - planarOffset,
                                            y + halfArenaHeight,
                                            z - planarOffset);

                                        BufferEntityInstantiation(spawner.ResourcePrefab, spawnPosition, ref ecb);

                                        spawnedResources++;
                                    }
                                }
                            }
                        }
                    }

                    #endregion Food_Init

                    #region Bee_Init

                    var minBeeBounds = GetBeeMinBounds(spawner);
                    var maxBeeBounds = GetBeeMaxBounds(spawner, minBeeBounds);
                    
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
                        if (abs(hitPoint.x) < spawner.ArenaExtents.x + spawner.GoalDepth - 1 &&
                            abs(hitPoint.z) < spawner.ArenaExtents.y - 1)
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

    private static void BufferEntityInstantiation(
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
}