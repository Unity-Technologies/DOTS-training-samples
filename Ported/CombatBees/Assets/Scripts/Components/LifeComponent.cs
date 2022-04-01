using Unity.Entities;

[GenerateAuthoringComponent]
public struct LifeComponent : IComponentData
{
    public float Value;
    public float Duration;
}
