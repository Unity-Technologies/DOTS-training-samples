using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Position : IComponentData
{
    public float3 Value;
}
