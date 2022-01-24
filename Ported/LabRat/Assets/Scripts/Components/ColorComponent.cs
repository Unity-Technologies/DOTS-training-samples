using Unity.Entities;
using Unity.Mathematics;

public struct ColorComponent : IComponentData
{
    public float4 color;

    public ColorComponent(float4 color)
    {
        this.color = color;
    }
}