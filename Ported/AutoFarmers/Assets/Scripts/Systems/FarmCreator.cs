using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in TileSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.GridSize.x; ++i)
                {
                    for (int j = 0; j < spawner.GridSize.y; ++j)
                    {
                        var instance = ecb.Instantiate(spawner.TilePrefab);
                        var translation = new Translation { Value = new float3(i, 0, j) };
                        ecb.SetComponent(instance, translation);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
