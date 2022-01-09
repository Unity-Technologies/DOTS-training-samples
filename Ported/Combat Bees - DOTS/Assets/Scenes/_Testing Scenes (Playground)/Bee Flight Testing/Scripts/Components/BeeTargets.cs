using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    public struct BeeTargets : IComponentData
    {
        public Entity ResourceTarget;
        public float3 HomePosition;
        public float TargetReach;
        public Random random;
    }
}