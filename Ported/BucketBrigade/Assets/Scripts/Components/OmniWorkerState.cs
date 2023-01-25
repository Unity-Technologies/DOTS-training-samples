using Unity.Entities;

public struct OmniWorkerAIState : IComponentData
{
    public OmniWorkerState omniWorkerState;
}

public enum OmniWorkerState
{
    FetchingBucket,
    FillingBucket,
    EmptyingBucket
}
