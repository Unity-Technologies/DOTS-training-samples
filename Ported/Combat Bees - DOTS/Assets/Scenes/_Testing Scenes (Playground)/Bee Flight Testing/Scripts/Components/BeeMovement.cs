using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees.Testing.BeeFlight
{
    public struct BeeMovement : IComponentData
    {
        public float3 Velocity;
        public float Speed;
        public float ChaseForce;
        public float Damping;
    }
}