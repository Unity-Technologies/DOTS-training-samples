using Unity.Entities;

public struct BucketHeld : IComponentData
{
    public Entity Value;
    public bool IsFull;
}
