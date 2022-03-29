using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity FetcherPrefab;
    public Entity FireCaptainPrefab;
    public Entity WaterCaptainPrefab;
    public Entity FullBucketWorkerPrefab;
    public Entity EmptyBucketWorkerPrefab;
    public Entity OmniWorkerPrefab;
    public Entity WaterPoolPrefab;
    public Entity FlameCellPrefab;
    public Entity BucketPrefab;
    public Entity GroundPrefab;

    public int TeamCount;
    public int OmniWorkerCount;
    public int MembersCount;
    public int FireDimension;
    public int WaterCount;
    public int BucketCount;
    public int MinWaterSupplyCount;
    public int MaxWaterSupplyCount;
}
