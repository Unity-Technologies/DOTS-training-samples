using Unity.Entities;

namespace FireBrigade.Components
{
    [GenerateAuthoringComponent]
    public struct GroupIndex : IComponentData
    {
        public int Value;
    }
}