using Unity.Entities;

public readonly partial struct CarAspect : IAspect<CarAspect>
{
    readonly RefRO<CurrentSpeedComponent> m_currentSpeed;
    readonly RefRO<CruisingSpeed> m_cruisingSpeed;
    readonly RefRO<LaneComponent> m_LaneComponent;

    public readonly Entity carEntity;

    public int Lane
    {
        get => m_LaneComponent.ValueRO.LaneNumber;
    }

    public float CurrentSpeed
    {
        get => m_currentSpeed.ValueRO.CurrentSpeed;
    }

    public float CruisingSpeed
    {
        get => m_cruisingSpeed.ValueRO.Value;
    }
}
