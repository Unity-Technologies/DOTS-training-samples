using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(DestroyerSystem))]
public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var goalDepth = 10;
        var arenaExtents = new int2(40, 15);
        var arenaHeight = 20;
        var startingBeeCountPerTeam = 10;

        var inputReinitialize = Input.GetKeyDown(KeyCode.R);
        var randomSeed = (uint) min(1,
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

                    for (var i = 0; i < arenaExtents.y * arenaExtents.y; i++)
                    {
                        var resourceRandomX = random.NextInt(-arenaExtents.y, arenaExtents.y);
                        var resourceRandomZ = random.NextInt(-arenaExtents.y, arenaExtents.y);

                        BufferEntityInstantiation(spawner.ResourcePrefab,
                            new float3(resourceRandomX, 0.5f, resourceRandomZ), ref ecb);
                    }

                    var minBeeBounds = new int3(arenaExtents.x + 1, arenaHeight / 4, -arenaExtents.y + 1);
                    var maxBeeBounds = new int3(arenaExtents.x + goalDepth - 1, minBeeBounds.y * 3, arenaExtents.y - 1);

                    for (var i = 0; i < startingBeeCountPerTeam * 2; i++)
                    {
                        var beeRandomY = random.NextInt(minBeeBounds.y, maxBeeBounds.y);
                        var beeRandomZ = random.NextInt(minBeeBounds.z, maxBeeBounds.z);

                        if (i < startingBeeCountPerTeam)
                        {
                            // Yellow Bees
                            var beeRandomX = random.NextInt(minBeeBounds.x, maxBeeBounds.x);

                            BufferEntityInstantiation(spawner.YellowBeePrefab,
                                new float3(beeRandomX, beeRandomY, beeRandomZ),
                                ref ecb);
                        }
                        else
                        {
                            // Blue Bees
                            var beeRandomX = random.NextInt(-maxBeeBounds.x, -minBeeBounds.x);

                            BufferEntityInstantiation(spawner.BlueBeePrefab,
                                new float3(beeRandomX, beeRandomY, beeRandomZ),
                                ref ecb);
                        }
                    }
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