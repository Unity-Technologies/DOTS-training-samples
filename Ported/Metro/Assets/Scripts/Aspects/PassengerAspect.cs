using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct PassengerAspect : IAspect
{
    // An Entity field in an aspect provides access to the entity itself.
    // This is required for registering commands in an EntityCommandBuffer for example.
    public readonly Entity Self;

    // Aspects can contain other aspects.
    readonly TransformAspect Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRW<Passenger> Passenger;

    readonly RefRO<SpeedComponent> SpeedComponent;

    // Properties like this are not mandatory, the Transform field could just have been made public instead.
    // But they improve readability by avoiding chains of "aspect.aspect.aspect.component.value.value".
    public float3 Position
    {
        get => Transform.LocalPosition;
        set => Transform.LocalPosition = value;
    }
    
    public quaternion Rotation
    {
        get => Transform.LocalRotation;
        set => Transform.LocalRotation = value;
    }

    public PassengerState State
    {
        get => Passenger.ValueRO.State;
        set => Passenger.ValueRW.State = value;
    }

    public float Speed
    {
        get => SpeedComponent.ValueRO.Current;
    }
}