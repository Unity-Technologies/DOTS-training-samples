using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Size : IComponentData
{
    // TODO: move Begin/End/Speed to constant data and store in Singleton
    public float3 BeginSize;
    public float3 EndSize;
    public float Time;
    public float Speed;
}
