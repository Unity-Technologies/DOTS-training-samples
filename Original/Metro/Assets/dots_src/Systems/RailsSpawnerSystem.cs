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

                for (int lineId = 0; lineId < splineDataArrayRef.Value.splineBlobAssets.Length; lineId++)
                {
                    ref var splineData = ref splineDataArrayRef.Value.splineBlobAssets[0];
                    for (int i = 0; i < splineData.points.Length; i++)
                    {
                        var instance = ecb.Instantiate(spawner.RailPrefab);
                        var translation = new Translation {Value = splineData.points[i]};
                        ecb.SetComponent(instance, translation);
                    }
                }
            }
        ).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
