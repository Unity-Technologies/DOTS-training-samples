using Unity.Entities;
using Unity.Mathematics;

public struct SplineBlobAsset
{
    public BlobArray<float3> equalDistantPoints;
    public BlobArray<float> unitPointPlatformPositions;
    public float length;
    public int TrainCount;
    public int CarriagesPerTrain;
}

public struct SplineBlobAssetArray
{
    public BlobArray<SplineBlobAsset> splineBlobAssets;
}
