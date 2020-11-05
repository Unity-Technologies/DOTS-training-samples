using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveTowardFire : IComponentData
{
    public Entity Target;
}
