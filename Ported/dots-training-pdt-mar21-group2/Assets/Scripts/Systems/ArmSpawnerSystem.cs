using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ArmSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in ArmSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.ArmCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.ArmPrefab);
                    var translation = new Translation {Value = new float3(i, 0, 0)};
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
