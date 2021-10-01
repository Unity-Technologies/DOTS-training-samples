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
        
        Entities.
            ForEach((Entity entity, in RailsSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                Rotation rotation = default;
                for (var lineId = 0; lineId < splineDataArrayRef.Value.splineBlobAssets.Length; lineId++)
                {
                    ref var splineBlobAsset = ref splineDataArrayRef.Value.splineBlobAssets[lineId];
                    for (var i = 0; i < splineBlobAsset.equalDistantPoints.Length; i++)
                    {
                        var instance = ecb.Instantiate(spawner.RailPrefab);
                        if (i < splineBlobAsset.equalDistantPoints.Length - 1)
                            rotation = new Rotation
                            {
                                Value = quaternion.LookRotation(
                                    splineBlobAsset.equalDistantPoints[i + 1] - splineBlobAsset.equalDistantPoints[i], 
                                    Vector3.up)
                            };
                        ecb.SetName(instance, $"Rail {lineId}-{i}");
                        ecb.SetComponent(instance, rotation);
                        ecb.SetComponent(instance, new Translation {Value = splineBlobAsset.equalDistantPoints[i]});
                    }
                }
            }
        ).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
