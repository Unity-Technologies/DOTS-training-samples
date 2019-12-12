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