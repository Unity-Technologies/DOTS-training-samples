using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class PlatformSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var splineDataArrayRef = GetSingleton<SplineDataReference>().BlobAssetReference;

        const float platformSize = 30.0f;
        float3 returnPlatformOffset = new float3(20, 0, -8);
        
        Entities.
            ForEach((Entity entity, in PlatformSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);
                for (var lineId = 0; lineId < splineDataArrayRef.Value.splineBlobAssets.Length; lineId++)
                {
                    ref var splineBlobAsset = ref splineDataArrayRef.Value.splineBlobAssets[lineId];
                    int nbPlatforms = splineBlobAsset.unitPointPlatformPositions.Length;
                    int halfPlatforms = nbPlatforms / 2;
                    NativeArray<Rotation> outBoundsRotations = new NativeArray<Rotation>(halfPlatforms, Allocator.Temp);
                    NativeArray<float3> outBoundsTranslations = new NativeArray<float3>(halfPlatforms, Allocator.Temp);
                    for (int i = 0; i < nbPlatforms; i++)
                    {
                        var instance = ecb.Instantiate(spawner.PlatformPrefab);
                        int pointIndex = Mathf.FloorToInt(splineBlobAsset.unitPointPlatformPositions[i] * splineBlobAsset.equalDistantPoints.Length);

                        Translation translation;
                        Rotation rotation = default;
                        if (i < halfPlatforms)
                        {
                            int centerPlatformIndex = Mathf.FloorToInt((splineBlobAsset.unitPointPlatformPositions[i] - platformSize/(2 * splineBlobAsset.length))  * splineBlobAsset.equalDistantPoints.Length );
                            var centerPos = splineBlobAsset.equalDistantPoints[centerPlatformIndex];
                            var centerNextPos = splineBlobAsset.equalDistantPoints[centerPlatformIndex + 1];
                            (translation ,rotation) = GetStationTransform(splineBlobAsset.equalDistantPoints[pointIndex],
                                centerPos,
                                centerNextPos);
                            outBoundsRotations[i] = rotation;
                            outBoundsTranslations[i] = translation.Value;
                        }
                        else
                        {
                            var outBoundQuaternion = outBoundsRotations[nbPlatforms - i - 1].Value;
                            var outBoundTranslation = outBoundsTranslations[nbPlatforms - i - 1];
                            var returnTranslation = math.mul(outBoundQuaternion, returnPlatformOffset) + outBoundTranslation;
                            translation = new Translation() {Value = returnTranslation};
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
    
    static (Translation, Rotation) GetStationTransform(float3 curPos, float3 centerPos, float3 centerNextPos)
    {
        float3 backTrackDir =  - math.normalize(centerNextPos - centerPos);
        float3 forwardPlatformDir = math.cross(backTrackDir, math.up());
        Rotation rotation = new Rotation()
        {
            Value = quaternion.LookRotation(forwardPlatformDir, math.up())
        };

        Translation translation = new Translation()
        {
            Value = curPos,
        };
        return (translation, rotation);
    }
}
