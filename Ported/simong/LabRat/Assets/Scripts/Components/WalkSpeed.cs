using Unity.Entities;

[GenerateAuthoringComponent]
public struct WalkSpeed : IComponentData
{
    public float Value;
    public float RotationSpeed;
}
