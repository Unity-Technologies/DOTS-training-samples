using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{
    [GenerateAuthoringComponent]
    public struct BeeResourcePair : IBufferElementData
    {
        public Entity BeeEntity;
        public Entity ResourceEntity;
    }
}