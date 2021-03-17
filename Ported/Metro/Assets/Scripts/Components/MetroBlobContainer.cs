using System;
using Unity.Entities;
using Unity.Mathematics;

public struct MetroBlobContaner : IComponentData
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
    public int PlatformStartIndex;
    public int PlatformEndIndex;
}