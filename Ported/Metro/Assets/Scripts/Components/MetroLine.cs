using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct MetroLine : IComponentData
{
    public NativeArray<Entity> Platforms;
    public NativeArray<Entity> Trains;
    
    public NativeArray<float3> RailwayPositions;
    public NativeArray<quaternion> RailwayRotations;
    public NativeArray<RailwayPointType> RailwayTypes;
    public NativeArray<int> StationIds;

    public float4 Color;
}