using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
        foreach (var (commuter, queueingData, seatReservation, movementQueue)
                 in SystemAPI.Query<RefRW<Commuter>, RefRW<QueueingData>, RefRW<SeatReservation>, RefRW<SaschaMovementQueue>>())
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
                
                
                case CommuterState.InTrain:
                {
                    //TODO: wait for train to be at the right station

                    var carriageTransform = SystemAPI.GetComponent<WorldTransform>(seatReservation.ValueRO.TargetCarriage);
                    SystemAPI.SetComponent(seatReservation.ValueRO.TargetSeat, new Seat(){ IsTaken = false });

                    var (targetPlatform, targetQueue) = FindTargetPlatformQueue(ref state, seatReservation.ValueRO.TargetCarriage);
                    var queueTransform = SystemAPI.GetComponent<WorldTransform>(targetQueue);

                    seatReservation.ValueRW.TargetSeat = Entity.Null;
                    seatReservation.ValueRW.TargetCarriage = Entity.Null;

                    movementQueue.ValueRW.QueuedInstructions.Clear();
                    movementQueue.ValueRW.QueuedInstructions.Enqueue(new()
                    {
                        Destination = carriageTransform.Position
                    });
                    movementQueue.ValueRW.QueuedInstructions.Enqueue(new()
                    {
                        Destination = queueTransform.Position,
                        Platform = targetPlatform
                    });

                    commuter.ValueRW.State = CommuterState.Unboarding;
                    break;
                }
            }
        }

    }

    [BurstCompile]
    private (Entity, Entity) FindTargetPlatformQueue(ref SystemState state, Entity carriage)
    {
        foreach (var (platform, platformEntity) in SystemAPI.Query<Platform>().WithEntityAccess())
        {
            var queues = SystemAPI.GetBuffer<PlatformQueue>(platformEntity);
            foreach (var queue in queues)
            {
                var queueState = SystemAPI.GetComponent<QueueState>(queue.Queue);
                if (queueState.FacingCarriage == carriage)
                {
                    return (platformEntity, queue.Queue);
                }
            }
        }

        return (Entity.Null, Entity.Null);
    }
}
