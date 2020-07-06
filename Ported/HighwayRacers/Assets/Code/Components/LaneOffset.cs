using Unity.Entities;

namespace HighwayRacer
{
    public struct LaneOffset : IComponentData
    {
        public float Val;   // where the car should be rendered relative to its assigned lane
                            // (merging cars instantly join the new lane, but are rendered moving into the new lane incrementally)
    }
}