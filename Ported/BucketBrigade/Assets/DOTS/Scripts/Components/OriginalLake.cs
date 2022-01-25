using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct OriginalLake : IComponentData
{
    public float3 Scale;
    public float Volume;
}