using Unity.Entities;

namespace Components
{
    public struct PhysicsSettings : IComponentData
    {
        public float damping;
        public float friction;
        public float breakResistance;
        public float expForce;
    }
}