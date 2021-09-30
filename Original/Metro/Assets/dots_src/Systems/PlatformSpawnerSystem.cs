using System.Collections;
using System.Collections.Generic;
using dots_src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial class PlatformSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var splineDataArrayRef = GetSingleton<SplineDataReference>().BlobAssetReference;

        const float platformSize = 30.0f;
        float3 returnPlatformOffset = new float3(20, 0, -8);
        var random = new Random(11);
        
        Entities.
            ForEach((Entity entity, in PlatformSpawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                var marker = ecb.CreateEntity();
                ecb.AddComponent<LineEntityHolder>(marker);
                var lines = ecb.AddBuffer<EntityBufferElement>(marker);
                for (var lineId = 0; lineId < splineDataArrayRef.Value.splineBlobAssets.Length; lineId++)
                {
                    //Create the line entities
                    var lineInstance = ecb.Instantiate(spawner.LinePrefab);
                    var entityBuffer = ecb.SetBuffer<EntityBufferElement>(lineInstance);
                    lines.Add(lineInstance);
                    
                    ref var splineBlobAsset = ref splineDataArrayRef.Value.splineBlobAssets[lineId];
                    int nbPlatforms = splineBlobAsset.unitPointPlatformPositions.Length;
                    int halfPlatforms = nbPlatforms / 2;
                    NativeArray<Rotation> outBoundsRotations = new NativeArray<Rotation>(halfPlatforms, Allocator.Temp);
                    NativeArray<float3> outBoundsTranslations = new NativeArray<float3>(halfPlatforms, Allocator.Temp);

                    var lineColor = 0.5f * (random.NextFloat4() + new float4(0.5f,0.5f,0.5f,0.5f));
                    for (int i = 0; i < nbPlatforms; i++)
                    {
                        var platformInstance = ecb.Instantiate(spawner.PlatformPrefab);
                        Translation translation;
                        Rotation rotation = default;
                        if (i < halfPlatforms)
                        {
                            int centerPlatformIndex = (int)math.floor(splineBlobAsset.unitPointPlatformPositions[i] - splineBlobAsset.DistanceToPointUnitDistance(platformSize/2) );
                            var centerPos = splineBlobAsset.equalDistantPoints[centerPlatformIndex];
                            var centerNextPos = splineBlobAsset.equalDistantPoints[centerPlatformIndex + 1];
                            var curPos = splineBlobAsset.PointUnitPosToWorldPos(splineBlobAsset.unitPointPlatformPositions[i]).Item1;
                            (translation ,rotation) = GetStationTransform(curPos,
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

                        ecb.SetComponent(platformInstance, rotation);
                        ecb.SetComponent(platformInstance, translation);
                        ecb.AddComponent(platformInstance,
                           new URPMaterialPropertyBaseColor {Value = lineColor});
     
                        entityBuffer.Add(platformInstance);
                    }

                    outBoundsRotations.Dispose();
                    outBoundsTranslations.Dispose();
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

public struct LineEntityHolder : IComponentData { }
