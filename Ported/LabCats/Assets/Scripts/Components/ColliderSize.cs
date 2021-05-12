using Unity.Entities;

[GenerateAuthoringComponent]
public struct ColliderSize : IComponentData
{
    public float X;
    public float Y;
}