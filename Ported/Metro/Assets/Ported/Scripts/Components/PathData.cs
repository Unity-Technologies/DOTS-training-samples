using Unity.Entities;
using Unity.Mathematics;

public struct PathData : IComponentData
{
    public BlobArray<float3> Positions;
    public BlobArray<int> MarkerType;
    public BlobArray<float3> HandlesIn;
    public BlobArray<float3> HandlesOut;
    public BlobArray<float> Distances;
    public float TotalDistance;
}

public struct PathRef : IComponentData
{
    public BlobAssetReference<PathData> Data;
}

public struct PathBufferEntity : IBufferElementData
{
    public Entity Value;

    public static implicit operator PathBufferEntity(Entity value) => new PathBufferEntity {Value = value};
    public static implicit operator Entity(PathBufferEntity element) => element.Value;
}