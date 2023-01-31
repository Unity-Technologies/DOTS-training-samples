using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct PierreDebug_QueueInputSystem : ISystem
{
    Random m_Random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        m_Random = Random.CreateFromIndex(1234);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var doorTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Door>())
            {
                doorTransform.ValueRW.Scale = doorTransform.ValueRW.Scale > 0 ? 0 : 1;
            }

            foreach (var train in SystemAPI.Query<RefRW<Train>>())
            {
                train.ValueRW.State = train.ValueRW.State == TrainState.Idle ? TrainState.Boarding : TrainState.Idle;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var config = SystemAPI.GetSingleton<Config>();
             ecb.Instantiate(config.CommuterPrefab);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            NativeList<Entity> queues = new(Allocator.Temp);
            foreach (var (queue, queueEntity) in SystemAPI.Query<Queue>().WithEntityAccess())
            {
                queues.Add(queueEntity);
            }

            foreach (var (commuter, queueingData, seatReservation, targetDestination)
                     in SystemAPI.Query<RefRW<Commuter>, RefRW<QueueingData>, RefRW<SeatReservation>, RefRW<TargetDestination>>())
            {
                switch (commuter.ValueRO.State)
                {
                    case CommuterState.Idle:
                    {
                        var targetQueue = queues[math.abs(m_Random.NextInt()) % queues.Length];
                        var queueState = SystemAPI.GetComponent<QueueState>(targetQueue);
                        queueingData.ValueRW.TargetQueue = targetQueue;
                        queueingData.ValueRW.PositionInQueue = queueState.QueueSize;

                        ++queueState.QueueSize;
                        SystemAPI.SetComponent(targetQueue, queueState);

                        commuter.ValueRW.State = CommuterState.Queueing;
                        break;
                    }

                    case CommuterState.InTrain:
                    {
                        SystemAPI.SetComponent(seatReservation.ValueRO.TargetSeat, new Seat(){ IsTaken = false });
                        seatReservation.ValueRW.TargetSeat = Entity.Null;

                        targetDestination.ValueRW.TargetPosition = float3.zero;
                        commuter.ValueRW.State = CommuterState.Unboarding;
                        break;
                    }
                }
            }
        }
    }
}
