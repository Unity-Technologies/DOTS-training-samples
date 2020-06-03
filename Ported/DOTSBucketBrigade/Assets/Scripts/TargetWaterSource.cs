using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetWaterSource : IComponentData
{
    public Entity Target;
}