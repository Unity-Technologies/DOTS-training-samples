using Unity.Entities;

public struct ShrinkAndDestroy : IComponentData
{
    public float lifetime;
    public float age;
}