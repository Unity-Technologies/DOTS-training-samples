using System;
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
partial struct OmniWorkerAiSystem : ISystem
{
    EntityQuery m_OmniWorkerQuery;
    
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // todo replace managed arrays in queries to allow burst compile
        m_OmniWorkerQuery = state.GetEntityQuery(ComponentType.ReadOnly<Target>(), ComponentType.ReadWrite<OmniWorkerAIState>(), ComponentType.ReadWrite<MoveInfo>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
    }
}

[WithAll(typeof(HasReachedDestinationTag))]
[BurstCompile]
partial struct OmniWorkerAIJob : IJobEntity
{
    void Execute(in Target target, ref OmniWorkerAIState state, ref MoveInfo moveInfo)
    {
        switch (state.omniWorkerState)
        {
            case OmniWorkerState.FetchingBucket:
                break;
            case OmniWorkerState.FillingBucket:
                break;
            case OmniWorkerState.EmptyingBucket:
                break;
            default:
        }
    }
}
