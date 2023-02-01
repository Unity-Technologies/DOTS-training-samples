using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct CommuterStateSystem : ISystem
{
    Random m_Random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_Random = Random.CreateFromIndex(1234);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (commuter, queueingData, seatReservation, destinationAspect)
                 in SystemAPI.Query<RefRW<Commuter>, RefRW<QueueingData>, RefRO<SeatReservation>, DestinationAspect>())
        {
            switch (commuter.ValueRO.State)
            {
                case CommuterState.PickQueue:
                {
                    var platformQueues = SystemAPI.GetBuffer<PlatformQueue>(commuter.ValueRO.CurrentPlatform);
                    if (platformQueues.Length > 0)
                    {
                        commuter.ValueRW.State = CommuterState.Queueing;

                        var targetQueue = platformQueues[math.abs(m_Random.NextInt()) % platformQueues.Length].Queue;
                        var queueState = SystemAPI.GetComponent<QueueState>(targetQueue);
                        queueingData.ValueRW.TargetQueue = targetQueue;
                        queueingData.ValueRW.PositionInQueue = queueState.QueueSize;

                        ++queueState.QueueSize;
                        SystemAPI.SetComponent(targetQueue, queueState);
                    }

                    break;
                }

                case CommuterState.Queueing:
                {
                    if (seatReservation.ValueRO.TargetSeat != Entity.Null)
                    {
                        commuter.ValueRW.State = CommuterState.Boarding;
                    }

                    break;
                }

                case CommuterState.Boarding:
                {
                    if (!destinationAspect.IsAtDestination())
                    {
                        commuter.ValueRW.State = CommuterState.InTrain;
                        //TODO: Set seat as parent
                    }
                    break;
                }

                case CommuterState.Unboarding:
                {
                    if (!destinationAspect.IsAtDestination())
                    {
                        commuter.ValueRW.State = CommuterState.Idle;
                    }

                    break;
                }
            }
        }

    }
}
