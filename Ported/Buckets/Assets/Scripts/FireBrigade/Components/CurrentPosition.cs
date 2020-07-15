using Unity.Entities;
using Unity.Mathematics;

namespace FireBrigade.Components
{
    [GenerateAuthoringComponent]
    public struct CurrentPosition : IComponentData
    {
        public float3 Value;
    }
}