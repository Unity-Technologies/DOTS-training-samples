using Unity.Entities;

public struct BucketHeld : IComponentData
{
    public Entity Value;
    public bool IsFull;

    public BucketHeld(Entity bucket, bool fullness)
    {
        Value = bucket;
        IsFull = fullness;
    }

    public static BucketHeld NoBucket => new BucketHeld(Entity.Null, false);
}
