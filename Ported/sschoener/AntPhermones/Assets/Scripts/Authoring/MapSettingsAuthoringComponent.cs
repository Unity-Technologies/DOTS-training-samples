using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class MapSettingsAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameManager GameManager;

    [Range(0, 1)]
    public float TrailDecay;
    [Range(0, 1)]
    public float TrailAdd;

    BlobAssetReference<ObstacleData> BuildObstacleData()
    {
        var obstacleList = GameManager.Obstacles;
        var obstacleMap = GameManager.ObstacleBuckets;
        using (BlobBuilder b = new BlobBuilder(Allocator.Temp))
        {
            ref var obs = ref b.ConstructRoot<ObstacleData>();
            obs.BucketResolution = GameManager.bucketResolution;
            obs.MapSize = GameManager.mapSize;
            var list = b.Allocate(ref obs.Obstacles, obs.MapSize * obs.MapSize);
            for (int i = 0; i < obstacleList.Length; i++)
                list[i] = obstacleList[i];
            
            var map = b.Allocate(ref obs.ObstacleMap, obs.MapSize * obs.MapSize);
            {
                int offset = 0;
                for (int y = 0; y < obs.MapSize; y++)
                {
                    for (int x = 0; x < obs.MapSize; x++)
                    {
                        map[offset] = obstacleMap[y, x].Length > 0;
                        offset++;
                    }
                }
            }
            return b.CreateBlobAssetReference<ObstacleData>(Allocator.Persistent);
        }
    } 
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MapSettingsComponent
        {
            MapSize = GameManager.mapSize,
            ColonyPosition = GameManager.ColonyPosition,
            ResourcePosition = GameManager.ResourcePosition,
            TrailDecay = TrailDecay,
            TrailAdd = TrailAdd,
            ObstacleRadius = GameManager.obstacleRadius,
            Obstacles = BuildObstacleData()
        });
    }
}
