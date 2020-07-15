using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BeeSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
    .ForEach((Entity entity, in BeeSpawner spawner, in LocalToWorld ltw) =>
    {
        for (int x = 0; x < spawner.BeeCount; ++x)
        {
            //var posX = 2 * (x - (spawner.CountX - 1) / 2);
            //var posZ = 2 * (z - (spawner.CountZ - 1) / 2);
            var instance = EntityManager.Instantiate(spawner.Prefab);
            SetComponent(instance, new Translation
            {
                Value = ltw.Position + new float3(x, 0, 0)
            });
        }

        EntityManager.DestroyEntity(entity);
    }).Run();
    }
}
