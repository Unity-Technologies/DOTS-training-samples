using Unity.Entities;
using Unity.Mathematics;

struct Fetcher : IComponentData
{
    public Entity TargetBucket;
    
    public float3 Speed;
}