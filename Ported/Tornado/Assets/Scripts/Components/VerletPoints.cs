using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct VerletPoints : IComponentData
    {
        public float3 oldPosition;
        public float3 currentPosition;
    }
}