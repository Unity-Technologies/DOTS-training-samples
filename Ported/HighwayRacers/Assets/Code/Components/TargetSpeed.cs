using Unity.Entities;

namespace HighwayRacer
{
    public struct TargetSpeed : IComponentData
    {
        public float Val;   // meters per second; speed which we are currently accelerating or decelerating to
    }
}