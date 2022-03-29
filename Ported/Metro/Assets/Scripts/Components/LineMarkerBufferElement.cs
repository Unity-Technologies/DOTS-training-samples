using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(16)]
public struct LineMarkerBufferElement : IBufferElementData
{
    public float3 Position;
    public bool IsPlatform;
}