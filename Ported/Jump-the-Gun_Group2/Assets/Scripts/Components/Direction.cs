using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Direction : IComponentData
{
    public float2 Value;
}
