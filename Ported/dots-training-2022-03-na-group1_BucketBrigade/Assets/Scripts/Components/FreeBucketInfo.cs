using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct FreeBucketInfo : IBufferElementData
{
    public Entity BucketEntity;
    public Position BucketPosition;
    public MyBucketState BucketState;
}