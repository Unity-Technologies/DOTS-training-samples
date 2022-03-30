using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct TornadoParameters : IComponentData
    {
        public float tornadoFader;
        public float3 eyePosition;
    }
}