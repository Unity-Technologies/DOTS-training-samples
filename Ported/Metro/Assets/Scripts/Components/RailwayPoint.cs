using Unity.Entities;
using Unity.Mathematics;

public enum RailwayPointType
{
    Platform,
    Route
}

public struct RailwayPoint : IComponentData
{
    public RailwayPointType RailwayPointType;
    //public int StationId;

    public float3 PreviousPoint;
    public float3 NextPoint;
}