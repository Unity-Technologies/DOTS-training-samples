using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

readonly partial struct FollowAspect : IAspect<FollowAspect>
{
    public readonly Entity Self;

    private readonly TransformAspect Transform;

    private readonly RefRO<Follow> Follow;

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public float3 Offset
    {
        get => Follow.ValueRO.Offset;
    }

    public Entity ThingToFollow
    {
        get => Follow.ValueRO.EntityToFollow;
    }
}
