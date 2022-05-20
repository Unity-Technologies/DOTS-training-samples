using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct FiremanAspect : IAspect<FiremanAspect>
{
    public readonly Entity Self;

    private readonly RefRW<Fireman> Worker;

    private readonly TransformAspect Transform;

    public float Speed
    {
        get => Worker.ValueRW.Speed;
        set => Worker.ValueRW.Speed = value;
    }

    public float SearchRadius
    {
        get => Worker.ValueRW.SearchRadius;
        set => Worker.ValueRW.SearchRadius = value;
    }

    public float3 Destination
    {
        get => Worker.ValueRW.Destination;
        set => Worker.ValueRW.Destination = value;
    }

    public float3 Position
    {
        get => Transform.Position;
        set => Transform.Position = value;
    }

    public FiremanState FiremanState
    {
        get => Worker.ValueRO.State;
        set => Worker.ValueRW.State = value;
    }

    public void Translate(float3 translation)
    {
        Transform.TranslateWorld(translation);
    }
}
