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

    public float SpeedEmpty
    {
        get => Fetcher.ValueRO.SpeedEmpty;
        set => Fetcher.ValueRW.SpeedEmpty = value;
    }

    public float SpeedFull
    {
        get => Fetcher.ValueRO.SpeedFull;
        set => Fetcher.ValueRW.SpeedFull = value;
    }

    public Entity TargetPickUp
    {
        get => Fetcher.ValueRO.TargetPickUp;
        set => Fetcher.ValueRW.TargetPickUp = value;
    }
    
    public Entity TargetDropZone
    {
        get => Fetcher.ValueRO.TargetDropZone;
        set => Fetcher.ValueRW.TargetDropZone = value;
    }

    public FetcherState CurrentState
    {
        get => Fetcher.ValueRO.CurrentState;
        set => Fetcher.ValueRW.CurrentState = value;
    }
}
