using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct RoadAspect : IAspect<RoadAspect>
{
    private readonly RefRW<Road> m_Road;

    public readonly Entity Self;
    private readonly TransformAspect Transform;

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public quaternion Rotation
    {
        get => Transform.Rotation;
        set => Transform.Rotation = value;
    }
}