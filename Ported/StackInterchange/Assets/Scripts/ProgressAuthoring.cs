using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Progress : IComponentData
{
    public float Value;
}
