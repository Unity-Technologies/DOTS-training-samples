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
                for (int lineId = 0; lineId < splineDataArrayRef.Value.splineBlobAssets.Length; lineId++)
                {
                    ref var splineData = ref splineDataArrayRef.Value.splineBlobAssets[lineId];
                    for (int i = 0; i < splineData.points.Length; i++)
                    {
                        var instance = ecb.Instantiate(spawner.RailPrefab);
                        var translation = new Translation {Value = splineData.points[i]};
                        if (i < splineData.points.Length - 1)
                            rotation = GetRailRotation(splineData.points[i], splineData.points[i + 1]);
                        ecb.SetComponent(instance, rotation);
                        ecb.SetComponent(instance, translation);
                    }
                }
            }
        ).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    static Rotation GetRailRotation(float3 curPos, float3 nextPos)
    {
        Vector3 forwardDir = Vector3.Normalize(nextPos - curPos);
        
        Rotation rotation = new Rotation()
        {
            Value = Quaternion.LookRotation(forwardDir, Vector3.up)
        };
        return rotation;
    }
}
