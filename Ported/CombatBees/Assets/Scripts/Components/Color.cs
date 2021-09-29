using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Color : IComponentData
{
    public float3 BeginColor;
    public float3 EndColor;
    public float Time;
    public float Speed;
}
