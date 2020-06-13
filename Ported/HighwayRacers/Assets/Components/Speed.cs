using Unity.Entities;

namespace HighwayRacer
{
    public struct Speed : IComponentData
    {
        public float Val;   // meters per second; actual current speed for this frame
    }
}