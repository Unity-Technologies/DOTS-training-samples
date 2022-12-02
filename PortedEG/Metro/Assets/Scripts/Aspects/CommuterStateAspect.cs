using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct CommuterStateUpdateAspect : IAspect
{
    public readonly Entity Self;

    // Aspects can contain other aspects.
    readonly TransformAspect Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRW<CommuterStateInfo> StateInfo;
}