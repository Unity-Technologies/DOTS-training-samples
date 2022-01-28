using Unity.Entities;

public struct Spline : IComponentData
{
    public BlobAssetReference<SplineDataBlob> splinePath;
}

