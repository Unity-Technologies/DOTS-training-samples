using Unity.Entities;
using Unity.Mathematics;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct PropagateColor : IComponentData
    {
        public float4 color;
    }
}