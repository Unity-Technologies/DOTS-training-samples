using Unity.Entities;
using Unity.Mathematics;

public struct Cluster : IComponentData
{
    public float3 Position;
    public int NumberOfSubClusters;
    // Remove these probably:
    public int MinTowerHeight;
    public int MaxTowerHeight;
}

public struct GenerateCluster : IComponentData
{
}

public struct Joint : IBufferElementData
{
    public float3 Value;
    public bool IsAnchored;
}

public struct Connection : IBufferElementData
{
    public int J1, J2;
    public float OriginalLength;
}