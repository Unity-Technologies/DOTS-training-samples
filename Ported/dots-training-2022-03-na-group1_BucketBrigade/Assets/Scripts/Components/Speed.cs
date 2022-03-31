using Unity.Entities;

public struct Speed : IComponentData
{
    public float Value;

    public static Speed Free => new Speed() { Value = BucketBrigadeUtility.FreeSpeed };
    public static Speed WithFullBucket => new Speed() { Value = BucketBrigadeUtility.FullBucketSpeed };
    public static Speed WithEmptyBucket => new Speed() { Value = BucketBrigadeUtility.EmptyBucketSpeed };
}
