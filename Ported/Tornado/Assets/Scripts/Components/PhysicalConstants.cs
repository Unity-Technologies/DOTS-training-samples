using Unity.Entities;

namespace Assets.Scripts.Components
{
    [GenerateAuthoringComponent]

    public struct PhysicalConstants : IComponentData
    {
        public float friction;
        public float airResistance;
        public float breakingDistance;
        public float gravity;
    }
}
