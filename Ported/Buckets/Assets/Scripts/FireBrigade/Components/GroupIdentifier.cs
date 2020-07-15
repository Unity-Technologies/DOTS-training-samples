using Unity.Entities;

namespace FireBrigade.Authoring
{
    [GenerateAuthoringComponent]
    public struct GroupIdentifier : IComponentData
    {
        public int Value;
    }
}