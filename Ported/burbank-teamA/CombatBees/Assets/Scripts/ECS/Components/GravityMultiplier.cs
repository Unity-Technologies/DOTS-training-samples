using Unity.Entities;

[GenerateAuthoringComponent]
public struct GravityMultiplier : IComponentData
{
    public float Value;
}
