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
                        ecb.SetComponent(instance, rotation);
                        ecb.SetComponent(instance, new Translation {Value = splineBlobAsset.equalDistantPoints[i]});
                    }

                    int nbPlatforms = splineBlobAsset.unitPointPlatformPositions.Length;
                    int halfPlatforms = nbPlatforms / 2;
                    NativeArray<Rotation> outBoundsRotations = new NativeArray<Rotation>(halfPlatforms, Allocator.Temp);
                    for (int i = 0; i < nbPlatforms; i++)
                    {
                        var instance = ecb.Instantiate(spawner.PlatformPrefab);
                        int pointIndex = Mathf.FloorToInt(splineBlobAsset.unitPointPlatformPositions[i] * splineBlobAsset.equalDistantPoints.Length);
                        Translation translation;
                        if (i < halfPlatforms)
                        {
                            (translation ,rotation) = GetStationTransform(splineBlobAsset.equalDistantPoints[pointIndex],
                                splineBlobAsset.equalDistantPoints[pointIndex + 1]);
                            outBoundsRotations[i] = rotation;
                        }
                        else
                        {
                            translation = new Translation {Value = splineBlobAsset.equalDistantPoints[pointIndex]};
                            var outBoundQuaternion = outBoundsRotations[nbPlatforms - i - 1].Value;
                            rotation = new Rotation() {Value = math.mul(quaternion.RotateY(math.PI), outBoundQuaternion)};
                        }

                        ecb.SetComponent(instance, rotation);
                        ecb.SetComponent(instance, translation);
                    }
                }
            }
        ).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
    
    // @Todo: This is isolated, so try experimenting with making it use Unity.Mathematics
    static (Translation, Rotation) GetStationTransform(float3 curPos, float3 nextPos)
    {
        Vector3 backTrackDir =  - Vector3.Normalize(nextPos - curPos);
        Vector3 forwardPlatformDir = Vector3.Cross(backTrackDir, Vector3.up);
        Rotation rotation = new Rotation()
        {
            Value = Quaternion.LookRotation(forwardPlatformDir, Vector3.up)
        };

        Translation translation = new Translation()
        {
            Value = curPos,
        };
        return (translation, rotation);
    }
}
