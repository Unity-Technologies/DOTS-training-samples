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

                    int nbPlatforms = splineData.platformPositions.Length;
                    int halfPlatforms = nbPlatforms / 2;
                    NativeArray<Rotation> outBoundsRotations = new NativeArray<Rotation>(halfPlatforms, Allocator.Temp);
                    for (int i = 0; i < nbPlatforms; i++)
                    {
                        var instance = ecb.Instantiate(spawner.PlatformPrefab);
                        int pointIndex = Mathf.FloorToInt(splineData.platformPositions[i] * splineData.points.Length);
                        Translation translation;
                        if (i < halfPlatforms)
                        {
                            (translation ,rotation) = GetStationTransform(splineData.points[pointIndex],
                                splineData.points[pointIndex + 1]);
                            outBoundsRotations[i] = rotation;
                        }
                        else
                        {
                            translation = new Translation {Value = splineData.points[pointIndex]};
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

    static Rotation GetRailRotation(float3 curPos, float3 nextPos)
    {
        Vector3 forwardDir = Vector3.Normalize(nextPos - curPos);
        Rotation rotation = new Rotation()
        {
            Value = Quaternion.LookRotation(forwardDir, Vector3.up)
        };
        return rotation;
    }

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
