using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Core;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;


public class AntSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var random = new Unity.Mathematics.Random(1234);

        Entities
            .WithAll<AntSpawner>()
            .ForEach((Entity entity, in AntSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.AntCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.AntPrefab);
                    var translation = new Translation { Value = new float3(-5.0f+random.NextFloat()*10.0f, 0, 0) };
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
