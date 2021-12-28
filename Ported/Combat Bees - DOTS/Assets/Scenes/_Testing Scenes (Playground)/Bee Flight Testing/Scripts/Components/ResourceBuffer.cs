using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{
    [GenerateAuthoringComponent]
    public struct ResourceBuffer : IBufferElementData
    {
        public Entity Value;

    }
}
