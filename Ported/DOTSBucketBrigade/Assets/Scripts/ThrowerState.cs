using Unity.Entities;

public enum EThrowerState
{
    WaitForBucket,
    FindFire,
    WalkToFire,
    EmptyBucket,
    PassBucket,
    StartWalkingToChainEnd,
    WaitUntilChainEndInRange
};

[GenerateAuthoringComponent]
public struct ThrowerState : IComponentData
{
    public EThrowerState State;
}
