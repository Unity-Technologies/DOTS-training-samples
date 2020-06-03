using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetLane : IComponentData
{
    public int Value;
}