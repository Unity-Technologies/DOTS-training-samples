using Unity.Entities;

namespace FireBrigade.Authoring
{
    [GenerateAuthoringComponent]
    public struct GroupIndex : IComponentData
    {
        public int Value;
    }
}