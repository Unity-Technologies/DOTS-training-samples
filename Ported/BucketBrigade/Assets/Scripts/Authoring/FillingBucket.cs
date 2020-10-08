using Unity.Entities;

[GenerateAuthoringComponent]
public struct FillingBucket : IComponentData
{
    public bool Filling;
    public bool Full;
    public Entity Entity;
}