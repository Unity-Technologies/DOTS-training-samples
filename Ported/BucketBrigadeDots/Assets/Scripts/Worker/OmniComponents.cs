using Unity.Entities;

public enum OmniStates
{
    Idle,
    MovingToBucket,
    FillingBucket,
    MovingToFire,
}

public struct OmniState : IComponentData
{
    public OmniStates Value;
}

public struct OmniData : IComponentData
{
    public bool HasBucket;
}