using Unity.Entities;

namespace HighwayRacer
{
    public struct UnblockedSpeed : IComponentData
    {
        public float Val;    // meters per second; desired cruising speed when unblocked (and not merging or overtaking)
    }
}