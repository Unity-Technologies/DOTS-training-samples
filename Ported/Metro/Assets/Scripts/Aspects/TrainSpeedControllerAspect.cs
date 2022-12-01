using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct TrainSpeedControllerAspect : IAspect
{
    // Aspects can contain other aspects.
    readonly RefRO<WorldTransform> Transform;

    // A RefRW field provides read write access to a component. If the aspect is taken as an "in"
    // parameter, the field will behave as if it was a RefRO and will throw exceptions on write attempts.
    readonly RefRW<SpeedComponent> SpeedComponent;
    readonly RefRO<Train> Train;

    readonly RefRO<MetroLineID> LineID;
    readonly RefRO<UniqueTrainID> TrainID;

    readonly RefRW<TrainStateComponent> StateComponent;
    
    public TrainState State
    {
        get => StateComponent.ValueRO.State;
        set => StateComponent.ValueRW.State = value;
    }

    public int MetroLineID => LineID.ValueRO.ID;

    public int NextTrainID => TrainID.ValueRO.NextTrainID;

    public float Speed
    {
        get => SpeedComponent.ValueRO.Current;
        set => SpeedComponent.ValueRW.Current = value;
    }

    public float MaxSpeed => SpeedComponent.ValueRO.Max;

    public float3 Position => Transform.ValueRO.Position;

    public float3 Destination => Train.ValueRO.Destination;
    public RailwayPointType DestinationType => Train.ValueRO.DestinationType;
}