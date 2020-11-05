using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveTowardWater : IComponentData
{
    public Entity Target;
}
