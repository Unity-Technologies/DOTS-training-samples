using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components
{
    public struct Car : IComponentData
    {
        public float Speed;
        public float SafeDistance;
        
        public Entity Track;
        // ¿ lane ?
        public float T;
        
    }
}
