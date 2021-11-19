using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public int GridSize;
    public Entity HeldBucket;
    public int FullBucketWorkerPerTeamCount;
    public int EmptyBucketWorkerPerTeamCount;
    public int BucketCount;
    public int TeamCount;
    public int OmniworkerCount;
    public int StartingFireCount;

    public Entity CellPrefab;
    public Entity WaterPatchPrefab;
    public Entity BucketPrefab;
    public Entity FullBucketWorkerPrefab;
    public Entity EmptyBucketWorkerPrefab;
    public Entity WaterFetcherWorkerPrefab;
    public Entity OmniWorkerWorkerPrefab;
}