using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct TrainStateMachine : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Train>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new TrainStateDecider
        {
            deltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
}

//[BurstCompile]
public partial struct TrainStateDecider : IJobEntity
{
    public float deltaTime;

    private void Execute(TrainSchedulingAspect trainScheduling)
    {
        trainScheduling.schedule.ValueRW.timeInState += deltaTime;
        
        TrainState oldState = trainScheduling.train.ValueRO.State;
        TrainState newState = oldState;
        
        switch (trainScheduling.train.ValueRO.State)
        {
            case TrainState.Idle:
                newState = TrainState.TrainMovement;
                break;
            case TrainState.DoorOpening:
                if (trainScheduling.schedule.ValueRW.timeInState >= trainScheduling.schedule.ValueRO.doorTransisionTime)
                {
                    newState = TrainState.Boarding;
                }
                break;
            case TrainState.DoorClosing:
                if (trainScheduling.schedule.ValueRW.timeInState >= trainScheduling.schedule.ValueRO.doorTransisionTime)
                {
                    newState = TrainState.TrainMovement;
                }
                break;
            case TrainState.Boarding:
                if (trainScheduling.schedule.ValueRW.timeInState >= trainScheduling.schedule.ValueRO.doorTransisionTime)
                {
                    newState = TrainState.DoorClosing;
                }
                break;
            case TrainState.TrainMovement:
                if (trainScheduling.targetDestination.IsAtDestination())
                {
                    newState = TrainState.DoorOpening;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (oldState != newState)
        {
            Debug.Log($"New State: {newState.ToString()}");
            trainScheduling.train.ValueRW.State = newState;
            trainScheduling.schedule.ValueRW.timeInState = 0f;
        }

    }
}