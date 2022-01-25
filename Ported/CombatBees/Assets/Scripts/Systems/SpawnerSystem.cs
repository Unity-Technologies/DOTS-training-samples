using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(DestroyerSystem))]
[UpdateBefore(typeof(PP_Movement))]
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
        var inputReinitialize = Input.GetKeyDown(KeyCode.R);
        var randomSeed = (uint) max(1,
            DateTime.Now.Millisecond +
            DateTime.Now.Second +
            DateTime.Now.Minute +
            DateTime.Now.Day +
            DateTime.Now.Month +
            DateTime.Now.Year);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                if (inputReinitialize)
                {
                    var random = new Random(randomSeed);

                    #region Food_Init

                    var foodRowsAndColumns = spawner.ArenaExtents.y * 2 - 2;

                    for (var i = 0; i < foodRowsAndColumns * foodRowsAndColumns; i++)
                    {
                        var resourceRandomX = (float) random.NextInt(foodRowsAndColumns + 1);
                        resourceRandomX -= spawner.ArenaExtents.y - 1;
                        var resourceRandomZ = (float) random.NextInt(foodRowsAndColumns + 1);
                        resourceRandomZ -= spawner.ArenaExtents.y - 1;

                        BufferEntityInstantiation(spawner.ResourcePrefab,
                            new float3(resourceRandomX, 0.5f, resourceRandomZ), ref ecb);
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
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    // Learning Note: this must be static to be accessed from a Burst-compiled system
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

        var movement = new PP_Movement
        {
            startLocation = position,
            endLocation = position
        };
        ecb.SetComponent(instance, movement);
    }
}