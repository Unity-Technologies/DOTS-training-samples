using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Temperature : IComponentData
{
    public float Value;
}
