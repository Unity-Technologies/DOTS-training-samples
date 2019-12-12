using Unity.Entities;
using Unity.Mathematics;

public struct RenderMatrixEntry : IBufferElementData
{
    public float4x4 Value;
} 
