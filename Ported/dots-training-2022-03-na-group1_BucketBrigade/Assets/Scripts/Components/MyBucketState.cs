using Unity.Entities;

public struct MyBucketState : IComponentData
{
    public BucketState Value;
    public int FrameChanged;
}
