using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public class BeeSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in BeeSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.numBeesToSpawn; ++i)
                {
                    float xPos = (spawner.teamNumber * 100f) - 150f;

                    var instance = ecb.Instantiate(spawner.beePrefab);
                    var translation = new Translation { Value = new float3(xPos, i*2, 0) };
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
