using Unity.Entities;
using Unity.Mathematics;

public unsafe struct IntersectionBuffer : IBufferElementData
{
    public float3 position;
    public fixed int neighbors[3];
}