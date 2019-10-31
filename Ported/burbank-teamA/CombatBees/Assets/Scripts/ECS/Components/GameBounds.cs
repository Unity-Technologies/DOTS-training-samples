using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct GameBounds : IComponentData
{
    public float3 Value;
    public float PlayAreaPercentage;
}