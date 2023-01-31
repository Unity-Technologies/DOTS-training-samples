using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
        Line testLine = SystemAPI.GetSingleton<Line>();

        foreach (RefRW<Train> train in SystemAPI.Query<RefRW<Train>>())
        {
            train.ValueRW.Line = testLine;
        }
        
        new TrainStateDecider
        {
            deltaTime = SystemAPI.Time.DeltaTime
        }.ScheduleParallel();
    }
}

[BurstCompile]
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
                SetInitialStation(trainScheduling);
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
                if (trainScheduling.schedule.ValueRW.timeInState >= trainScheduling.schedule.ValueRO.stationWaitTime)
                {
                    newState = TrainState.DoorClosing;
                }
                break;
            case TrainState.TrainMovement:
                if (trainScheduling.targetDestination.IsAtDestination())
                {
                    newState = TrainState.DoorOpening;
                    SetNextStation(trainScheduling);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (oldState != newState)
        {
            trainScheduling.train.ValueRW.State = newState;
            trainScheduling.schedule.ValueRW.timeInState = 0f;
        }
    }

    private void SetNextStation(TrainSchedulingAspect trainScheduling)
    {
        if (trainScheduling.train.ValueRO.nextStationIndex < 0)
        {
            SetInitialStation(trainScheduling);
            return;
        }
        
        //Get the current index of the "current" station
        int stationCount = trainScheduling.train.ValueRO.Line.platformStopPositions.Length;
        int currentStationIndex = trainScheduling.train.ValueRO.nextStationIndex;

        //Flip direction at the end of the line
        if (currentStationIndex + trainScheduling.train.ValueRO.direction >= stationCount ||
            currentStationIndex + trainScheduling.train.ValueRO.direction < 0)
        {
            trainScheduling.train.ValueRW.direction *= -1;
        }

        int nextStationIndex = currentStationIndex + trainScheduling.train.ValueRO.direction;
        trainScheduling.train.ValueRW.nextStationIndex = nextStationIndex;
        
        
        trainScheduling.targetDestination.target.ValueRW.TargetPosition = trainScheduling.train.ValueRO.Line.platformStopPositions[nextStationIndex];
    }

    private void SetInitialStation(TrainSchedulingAspect trainScheduling)
    {
        const int defaultStartStation = 0;
        trainScheduling.train.ValueRW.nextStationIndex = defaultStartStation;
        trainScheduling.train.ValueRW.direction = 1;
        trainScheduling.targetDestination.target.ValueRW.TargetPosition =
            trainScheduling.train.ValueRO.Line.platformStopPositions[defaultStartStation];
    }
}