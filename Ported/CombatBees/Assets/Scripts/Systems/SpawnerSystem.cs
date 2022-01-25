using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;
using Random = Unity.Mathematics.Random;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var goalDepth = 10;
        var arenaExtents = new int2(40, 15);
        var halfArenaHeight = 10;
        var startingBeeCountPerTeam = 10;

        var inputReinitialize = Input.GetKeyDown(KeyCode.R);
        var randomSeed = (uint) (System.DateTime.Now.Millisecond + 1);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                if (true)
                {
                    var random = new Random(randomSeed);

                    for (var i = 0; i < arenaExtents.y * arenaExtents.y; i++)
                    {
                        var randomX = random.NextInt(-arenaExtents.y, arenaExtents.y);
                        var randomZ = random.NextInt(-arenaExtents.y, arenaExtents.y);

                        BufferEntityInstantiation(spawner.ResourcePrefab, new float3(randomX, 0.5f, randomZ), ref ecb);
                    }

                    for (var i = 0; i < startingBeeCountPerTeam * 2; i++)
                    {
                        if (i < startingBeeCountPerTeam) // Yellow Bees
                        {
                            var randomX = random.NextInt(arenaExtents.x + 1, arenaExtents.x + goalDepth - 1);
                            var randomY = random.NextInt(halfArenaHeight + halfArenaHeight);
                            var randomZ = random.NextInt(-arenaExtents.y + 1, arenaExtents.y - 1);
                    
                            BufferEntityInstantiation(spawner.YellowBeePrefab, new float3(randomX, randomY, randomZ),
                                ref ecb);
                        }
                        else // Blue Bees
                        {
                            var randomX = random.NextInt(-arenaExtents.x + 1, -arenaExtents.x + goalDepth - 1);
                            var randomY = random.NextInt(halfArenaHeight + halfArenaHeight);
                            var randomZ = random.NextInt(-arenaExtents.y + 1, arenaExtents.y - 1);
                    
                            BufferEntityInstantiation(spawner.BlueBeePrefab, new float3(randomX, randomY, randomZ),
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
        // Debug.Log($"{position.x} : {position.z}");
        var translation = new Translation {Value = position};
        ecb.SetComponent(instance, translation);
    }
}