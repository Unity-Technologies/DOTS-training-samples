using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components
{
    struct Car : IComponentData
    {
        public float Speed;
        public Entity Track;
    }
}
