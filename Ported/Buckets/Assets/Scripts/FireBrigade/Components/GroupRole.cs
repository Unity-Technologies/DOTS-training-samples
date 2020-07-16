using Unity.Entities;

namespace FireBrigade.Components
{
    public enum FirefighterRole
    {
        scooper,
        thrower,
        full,
        empty,
        collector
    }
    public struct GroupRole : IComponentData
    {
        public FirefighterRole Value;
    }
}