using Unity.Entities;

public struct EmptyBucket : IComponentData
{
}

public struct BucketReadyFor : IComponentData
{
    public int Index;
}

public struct FullBucket : IComponentData
{
}
public struct BucketForScooper : IComponentData
{
}