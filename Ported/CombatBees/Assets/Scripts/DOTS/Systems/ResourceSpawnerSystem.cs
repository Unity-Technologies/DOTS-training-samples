using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ResourceSpawnerSystem : SystemBase
{
    protected override void OnCreate()
    {
        
    }

    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
        .ForEach((Entity entity, in Spawner spawner, in LocalToWorld ltw) =>
        {
            for (int x = 0; x < spawner.Count; ++x)
            {
                var instance = EntityManager.Instantiate(spawner.Prefab);
                SetComponent(instance, new Translation
                {
                    Value = ltw.Position + new float3(math.sin(x), 0, math.cos(x)) * x * 0.1f
                });
            }

            EntityManager.DestroyEntity(entity);
        }).Run();
    }
}
