using Unity.Entities;

[GenerateAuthoringComponent]
public struct Gravity : IComponentData
{
    public float Value;
}