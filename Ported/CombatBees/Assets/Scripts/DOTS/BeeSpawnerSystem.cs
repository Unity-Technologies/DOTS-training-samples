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
        //var currentScore = GetSingleton<BeeManager.Instance>();

        Entities.WithStructuralChanges()
    .ForEach((Entity entity, in BeeSpawner spawner, in LocalToWorld ltw) =>
    {
        for (int i = 0; i < BeeManager.Instance.teamColors.Length; i++)
        {
            for (int x = 0; x < spawner.BeeCount; ++x)
            {
                var instance = EntityManager.Instantiate(spawner.Prefab);
                SetComponent(instance, new Translation
                {
                    Value = ltw.Position + new float3(x, i * 2, 0)
                });
                SetComponent(instance, new Team
                {
                    Value = i
                });
                SetComponent(instance, new BeeColor
                {
                    Value = new float4(BeeManager.Instance.teamColors[i].r, BeeManager.Instance.teamColors[i].g, BeeManager.Instance.teamColors[i].b, BeeManager.Instance.teamColors[i].a)
                });
            }
        }

        EntityManager.DestroyEntity(entity);
    }).Run();
    }
}
