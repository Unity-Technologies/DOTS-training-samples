using Unity.Entities;

[GenerateAuthoringComponent]
public struct InitBucketData : IComponentData
{
    public int BucketCount;
    public Entity BucketPrefab;
}

