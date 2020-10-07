using Unity.Entities;

[GenerateAuthoringComponent]
public struct Timer : IComponentData
{
    public float Value;
}