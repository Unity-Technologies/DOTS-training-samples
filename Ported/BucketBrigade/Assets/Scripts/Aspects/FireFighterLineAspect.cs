using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct FireFighterLineAspect : IAspect<FireFighterLineAspect>
{
    public readonly Entity Self;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRW<FireFighterLine> FireFighterLine;

    public float2 StartPosition
    {
        get => FireFighterLine.ValueRO.StartPosition;
    }
    public float2 EndPosition
    {
        get => FireFighterLine.ValueRO.EndPosition;
        set => FireFighterLine.ValueRW.EndPosition = value;
    }
}