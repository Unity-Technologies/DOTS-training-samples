using Unity.Entities;

public enum EThrowerState
{
    WaitForBucket,
    FindFire,
    WalkToFire,
    EmptyBucket,
    PassBackBucket,
};

[GenerateAuthoringComponent]
public struct ThrowerState : IComponentData
{
    public EThrowerState State;
}
