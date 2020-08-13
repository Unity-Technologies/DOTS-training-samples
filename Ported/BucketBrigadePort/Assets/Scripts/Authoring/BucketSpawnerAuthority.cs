using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BucketSpawner : IComponentData
{
    public int TotalBuckets;
    public Entity Prefab;
}
