using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{
    public struct Bee : IComponentData
    {
        // Tag component
        public bool TeamA;
        public bool dead;
    }
}