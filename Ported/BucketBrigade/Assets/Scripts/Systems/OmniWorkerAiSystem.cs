using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct OmniWorkerAiSystemData : IComponentData
{
    public double NextUpdateTime;
}

// [UpdateAfter(typeof(TargetSystem))]
[UpdateAfter(typeof(BucketTargetingSystem))]
[UpdateBefore(typeof(MovementSystem))]
[BurstCompile]
partial struct OmniWorkerAiSystem : ISystem
{
    // [ReadOnly] ComponentLookup<HasReachedDestinationTag> m_HasReachedDestinationTagLookup;
    ComponentLookup<WaterAmount> m_WaterAmountLookup;

    const double k_TimeBetweenUpdates = 1;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // m_HasReachedDestinationTagLookup = state.GetComponentLookup<HasReachedDestinationTag>();
        m_WaterAmountLookup = state.GetComponentLookup<WaterAmount>();
        state.EntityManager.AddComponent<OmniWorkerAiSystemData>(state.SystemHandle);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // var currentTime = SystemAPI.Time.ElapsedTime;
        // if (currentTime < SystemAPI.GetComponent<OmniWorkerAiSystemData>(state.SystemHandle).NextUpdateTime) return;
        // SystemAPI.SetComponent(state.SystemHandle, new OmniWorkerAiSystemData() { NextUpdateTime = currentTime + k_TimeBetweenUpdates});

        // m_HasReachedDestinationTagLookup.Update(ref state);
        m_WaterAmountLookup.Update(ref state);

        var omniWorkerJob = new OmniWorkerAIJob()
        {
            WaterAmountLookup = m_WaterAmountLookup,
            MoveInfoLookup = state.GetComponentLookup<MoveInfo>(),
            // HasReachedDestinationTagLookup = state.GetComponentLookup<HasReachedDestinationTag>(),
            ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        };
        state.Dependency = omniWorkerJob.Schedule(state.Dependency);

        // omniWorkerJob.Complete();
    }
}

[WithAll(typeof(HasReachedDestinationTag))]
[BurstCompile]
partial struct OmniWorkerAIJob : IJobEntity
{
    // [ReadOnly] public ComponentLookup<HasReachedDestinationTag> HasReachedDestinationTagLookup;
    [ReadOnly] public ComponentLookup<WaterAmount> WaterAmountLookup;
    public ComponentLookup<MoveInfo> MoveInfoLookup;
    // public ComponentLookup<HasReachedDestinationTag> HasReachedDestinationTagLookup;

    public EntityCommandBuffer.ParallelWriter ECB;

    void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity, in Target target, in BucketTargetPosition bucketTargetPosition, in CarriedBucket carriedBucket, ref OmniWorkerAIState state)
    {
        // bool isMoving = MoveInfoLookup.HasComponent(entity);
        MoveInfo moveInfo = new();
        bool isMoving = MoveInfoLookup.TryGetComponent(entity, out moveInfo);

        var self = this;

        void ChangeToFilling(Entity entity1, Target target1, out OmniWorkerAIState omniWorkerAIState)
        {
            // self.MoveInfoLookup.GetRefRW(entity1, false).ValueRW.destinationPosition = target1.waterCellPosition;
            // moveInfo.destinationPosition = target.waterCellPosition;
            omniWorkerAIState.omniWorkerState = OmniWorkerState.FillingBucket;
        }

        void ChangeToEmptying(Entity entity2, Target target2, out OmniWorkerAIState state1)
        {
            // self.MoveInfoLookup.GetRefRW(entity2, false).ValueRW.destinationPosition = target2.flameCellPosition;
            // moveInfo.destinationPosition = target.flameCellPosition;
            state1.omniWorkerState = OmniWorkerState.EmptyingBucket;
        }

        void ChangeToIdle(int i, Entity entity3, out OmniWorkerAIState omniWorkerAIState1)
        {
            omniWorkerAIState1.omniWorkerState = OmniWorkerState.Idle;
            self.ECB.SetComponentEnabled<HasReachedDestinationTag>(i, entity3, false); // should always interact with that tag at the end of the frame. Order of systems is a mess
        }

        switch (state.omniWorkerState)
        {
            case OmniWorkerState.Idle:
                if (carriedBucket.bucket == Entity.Null)
                {
                    // moveInfo.destinationPosition = bucketTargetPosition.position;
                    if (!isMoving)
                    {
                        MoveInfoLookup.SetComponentEnabled(entity, true);
                        // ECB.AddComponent(chunkIndex, entity, moveInfo);
                        // isMoving = true;
                    }

                    state.omniWorkerState = OmniWorkerState.FetchingBucket;
                }
                else if (WaterAmountLookup.GetRefRO(carriedBucket.bucket).ValueRO.currentContain == 0)
                {
                    ChangeToFilling(entity, target, out state);
                }
                else if (WaterAmountLookup.GetRefRO(carriedBucket.bucket).ValueRO.currentContain > 0)
                {
                    ChangeToEmptying(entity, target, out state);
                }
                else if (isMoving) // has a bucket and is moving but is idle
                {
                    MoveInfoLookup.SetComponentEnabled(entity, false);
                    // ECB.RemoveComponent<MoveInfo>(chunkIndex, entity);
                    // isMoving = false;
                }

                // HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);
                // ECB.SetComponentEnabled<HasReachedDestinationTag>(chunkIndex, entity, false);
                break;
            case OmniWorkerState.FetchingBucket:
                if (carriedBucket.bucket != Entity.Null)
                {
                    if (target.waterTargetEntity != Entity.Null)
                    {
                        ChangeToFilling(entity, target, out state);
                    }
                    else
                    {
                        ChangeToIdle(chunkIndex, entity, out state);
                    }
                    // HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);

                    // ECB.SetComponentEnabled<HasReachedDestinationTag>(chunkIndex, entity, false);
                }

                break;
            case OmniWorkerState.FillingBucket:
                if (carriedBucket.bucket != Entity.Null)
                {
                    var waterAmount = WaterAmountLookup.GetRefRO(carriedBucket.bucket).ValueRO;

                    bool waterCellExists = WaterAmountLookup.HasComponent(target.waterTargetEntity);
                    if (waterAmount.currentContain == waterAmount.maxContain || !waterCellExists)
                    {
                        if (target.fireTargetEntity != Entity.Null)
                        {
                            ChangeToEmptying(entity, target, out state);
                        }
                        else
                        {
                            ChangeToIdle(chunkIndex, entity, out state);
                            state.omniWorkerState = OmniWorkerState.Idle;
                        }
                        // HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);

                        // ECB.SetComponentEnabled<HasReachedDestinationTag>(chunkIndex, entity, false);
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
                        if (target.waterTargetEntity != Entity.Null)
                        {
                            ChangeToFilling(entity, target, out state);
                        }
                        else
                        {
                            ChangeToIdle(chunkIndex, entity, out state);
                        }
                        // HasReachedDestinationTagLookup.SetComponentEnabled(entity, false);

                        // ECB.SetComponentEnabled<HasReachedDestinationTag>(chunkIndex, entity, false);
                    }
                }
                else
                {
                    Debug.Log($"{entity}");
                }

                break;
        }

        // if (isMoving)
        // {
        //     ECB.SetComponent(chunkIndex, entity, moveInfo);
        // }
    }
}
