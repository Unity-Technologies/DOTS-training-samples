using Unity.Entities;
using Unity.Mathematics;

public struct Color : IComponentData
{
    public float4 Value;

    public Color(float4 color)
    {
        Value = color;
    }
}