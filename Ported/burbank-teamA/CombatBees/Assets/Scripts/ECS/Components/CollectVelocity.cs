using Unity.Entities;

[GenerateAuthoringComponent]
public struct CollectVelocity : IComponentData
{
    public float Value;
}