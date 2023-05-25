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