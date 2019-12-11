using Unity.Entities;
using Unity.Mathematics;

struct ObstacleBlobAsset
{
    public float Radius;
    public int RingCount;
    public float ObstaclesPerRing;
    public BlobArray<float3> Positions;
}
struct Obstacle : IComponentData
{
    public Entity Prefab;
    public BlobAssetReference<ObstacleBlobAsset> Blob;
}