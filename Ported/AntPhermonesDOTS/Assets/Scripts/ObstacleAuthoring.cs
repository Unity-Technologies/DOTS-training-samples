using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Serialization;

[RequiresEntityConversion]
[ConverterVersion("kasperrasmus", 2)]
public class ObstacleAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [FormerlySerializedAs("Prefab")] public GameObject prefab;
    public int obstacleRingCount;
    [Range(0f,1f)]
    public float obstaclesPerRing;
    public float obstacleRadius;
    
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(prefab);
    }
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var builder = new BlobBuilder(Allocator.Temp);
        ref var root = ref builder.ConstructRoot<ObstacleBlobAsset>();

        root.Radius = obstacleRadius;
        root.RingCount = obstacleRingCount;
        root.ObstaclesPerRing = obstaclesPerRing;
        var positionCount = 4;
        var posArray = builder.Allocate(ref root.Positions, 4);
        for (var i = 0; i < positionCount; ++i)
        {
            // TODO move the position generation code from ObstacleSystem here.
            posArray[i] = new float3((float)i, (float)i, (float)i);
        }

        var blobReference = builder.CreateBlobAssetReference<ObstacleBlobAsset>(Allocator.Persistent);
        builder.Dispose();

        dstManager.AddComponentData(entity, new Obstacle
        {
            Prefab = conversionSystem.GetPrimaryEntity(prefab),
            Blob = blobReference
        });
    }
}
