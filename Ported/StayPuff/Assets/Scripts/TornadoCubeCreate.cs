using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TornadoCubeCreate : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in TornadoCubeData cube, in LocalToWorld ltw) =>
            {
                for (int x = 0; x < cube.CountX; ++x)
                    for (int z = 0; z < cube.CountZ; ++z)
                    {
                        var posX = UnityEngine.Random.Range(cube.BoundsMin.x, cube.BoundsMax.x);
                        var posY = UnityEngine.Random.Range(cube.BoundsMin.y, cube.BoundsMax.y);
                        var posZ = UnityEngine.Random.Range(cube.BoundsMin.z, cube.BoundsMax.z);
                        var instance = EntityManager.Instantiate(cube.Prefab);
                        SetComponent(instance, new Translation
                        {
                            Value = ltw.Position + new float3(posX, posY, posZ)
                        });
                    }

                EntityManager.DestroyEntity(entity);
            }).Run();
    }
}
