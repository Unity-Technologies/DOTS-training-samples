using Unity.Entities;
using Unity.Mathematics;

public struct ObstacleBucketIndices : IBufferElementData
{
    public static implicit operator Entity(ObstacleBucketIndices e) { return e.Value; }
    public static implicit operator ObstacleBucketIndices(Entity e) { return new ObstacleBucketIndices { Value = e }; }
    
    public Entity Value;

    public int GetObstacleBucketIndex(float2 pos, int mapSize, int bucketResolution)
    {
        return GetObstacleBucketIndex(pos.x, pos.y, mapSize, bucketResolution);
    }

    public int GetObstacleBucketIndex(float3 pos, int mapSize, int bucketResolution)
    {
        return GetObstacleBucketIndex(pos.x, pos.y, mapSize, bucketResolution);
    }

    public int GetObstacleBucketIndex(float posX, float posY, int mapSize, int bucketResolution)
    {
        int x = (int)(posX / mapSize * bucketResolution);
        int y = (int)(posY / mapSize * bucketResolution);
        if (x < 0 || y < 0 || x >= bucketResolution || y >= bucketResolution)
        {
            return -1;
        }
        else
        {
            return x * bucketResolution + y;
        }
    }
}
