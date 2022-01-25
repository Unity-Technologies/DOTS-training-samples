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
    private static readonly int goalDepth = 10;
    private static readonly int2 arenaExtents = new int2(40, 15);
    private static readonly int arenaHeight = 20;

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

                    var foodRowsAndColumns = arenaExtents.y * 2 - 2;

                    for (var i = 0; i < foodRowsAndColumns * foodRowsAndColumns; i++)
                    {
                        var resourceRandomX = (float) random.NextInt(foodRowsAndColumns + 1);
                        resourceRandomX -= arenaExtents.y - 1;
                        var resourceRandomZ = (float) random.NextInt(foodRowsAndColumns + 1);
                        resourceRandomZ -= arenaExtents.y - 1;

                        BufferEntityInstantiation(spawner.ResourcePrefab,
                            new float3(resourceRandomX, 0.5f, resourceRandomZ), ref ecb);
                    }

                    #endregion Food_Init

                    #region Bee_Init

                    var minBeeBounds = new int3(arenaExtents.x + 1, arenaHeight / 4, -arenaExtents.y + 1);
                    var maxBeeBounds = new int3(arenaExtents.x + goalDepth - 1, minBeeBounds.y * 3 + 1, arenaExtents.y);

                    for (var i = 0; i < spawner.StartingBees * 2; i++)
                    {
                        var beeRandomY = random.NextInt(minBeeBounds.y, maxBeeBounds.y);
                        var beeRandomZ = random.NextInt(minBeeBounds.z, maxBeeBounds.z);

                        if (i < spawner.StartingBees)
                        {
                            // Yellow Bees
                            var beeRandomX = random.NextInt(minBeeBounds.x, maxBeeBounds.x + 1);

                            BufferEntityInstantiation(spawner.YellowBeePrefab,
                                new float3(beeRandomX, beeRandomY, beeRandomZ),
                                ref ecb);
                        }
                        else
                        {
                            // Blue Bees
                            var beeRandomX = random.NextInt(-maxBeeBounds.x, -minBeeBounds.x + 1);

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