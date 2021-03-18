using System;
using Unity.Entities;
using Unity.Mathematics;

public struct MetroBlobContainer : IComponentData
{
    public BlobAssetReference<MetroBlob> Blob;
}

public struct MetroBlob
{
    public BlobArray<MetroLineBlob> Lines;
    public BlobArray<PlatformBlob> Platforms;
}

public struct MetroLineBlob
{
    public BlobArray<LinePoint> Path;
    public int FirstPlatform;
    public int PlatformCount;
    public float Distance;
}

public struct LinePoint
{
    public int index;
    public float3 location, handle_in, handle_out;
    public float distanceAlongPath;
}

public struct PlatformBlob
{
    public int ID;
    public int PlatformStartIndex;
    public int PlatformEndIndex;
    public int platformIndex;
    public float3 queuePoint;
    public float3 position;
    public quaternion rotation;
    public WalkwayBlob walkway;
    public int oppositePlatformIndex;
}

public struct WalkwayBlob
{
    public float3 frontStart;
    public float3 frontEnd;
    public float3 backStart;
    public float3 backEnd;
}