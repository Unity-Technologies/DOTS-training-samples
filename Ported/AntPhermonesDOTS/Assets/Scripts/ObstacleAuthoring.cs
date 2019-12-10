using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

struct ObstacleBlobAsset
{
    public float radius;
    public BlobArray<float3> positions;
}

struct ObstacleSet : IComponentData
{
    public BlobAssetReference<ObstacleBlobAsset> blob;
}

[RequiresEntityConversion]
[ConverterVersion("kasperrasmus", 1)]
public class ObstacleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int obstacleRingCount;
    [Range(0f,1f)]
    public float obstaclesPerRing;
    public float obstacleRadius;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var builder = new BlobBuilder(Allocator.Temp);
        ref var root = ref builder.ConstructRoot<ObstacleBlobAsset>();

        root.radius = obstacleRadius;

        var positionCount = 4;
        var posArray = builder.Allocate(ref root.positions, 4);
        for (var i = 0; i < positionCount; ++i)
        {
            posArray[i] = new float3((float)i, (float)i, (float)i);
        }

        var blobReference = builder.CreateBlobAssetReference<ObstacleBlobAsset>(Allocator.Persistent);
        builder.Dispose();

        dstManager.AddComponentData(entity, new ObstacleSet
        {
            blob = blobReference
        });
    }
}
