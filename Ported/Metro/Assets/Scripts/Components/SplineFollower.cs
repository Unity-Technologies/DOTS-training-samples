using Unity.Entities;

[GenerateAuthoringComponent]
public struct SplineFollower : IComponentData
{
    public Entity track;
}