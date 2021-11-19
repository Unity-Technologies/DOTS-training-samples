using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct EmptyBucketWorker : IComponentData
{
    public Entity nextWorker;
    // public Entity HeldBucket;    // We decided to remove this and just use a Bucket component in each worker Entity
}