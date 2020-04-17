using Unity.Entities;

[GenerateAuthoringComponent]
public struct GlobalPrefabs : IComponentData
{
    public Entity WorkerPrefab;
    public Entity BucketPrefab;
    public float BucketSpawnInterval;
    public float WorkerSpeed;
}