using Unity.Entities;

namespace FireBrigade.Components
{
    [GenerateAuthoringComponent]
    public struct GroupIdentifier : IComponentData
    {
        public int Value;
    }
}