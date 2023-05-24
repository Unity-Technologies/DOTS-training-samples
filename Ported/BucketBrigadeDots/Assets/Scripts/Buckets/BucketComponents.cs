using Unity.Entities;

public struct BucketSpawner : IComponentData
{
    public Entity BucketPrefab;
    public int NumberOfBuckets;
}

public struct BucketData : IComponentData
{
    public bool IsFull;
}