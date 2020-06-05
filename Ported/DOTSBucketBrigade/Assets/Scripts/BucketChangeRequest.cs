using Unity.Entities;
public struct BucketChangeRequest : IComponentData
{
    public Entity From;
    public Entity To;
    public Entity Bucket;
}
