using Unity.Entities;

[GenerateAuthoringComponent]
public struct JitterForce : IComponentData
{
    public float Value;
}