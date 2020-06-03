using Unity.Entities;

[GenerateAuthoringComponent]
public struct TargetBucket : IComponentData
{
    public Entity Target;
}