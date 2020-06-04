using Unity.Entities;

public enum EThrowerState
{
    WaitForBucket,
    FindFire,
    WalkToFire,
    EmptyBucket,
    PassBucket,
    WaitUntilChainEndInRangeAndNotCarrying
};

[GenerateAuthoringComponent]
public struct ThrowerState : IComponentData
{
    public EThrowerState State;
}
