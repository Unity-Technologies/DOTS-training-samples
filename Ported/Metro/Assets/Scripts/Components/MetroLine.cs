using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct MetroLine : IComponentData
{
    public NativeArray<int> Platforms; 
    
    public NativeArray<float3> RailwayPositions;
    public NativeArray<quaternion> RailwayRotations;
    public NativeArray<RailwayPointType> RailwayTypes;
    public NativeArray<int> StationIds;

    public float4 Color;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (float3,RailwayPointType, int)  GetNextRailwayPoint(int index)
    {
        var nextIndex = index - 1;
        if (nextIndex < 0)
            nextIndex = RailwayPositions.Length - 1;
        return (RailwayPositions[nextIndex], RailwayTypes[nextIndex], nextIndex);
    }
}