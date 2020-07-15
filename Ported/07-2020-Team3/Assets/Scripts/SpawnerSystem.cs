using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in SpawnerAuthoring spawner, in LocalToWorld ltw) =>
        {
            for (int i = 0; i < spawner.CountX; i++)
            {
                for (int j = 0; j < spawner.CountY; j++)
                {
                    float posX = 2 * (i - (spawner.CountX - 1) / 2);
                    float posZ = 2 * (j - (spawner.CountY - 1) / 2);

                    Entity instance = EntityManager.Instantiate(spawner.Prefab);

                    SetComponent(instance, new Translation
                    {
                        Value = ltw.Position + new Unity.Mathematics.float3(posX, 0, posZ)
                    });
                }
            }
            EntityManager.DestroyEntity(entity);
        }).Run();
    }
}
