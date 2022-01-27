using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees.Testing.BeeFlight
{
    public struct BeeMovement : IComponentData
    {
        public float3 Velocity;
        public float ChaseForce;
        public float Damping;
        public float FlightJitter;
        public float RotationStiffness;
        public float TeamAttraction;
        public float3 SmoothPosition;
    }
}