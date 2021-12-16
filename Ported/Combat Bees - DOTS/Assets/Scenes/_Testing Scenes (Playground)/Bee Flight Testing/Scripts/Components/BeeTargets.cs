using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees.Testing.BeeFlight
{
    public struct BeeTargets : IComponentData
    {
        public Entity ResourceTarget;
        public float3 HomeTarget;
        public float TargetReach;
    }
}