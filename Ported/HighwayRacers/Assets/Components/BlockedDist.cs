using Unity.Entities;

namespace HighwayRacer
{
    public struct BlockedDist : IComponentData
    {
        public float Val;   // distance to next car that triggers blocked state (once blocked, a car tries to overtake)  
    }
}