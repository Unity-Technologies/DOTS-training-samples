using Unity.Entities;

namespace FireBrigade.Components
{
    [GenerateAuthoringComponent]
    public struct MovementSpeed : IComponentData
    {
        public float Value;
    }
}