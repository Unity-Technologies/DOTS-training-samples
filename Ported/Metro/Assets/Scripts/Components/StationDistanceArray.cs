using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct StationDistanceArrayData
{
    public BlobArray<float> Distances;
}
public struct StationDistanceArray : IComponentData
{
    public BlobAssetReference<StationDistanceArrayData> StationDistances;
}
