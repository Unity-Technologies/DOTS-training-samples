using Unity.Entities;

public enum RailwayPointType
{
    Platform,
    Route
}

public struct RailwayPoint : IComponentData
{
    public RailwayPointType RailwayPointType;
}