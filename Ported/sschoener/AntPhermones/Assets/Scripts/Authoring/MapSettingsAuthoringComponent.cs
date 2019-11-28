using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

    static bool IsVisible(float2 point1, float2 point2, ref ObstacleData obstacles)
    {
        float2 d = point2 - point1;
        int stepCount = 10 * obstacles.MapSize;
        for (int i = 0; i < stepCount; i++)
        {
            float t = (float)i / stepCount;
            if (obstacles.HasObstacle(point1 + t * d))
                return false;
        }
        return true;
    }
    
    void BuildVisibilityData(int mapSize, BlobBuilderArray<byte> cells, float2 target, ref ObstacleData obstacles)
    {
        int idx = 0;
        for (int y = 0; y < mapSize; y++)
        {
            float py = y + .5f;
            for (int x = 0; x < mapSize; x++)
            {
                float px = x + .5f;
                byte mask = (byte)(0x1 << (idx % 8));
                if (IsVisible(new float2(px, py), target, ref obstacles))
                    cells[idx / 8] |= mask;
                else
                    cells[idx / 8] &= (byte)~mask;
                idx++;
            }
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var obstacles = BuildObstacleData();
        BlobAssetReference<VisibilityData> visibility;
        using (var blobBuilder = new BlobBuilder(Allocator.Temp))
        {
            int mapSize = GameManager.mapSize;
            int numCells = 1 + mapSize * mapSize / 8;
            ref var visData = ref blobBuilder.ConstructRoot<VisibilityData>();
            BuildVisibilityData(mapSize, blobBuilder.Allocate(ref visData.Resource, numCells), GameManager.ResourcePosition, ref obstacles.Value);
            BuildVisibilityData(mapSize, blobBuilder.Allocate(ref visData.Colony, numCells), GameManager.ColonyPosition, ref obstacles.Value);
            visData.MapSize = mapSize;
            visibility = blobBuilder.CreateBlobAssetReference<VisibilityData>(Allocator.Persistent);
        }
        
        dstManager.AddComponentData(entity, new MapSettingsComponent
        {
            MapSize = GameManager.mapSize,
            ColonyPosition = GameManager.ColonyPosition,
            ResourcePosition = GameManager.ResourcePosition,
            TrailDecay = TrailDecay,
            TrailAdd = TrailAdd,
            ObstacleRadius = GameManager.obstacleRadius,
            Obstacles = obstacles,
            Visibility = visibility
        });
    }
}
