using Unity.Entities;

public struct MyBucketState : IComponentData
{
    public BucketState Value;
    public int FrameChanged;

    public MyBucketState(BucketState value, int frame)
    {
        Value = value;
        FrameChanged = frame;
    }
}
