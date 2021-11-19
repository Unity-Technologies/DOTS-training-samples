using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Team : IComponentData
{
    public Entity targetFire;
    public int2 targetFirePosition;
    public float3 targetWaterPosition;
    public Entity fullBucketWorker;
    public Entity emptyBucketWorker;
    public Entity bucketFetcherWorker;
}