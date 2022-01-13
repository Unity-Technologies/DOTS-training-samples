using Unity.Entities;
using Unity.Mathematics;

namespace CombatBees.Testing.BeeFlight
{
    public struct Holder : IComponentData
    {
        public Entity BeeEntity;
        public float3 Offset;
    }
}