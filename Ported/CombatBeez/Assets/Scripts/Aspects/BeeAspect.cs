using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct BeeAspect : IAspect<BeeAspect>
{
    // An Entity field in an aspect provides access to the entity itself.
    // This is required for registering commands in an EntityCommandBuffer for example.
    public readonly Entity Self;

    // Aspects can contain other aspects.
    public readonly TransformAspect Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRW<Bee> _Bee;

    readonly RefRW<NonUniformScale> _Scale;

    // Properties like this are not mandatory, the Transform field could just have been made public instead.
    // But they improve readability by avoiding chains of "aspect.aspect.aspect.component.value.value".
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

    public float3 Scale
    {
        get => _Scale.ValueRW.Value;
        set => _Scale.ValueRW.Value = value;
    }

    public float3 Target
    {
        get => _Bee.ValueRO.Target;
        set => _Bee.ValueRW.Target = value;
    }

    public Entity TargetResource
    {
        get => _Bee.ValueRO.TargetResource;
        set => _Bee.ValueRW.TargetResource = value;
    }

    public bool AtBeeTarget
    {
        get => _Bee.ValueRO.AtTarget;
        set => _Bee.ValueRW.AtTarget = value;
    }

    public Entity TargetBee
    {
        get => _Bee.ValueRO.TargetBee;
        set => _Bee.ValueRW.TargetBee = value;
    }

    public float3 SpawnPoint
    {
        get => _Bee.ValueRO.SpawnPoint;
    }

    public Bee Bee
    {
        get => _Bee.ValueRW;
    }

    public Bee.BEESTATE BeeState
    {
        get => _Bee.ValueRW.beeState;
        set => _Bee.ValueRW.beeState = value;
    }

    public float3 OcillateOffset
    {
        get => _Bee.ValueRW.OcillateOffset;
        set => _Bee.ValueRW.OcillateOffset = value;
    }
}
