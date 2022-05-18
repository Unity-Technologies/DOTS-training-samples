using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct FetcherTargetAspect : IAspect<FetcherTargetAspect>
{ 
    public readonly Entity Self;

    private readonly TransformAspect Transform;

    private readonly RefRW<FetcherTarget> FetcherTarget;

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }
}