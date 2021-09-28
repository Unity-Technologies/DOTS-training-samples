using Unity.Entities;
using Unity.Mathematics;

public struct SplineBlobAsset
{
    public BlobArray<float3> points;
    public BlobArray<float> platformPositions;
    public float length;
}

public struct SplineBlobAssetArray
{
    public BlobArray<SplineBlobAsset> splineBlobAssets;
}
