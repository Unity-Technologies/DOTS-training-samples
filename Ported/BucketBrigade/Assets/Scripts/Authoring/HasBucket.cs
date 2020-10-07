using Unity.Entities;

[GenerateAuthoringComponent]
public struct HasBucket : IComponentData
{
    public bool Has;
    public Entity Entity;
}