using HighwayRacers;
using Unity.Entities;

namespace HighwayRacers
{
    public struct ProximityData : IComponentData
    {
        public HighwaySpacePartition.QueryResult data;
    }
}
