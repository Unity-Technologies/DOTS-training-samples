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
        var obstacleMap = GameManager.ObstacleBuckets;
        using (BlobBuilder b = new BlobBuilder(Allocator.Temp))
        {
            ref var obs = ref b.ConstructRoot<ObstacleData>();
            obs.BucketResolution = GameManager.bucketResolution;
            obs.MapSize = GameManager.mapSize;

            int h = obstacleMap.GetLength(0);
            int w = obstacleMap.GetLength(1);
            int totalSize = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                    totalSize += obstacleMap[y, x].Length;
            }

            {
                var obstacles = b.Allocate(ref obs.Obstacles, totalSize);
                var map = b.Allocate(ref obs.ObstacleBucketIndices, w * h);
                int offset = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        var bucket = obstacleMap[x, y];
                        int n = bucket.Length;
                        map[y * w + x] = offset;
                        for (int i = 0; i < n; i++)
                        {
                            obstacles[offset] = bucket[i];
                            offset++;
                        }
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
