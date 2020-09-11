using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MousePosition : IComponentData
{
    public float2 Value;
}