using Unity.Entities;

namespace Components
{
    public struct BeeProperties : IComponentData
    {
        public BeeMode BeeMode;

        // Data sheet lists a TargetBee but no TargetFood, using a single target for now.
        public Entity Target;

        public float Aggressivity;
    }
}