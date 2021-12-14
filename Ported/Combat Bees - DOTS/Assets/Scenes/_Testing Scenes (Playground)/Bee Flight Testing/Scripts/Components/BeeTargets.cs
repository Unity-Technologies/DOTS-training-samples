using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees.Testing.BeeFlight
{
    public struct BeeTargets : IComponentData
    {
        public float3 LeftTarget;
        public float3 RightTarget;
        public float3 CurrentTarget;
        public float TargetReach;
    }
}