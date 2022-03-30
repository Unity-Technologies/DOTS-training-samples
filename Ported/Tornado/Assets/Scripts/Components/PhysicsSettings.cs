using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct PhysicsSettings : IComponentData
    {
        public float damping;
        public float friction;
        public float breakResistance;
        public float expForce;
        public int constraintIterations;
        public float gravityForce;
    }
} 