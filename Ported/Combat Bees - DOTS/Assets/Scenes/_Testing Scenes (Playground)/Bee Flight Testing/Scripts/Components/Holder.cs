using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{
    public struct Holder : IComponentData
    {
        public Entity Value;
        public int TestNumber;
    }
}