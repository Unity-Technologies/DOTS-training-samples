using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct KinematicBody : IComponentData
    {
        public float3 Velocity;
    }
}