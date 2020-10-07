using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Target : IComponentData
{
    public float2 Position;
    public Entity Entity;
    public bool ReachedTarget;
}