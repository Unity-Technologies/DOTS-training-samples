using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct FetcherAspect : IAspect<FetcherAspect>
{
    public readonly Entity Self;

    private readonly TransformAspect Transform;

    private readonly RefRW<Fetcher> Fetcher;

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public float3 Speed
    {
        get => Fetcher.ValueRO.Speed;
        set => Fetcher.ValueRW.Speed = value;
    }

    public Entity TargetBucket
    {
        get => Fetcher.ValueRO.TargetBucket;
        set => Fetcher.ValueRW.TargetBucket = value;
    }
}