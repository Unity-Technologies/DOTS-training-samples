using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Cooldown : IComponentData
{
    public float Value;
}
