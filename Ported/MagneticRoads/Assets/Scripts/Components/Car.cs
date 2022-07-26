using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components
{
    public struct Car : IComponentData
    {
        public float Speed;
        public Entity Lane;
        public float SafeDistance;
    }
}
