using Unity.Entities;

public struct PathDataRef : IComponentData
{
    public BlobAssetReference<PathData> Data;
}