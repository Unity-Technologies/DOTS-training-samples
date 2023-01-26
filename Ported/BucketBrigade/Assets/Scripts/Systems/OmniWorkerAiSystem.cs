using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(TargetSystem))]
[UpdateAfter(typeof(BucketTargetingSystem))]
[UpdateBefore(typeof(MovementSystem))]
[BurstCompile]
partial struct OmniWorkerAiSystem : ISystem
{
    ComponentLookup<HasReachedDestinationTag> m_HasReachedDestinationTagLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_HasReachedDestinationTagLookup = state.GetComponentLookup<HasReachedDestinationTag>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_HasReachedDestinationTagLookup.Update(ref state);

        var omniWorkerJob = new OmniWorkerAIJob()
        {
            HasReachedDestinationTagLookup = m_HasReachedDestinationTagLookup,
        };
        omniWorkerJob.Schedule();
    }
}

[WithAll(typeof(HasReachedDestinationTag))]
[BurstCompile]
partial struct OmniWorkerAIJob : IJobEntity
{
    public ComponentLookup<HasReachedDestinationTag> HasReachedDestinationTagLookup;
    void Execute(in Entity entity, in Target target, in BucketTargetPosition bucketTargetPosition, ref OmniWorkerAIState state, ref MoveInfo moveInfo)
    {
        switch (state.omniWorkerState)
        {
            case OmniWorkerState.Idle:
                moveInfo.destinationPosition = bucketTargetPosition.position;
                state.omniWorkerState = OmniWorkerState.FetchingBucket;
                HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                break;
            case OmniWorkerState.FetchingBucket:
                moveInfo.destinationPosition = target.waterCellPosition;
                state.omniWorkerState = OmniWorkerState.FillingBucket;
                HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                break;
            case OmniWorkerState.FillingBucket:
                moveInfo.destinationPosition = target.flameCellPosition;
                state.omniWorkerState = OmniWorkerState.EmptyingBucket;
                HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                break;
            case OmniWorkerState.EmptyingBucket:
                moveInfo.destinationPosition = target.waterCellPosition;
                state.omniWorkerState = OmniWorkerState.FillingBucket;
                HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                break;
        }
    }
}
