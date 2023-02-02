using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TrainStateMachine : ISystem
{
    private ComponentLookup<Platform> allPlatforms;
    private BufferLookup<PlatformEntity> platformBuffer; 

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Train>();
        allPlatforms = SystemAPI.GetComponentLookup<Platform>();
        platformBuffer = SystemAPI.GetBufferLookup<PlatformEntity>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        allPlatforms.Update(ref state);
        platformBuffer.Update(ref state);
        
        new TrainStateDecider
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            allStations = platformBuffer,
            allPlatforms = allPlatforms
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct TrainStateDecider : IJobEntity
{
    public float deltaTime;
                                                                //platform, train
    //[NativeDisableContainerSafetyRestriction] public NativeHashMap<Entity, int> trainsArrivedAtStation;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<Platform> allPlatforms;
    [NativeDisableContainerSafetyRestriction] public BufferLookup<PlatformEntity> allStations;

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
                    InformLeftStation(trainScheduling);
                    trainScheduling.train.ValueRW.currentStation = Entity.Null;
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
                    InformAtStation(trainScheduling);
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
        int stationCount = allStations[trainScheduling.train.ValueRO.Line].Length;
        int currentStationIndex = trainScheduling.train.ValueRO.nextStationIndex;

        //Flip direction at the end of the line
        if (currentStationIndex + trainScheduling.train.ValueRO.direction >= stationCount ||
            currentStationIndex + trainScheduling.train.ValueRO.direction < 0)
        {
            trainScheduling.train.ValueRW.direction *= -1;
        }

        DynamicBuffer<PlatformEntity> lineStations = allStations[trainScheduling.train.ValueRO.Line];
        trainScheduling.train.ValueRW.currentStation = lineStations[currentStationIndex].Station;
        
        int nextStationIndex = currentStationIndex + trainScheduling.train.ValueRO.direction;
        trainScheduling.train.ValueRW.nextStationIndex = nextStationIndex;

        trainScheduling.targetDestination.target.ValueRW.TargetPosition = lineStations[nextStationIndex].StopPos;
    }

    private void SetInitialStation(TrainSchedulingAspect trainScheduling)
    {
        const int defaultStartStation = 0;
        trainScheduling.train.ValueRW.nextStationIndex = defaultStartStation;
        trainScheduling.train.ValueRW.direction = 1;
        trainScheduling.targetDestination.target.ValueRW.TargetPosition = allStations[trainScheduling.train.ValueRO.Line][defaultStartStation].StopPos;
    }

    private void InformAtStation(TrainSchedulingAspect trainScheduling)
    {
        allPlatforms.GetRefRW(trainScheduling.train.ValueRW.currentStation, false).ValueRW.ParkedTrain 
            = trainScheduling.train.ValueRW.entity;   
    }

    private void InformLeftStation(TrainSchedulingAspect trainScheduling)
    {
        allPlatforms.GetRefRW(trainScheduling.train.ValueRW.currentStation, false).ValueRW.ParkedTrain 
            = Entity.Null;
    }
}