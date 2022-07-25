using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    struct Car : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
        public float3 Direction;
        public float Speed;
        public Entity Track;
    }
}
