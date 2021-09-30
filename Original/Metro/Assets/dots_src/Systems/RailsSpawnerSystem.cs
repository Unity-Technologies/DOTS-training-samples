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

        const float platformSize = 30.0f;
        const float platformWidth = 10.0f;
        float3 returnPlatformOffset = new float3(20, 0, -8);
        
        Entities.
            ForEach((Entity entity, in RailsSpawner spawner) =>
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
                                Value = quaternion.LookRotation(
                                    splineData.points[i + 1] - splineData.points[i], 
                                    Vector3.up)
                            };
                        ecb.SetComponent(instance, rotation);
                        ecb.SetComponent(instance, new Translation {Value = splineData.points[i]});
                    }

                    int nbPlatforms = splineData.platformPositions.Length;
                    int halfPlatforms = nbPlatforms / 2;
                    NativeArray<Rotation> outBoundsRotations = new NativeArray<Rotation>(halfPlatforms, Allocator.Temp);
                    NativeArray<float3> outBoundsTranslations = new NativeArray<float3>(halfPlatforms, Allocator.Temp);
                    for (int i = 0; i < nbPlatforms; i++)
                    {
                        var instance = ecb.Instantiate(spawner.PlatformPrefab);
                        int pointIndex = Mathf.FloorToInt(splineData.platformPositions[i] * splineData.points.Length);
                        int centerPlatformIndex = Mathf.FloorToInt((splineData.platformPositions[i] - platformSize/(2 * splineData.length))  * splineData.points.Length );
                        var centerPos = splineData.points[centerPlatformIndex];
                        var centerNextPos = splineData.points[centerPlatformIndex + 1];
                        Translation translation;
                        if (i < halfPlatforms)
                        {
                            (translation ,rotation) = GetStationTransform(splineData.points[pointIndex],
                                splineData.points[centerPlatformIndex],
                                splineData.points[centerPlatformIndex + 1]);
                            outBoundsRotations[i] = rotation;
                            outBoundsTranslations[i] = translation.Value;
                        }
                        else
                        {
                            var outBoundQuaternion = outBoundsRotations[nbPlatforms - i - 1].Value;
                            var outBoundTranslation = outBoundsTranslations[nbPlatforms - i - 1];
                            var returnTranslation = math.mul(outBoundQuaternion, returnPlatformOffset) + outBoundTranslation;
                            //var returnTranslation = outBoundTranslation;
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

    static Rotation GetRailRotation(float3 curPos, float3 nextPos)
    {
        Vector3 forwardDir = Vector3.Normalize(nextPos - curPos);
        Rotation rotation = new Rotation()
        {
            Value = Quaternion.LookRotation(forwardDir, Vector3.up)
        };
        return rotation;
    }

    static (Translation, Rotation) GetStationTransform(float3 curPos, float3 centerPos, float3 centerNextPos)
    {
        Vector3 backTrackDir =  - Vector3.Normalize(centerNextPos - centerPos);
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
