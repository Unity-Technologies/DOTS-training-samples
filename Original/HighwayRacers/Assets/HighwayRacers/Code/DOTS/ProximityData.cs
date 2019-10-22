using HighwayRacers;
using Unity.Entities;

public struct ProximityData : IComponentData
{
    public HighwaySpacePartition.QueryResult data;
}
