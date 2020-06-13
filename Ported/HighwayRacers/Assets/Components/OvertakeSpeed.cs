using Unity.Entities;

namespace HighwayRacer
{
    public struct OvertakeSpeed : IComponentData
    {
        public float Val;    // meters per second; set target speed to this when switching to overtake state
    }
}