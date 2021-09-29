using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Color : IComponentData
{
    // TODO: move Begin/End/Speed to constant data and store in Singleton
    public float4 BeginColor;
    public float4 EndColor;
    public float Time;
    public float Speed;
}
