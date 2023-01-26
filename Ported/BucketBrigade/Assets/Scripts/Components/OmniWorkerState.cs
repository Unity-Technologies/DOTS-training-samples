using Unity.Entities;

public struct OmniWorkerAIState : IComponentData
{
    public OmniWorkerState omniWorkerState;
}

public enum OmniWorkerState
{
    Idle,
    FetchingBucket,
    FillingBucket,
    EmptyingBucket
}
