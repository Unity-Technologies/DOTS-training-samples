using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(TargetSystem))]
[UpdateAfter(typeof(BucketTargetingSystem))]
[UpdateBefore(typeof(MovementSystem))]
[BurstCompile]
partial struct OmniWorkerAiSystem : ISystem
{
    ComponentLookup<HasReachedDestinationTag> m_HasReachedDestinationTagLookup;
    ComponentLookup<WaterAmount> m_WaterAmountLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_HasReachedDestinationTagLookup = state.GetComponentLookup<HasReachedDestinationTag>();
        m_WaterAmountLookup = state.GetComponentLookup<WaterAmount>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_HasReachedDestinationTagLookup.Update(ref state);
        m_WaterAmountLookup.Update(ref state);

        var omniWorkerJob = new OmniWorkerAIJob()
        {
            HasReachedDestinationTagLookup = m_HasReachedDestinationTagLookup,
            WaterAmountLookup = m_WaterAmountLookup
        };
        state.Dependency = omniWorkerJob.Schedule(state.Dependency);
    }
}

[WithAll(typeof(HasReachedDestinationTag))]
[BurstCompile]
partial struct OmniWorkerAIJob : IJobEntity
{
    public ComponentLookup<HasReachedDestinationTag> HasReachedDestinationTagLookup;
    public ComponentLookup<WaterAmount> WaterAmountLookup;
    void Execute(in Entity entity, in Target target, in BucketTargetPosition bucketTargetPosition, in CarriedBucket carriedBucket, ref OmniWorkerAIState state, ref MoveInfo moveInfo)
    {
        switch (state.omniWorkerState)
        {
            case OmniWorkerState.Idle:
                moveInfo.destinationPosition = bucketTargetPosition.position;
                state.omniWorkerState = OmniWorkerState.FetchingBucket;
                HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                break;
            case OmniWorkerState.FetchingBucket:
                if (carriedBucket.bucket != Entity.Null)
                {
                    moveInfo.destinationPosition = target.waterCellPosition;
                    state.omniWorkerState = OmniWorkerState.FillingBucket;
                    HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                }
                break;
            case OmniWorkerState.FillingBucket:
                if (carriedBucket.bucket != Entity.Null)
                {
                    var waterAmount = WaterAmountLookup.GetRefRO(carriedBucket.bucket).ValueRO;
                    if (waterAmount.currentContain == waterAmount.maxContain)
                    {
                        moveInfo.destinationPosition = target.flameCellPosition;
                        state.omniWorkerState = OmniWorkerState.EmptyingBucket;
                        HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                    }
                }
                else
                {
                    Debug.Log($"{entity}");
                }

                break;
            case OmniWorkerState.EmptyingBucket:
                if (carriedBucket.bucket != Entity.Null)
                {
                    var waterAmount = WaterAmountLookup.GetRefRO(carriedBucket.bucket).ValueRO;
                    if (waterAmount.currentContain == 0)
                    {
                        moveInfo.destinationPosition = target.waterCellPosition;
                        state.omniWorkerState = OmniWorkerState.FillingBucket;
                        HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                    }
                }
                else
                {
                    Debug.Log($"{entity}");
                }
                break;
        }
    }
}
