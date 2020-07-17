using Unity.Entities;


namespace Water
{
    public struct ExtinguishAmount : IComponentData
    {
        public float Value;
        public bool Propagate;
    }
}
