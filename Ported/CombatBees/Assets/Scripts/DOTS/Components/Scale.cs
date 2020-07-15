using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Scale : IComponentData
{
    public float3 Value;
}
