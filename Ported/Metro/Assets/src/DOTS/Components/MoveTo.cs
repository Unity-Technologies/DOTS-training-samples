using Unity.Entities;
using Unity.Mathematics;

namespace src.DOTS.Components
{
    public struct MoveTo : IComponentData
    {
        public float3 target;
    }
}