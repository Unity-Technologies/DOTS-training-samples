using Unity.Entities;

[GenerateAuthoringComponent]
public struct Size : IComponentData
{
    public float BeginSize;
    public float EndSize;
    public float Time;
    public float Speed;
}
