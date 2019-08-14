using Unity.Entities;
using Unity.Mathematics;

public unsafe struct IntersectionPoint : IBufferElementData
{
    public float3 Position;
    public fixed int Neighbors[3];
}