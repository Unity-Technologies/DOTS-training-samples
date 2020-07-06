using Unity.Entities;

namespace HighwayRacer
{
    public struct Blocking : IComponentData
    {
        public float Dist;              // distance to next car that triggers blocked state (once blocked, a car tries to overtake)
    }
}