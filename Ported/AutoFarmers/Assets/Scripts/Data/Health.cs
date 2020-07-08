using Unity.Entities;

[GenerateAuthoringComponent]
public struct Health : IComponentData
{
    public float Value;
}