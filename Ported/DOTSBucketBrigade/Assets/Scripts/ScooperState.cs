using Unity.Entities;

public enum EScooperState
{
    FindBucket,
    WalkToBucket,
    FindWater,
    WalkToWater,
    FillBucket,
    PassBucket,
};

[GenerateAuthoringComponent]
public struct ScooperState : IComponentData
{
    public EScooperState State;
}
