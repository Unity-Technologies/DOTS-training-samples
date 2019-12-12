using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Serialization;
using Random = Unity.Mathematics.Random;

[RequiresEntityConversion]
[ConverterVersion("kasperrasmus", 3)]
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
        var mapSize = 128; // TODO: Fetch proper value.
        
        var builder = new BlobBuilder(Allocator.Temp);
        ref var root = ref builder.ConstructRoot<ObstacleBlobAsset>();

        root.Radius = obstacleRadius;
        root.RingCount = obstacleRingCount;
        root.ObstaclesPerRing = obstaclesPerRing;
        
        var RNG = new Random(1);
        
        var worldOffset = new float3(1, 1, 0) * ((float)mapSize / -2.0f);
        var positionList = new List<float3>();
        for (var i = 1; i < obstacleRingCount; i++)
        {
            float ringRadius = (i / (obstacleRingCount+1f)) * (mapSize * .5f);
            float circumference = ringRadius * 2f * math.PI;
            int maxCount = (int)math.ceil(circumference / (2f * obstacleRadius) * 2f);
            int offset = RNG.NextInt(0,maxCount);
            int holeCount = RNG.NextInt(1,3);
            for (int j = 0; j < maxCount; j++)
            {
                float t = (float)j / maxCount;
                if ((t * holeCount)%1f < obstaclesPerRing)
                {
                    float angle = (j + offset) / (float)maxCount * (2f * math.PI);
                    positionList.Add(worldOffset + new float3(mapSize * .5f + math.cos(angle) * ringRadius,
                                     mapSize * .5f + math.sin(angle) * ringRadius, 0f));
                }
            }
        }
        
        var blobPosArray = builder.Allocate(ref root.Positions, positionList.Count);
        for (var i = 0; i < positionList.Count; ++i)
        {
            blobPosArray[i] = positionList[i];
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
