using Unity.Entities;

public struct Spline : IComponentData
{
    public BlobAssetReference<SplineHandle> Value;
}

public struct SplineHandle
{
    public BlobArray<Entity> Segments;
}
