using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct KinematicBody : IComponentData
    {
        public float3 velocity;
        public float3 landPosition;
    }

    public struct KinematicBodyState : ISharedComponentData
    {
        public byte isEnabled;
    }
}