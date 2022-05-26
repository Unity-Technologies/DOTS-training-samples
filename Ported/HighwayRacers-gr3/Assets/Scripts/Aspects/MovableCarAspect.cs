using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct MovableCarAspect : IAspect<MovableCarAspect>
{
    public readonly Entity self;
    private readonly TransformAspect Transfrom;

    private readonly RefRO<CarId> m_CarId;
    private readonly RefRO<CruisingSpeed> m_CruisingSpeed;
    private readonly RefRW<CarTraveledDistance> m_TraveledDistance;
    private readonly RefRO<CurrentLaneComponent> m_Lane;
    private readonly RefRO<LaneComponent> m_LaneComponent;

    public int Id
    {
        get => m_CarId.ValueRO.Value;
    }

    public float3 Position
    {
        get => Transfrom.Position;
        set => Transfrom.Position = value;
    }

    public quaternion Rotation
    {
        get => Transfrom.Rotation;
        set => Transfrom.Rotation = value;
    }
    
    public float Speed
    {
        get => m_CruisingSpeed.ValueRO.Value;
    }

    public float TraveledDistance
    {
        get => m_TraveledDistance.ValueRW.Value;
        set => m_TraveledDistance.ValueRW.Value = value;
    }

    public int Lane
    {
        get => m_LaneComponent.ValueRO.LaneNumber;
    }
}
