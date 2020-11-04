using Unity.Entities;
using Unity.Mathematics;

namespace MetroECS.Comuting
{
    public struct MoveTarget : IBufferElementData
    {
        public float3 Position;
    }
}