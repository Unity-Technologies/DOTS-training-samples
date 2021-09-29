using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class RailsSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var splineDataArrayRef = GetSingleton<SplineDataReference>().BlobAssetReference;

        Entities.ForEach((Entity entity, RailsSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                Rotation rotation = default;
                for (var lineId = 0; lineId < splineDataArrayRef.Value.splineBlobAssets.Length; lineId++)
                {
                    ref var splineData = ref splineDataArrayRef.Value.splineBlobAssets[lineId];
                    for (var i = 0; i < splineData.points.Length; i++)
                    {
                        var instance = ecb.Instantiate(spawner.RailPrefab);
                        if (i < splineData.points.Length - 1)
                            rotation = new Rotation
                            {
                                Value = Quaternion.LookRotation(
                                    splineData.points[i + 1] - splineData.points[i], 
                                    Vector3.up)
                            };
                        ecb.SetComponent(instance, rotation);
                        ecb.SetComponent(instance, new Translation {Value = splineData.points[i]});
                    }
                }
            }
        ).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
