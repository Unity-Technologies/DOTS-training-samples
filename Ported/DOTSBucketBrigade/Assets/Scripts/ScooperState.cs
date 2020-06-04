using Unity.Entities;

public enum EScooperState
{
    FindBucket,
    FindWater,
    StartWalkingToWater,
    WaitUntilWaterInRange,
    FillBucket,
    StartWalkingToChainStart,
    WaitUntilChainStartInRange,
    PassBucket,
    StartWalkingToBucket,
    WaitUntilBucketInRange
};

[GenerateAuthoringComponent]
public struct ScooperState : IComponentData
{
    public EScooperState State;
}
