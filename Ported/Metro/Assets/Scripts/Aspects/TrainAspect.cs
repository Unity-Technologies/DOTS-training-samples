using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct TrainAspect : IAspect
{
    public readonly Entity Self;
    
    // Aspects can contain other aspects.
    readonly TransformAspect Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    public readonly RefRW<Train> Train;
    readonly RefRW<TrainStateComponent> StateComponent;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
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

    public float3 Forward => Transform.Forward;
    public Entity MetroLine => Train.ValueRO.MetroLine;

    public TrainState State
    {
        get => StateComponent.ValueRO.State;
        set => StateComponent.ValueRW.State = value;
    }

    public float3 Destination
    {
        get => Train.ValueRO.Destination;
        set => Train.ValueRW.Destination = value;
    }

    public RailwayPointType DestinationType
    {
        get => Train.ValueRO.DestinationType;
        set => Train.ValueRW.DestinationType = value;
    }
    
    public int DestinationIndex
    {
        get => Train.ValueRO.DestinationIndex;
        set => Train.ValueRW.DestinationIndex = value;
    }

    public float CurrentSpeed => SpeedComponent.ValueRO.Current;
}