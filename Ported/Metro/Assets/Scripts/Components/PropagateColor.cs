using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct PropagateColor : IComponentData
    {
        public float4 color;
    }
}