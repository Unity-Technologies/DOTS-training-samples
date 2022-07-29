using Unity.Entities;

struct BucketFillerFetcher : IComponentData
{
    public enum BucketFillerFetcherState
    {
        GoToBucket,
        GoToLake,
        FillBucket
    };
    public BucketFillerFetcherState state;
}