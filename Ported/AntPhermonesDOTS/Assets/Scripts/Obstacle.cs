using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

struct ObstacleBlobAsset
{
    public float Radius;
    public BlobArray<float3> Positions;
    public BlobArray<BitField64> TileOccupancy;
}
struct Obstacle : IComponentData
{
    public Entity Prefab;
    public BlobAssetReference<ObstacleBlobAsset> Blob;
}

public static class ObstacleHelper
{
    const int BitFieldSize = 64;
    
    public static int2 WorldPosToTilePos(float2 position, int mapSize, int tileSize) {
        var discretePosition = new int2((position.xy + mapSize * 0.5f) / new float2(tileSize, tileSize));
        return discretePosition;
    }
    
    public static int TileIndex(int2 tilePos, int tileCount)
    {
        return tilePos.x * tileCount + tilePos.y;
    }

    public static int BitFieldIndex(int tileIndex)
    {
        return tileIndex / BitFieldSize;
    }
    
    public static int BitIndex(int tileIndex)
    {
        return tileIndex % BitFieldSize;
    }
}
