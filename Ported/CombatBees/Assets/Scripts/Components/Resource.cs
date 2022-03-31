using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Resource : IComponentData
    {
        public float3 OwnerPosition;
    }

    public struct ResourceOwner : IComponentData
    {
        public Entity Owner;
    }
}