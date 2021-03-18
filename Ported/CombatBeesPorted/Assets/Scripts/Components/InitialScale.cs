using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct InitialScale: IComponentData
{
    public float3 Value;
}