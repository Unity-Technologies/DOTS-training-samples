using Unity.Entities;

namespace FireBrigade.Components
{
    [GenerateAuthoringComponent]
    public struct RoleIndex : IComponentData
    {
        public int Value;
    }
}