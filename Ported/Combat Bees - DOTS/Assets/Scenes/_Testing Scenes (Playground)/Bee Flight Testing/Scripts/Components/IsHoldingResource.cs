using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{


    public struct IsHoldingResource : IComponentData
    {
        public bool Value;
        public bool JustPickedUp;
        public bool ReachedHome;
    }
}