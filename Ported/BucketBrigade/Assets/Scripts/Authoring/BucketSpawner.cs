using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BucketSpawner : IComponentData
{
    public Entity bucketPrefab;
    public int numberBuckets;
    public float spawnRadius;
    public float2 spawnCenter;
}