using Unity.Entities;

namespace HighwayRacer
{
    public struct DesiredSpeed : IComponentData
    {
        public float Unblocked;    // meters per second; desired cruising speed when unblocked (and not merging or overtaking)
        public float Overtake;     // meters per second; set target speed to this when switching to overtake state
    }
}