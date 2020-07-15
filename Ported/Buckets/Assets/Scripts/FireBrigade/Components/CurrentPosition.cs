using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Authoring
{
    [GenerateAuthoringComponent]
    public struct CurrentPosition : IComponentData
    {
        public float3 Value;
    }
}