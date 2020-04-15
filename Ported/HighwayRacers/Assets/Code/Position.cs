using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Position : IComponentData
{
    public float2 Value;
}
