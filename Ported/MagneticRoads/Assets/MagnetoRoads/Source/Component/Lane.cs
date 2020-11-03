using Unity.Entities;

[GenerateAuthoringComponent]
public struct Lane : IComponentData
{
    public float Length;
    public Entity Car;

}
