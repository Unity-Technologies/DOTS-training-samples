using Unity.Entities;

public readonly partial struct LaneAspect : IAspect<LaneAspect>
{
    public readonly Entity self;

    readonly RefRO<LaneTag> m_LaneTag;
    readonly RefRO<LaneComponent> m_LaneComponent;

    public int Lane
    {
        get => m_LaneComponent.ValueRO.LaneNumber;
    }
}
