using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Color : IComponentData
    {
        public float4 color;
    }
}