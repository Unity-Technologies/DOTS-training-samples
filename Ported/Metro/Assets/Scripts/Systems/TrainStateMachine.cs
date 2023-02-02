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
    private ComponentLookup<QueueState> allQueueStates;
    private ComponentLookup<Carriage> allCarriages;
    private BufferLookup<PlatformEntity> allStations; 
    private BufferLookup<PlatformQueue> allPlatformQueues; 
    private BufferLookup<Child> allChildren; 

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Train>();
        allPlatforms = SystemAPI.GetComponentLookup<Platform>();
        platformBuffer = SystemAPI.GetBufferLookup<PlatformEntity>();
        allQueueStates = SystemAPI.GetComponentLookup<QueueState>();
        allCarriages = SystemAPI.GetComponentLookup<Carriage>();
        allStations = SystemAPI.GetBufferLookup<PlatformEntity>();
        allPlatformQueues = SystemAPI.GetBufferLookup<PlatformQueue>(true);
        allChildren = SystemAPI.GetBufferLookup<Child>(true);
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
        
        RefRW<RandomComponent> rand = SystemAPI.GetSingletonRW<RandomComponent>();
        
                    //platformID, trainID
        //NativeHashMap<Entity, int> trainsArrivedAtStation = new NativeHashMap<int, int>(0, Allocator.TempJob);
        new TrainStateDecider
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            allStations = allStations,
            allPlatforms = allPlatforms,
            allPlatformQueues = allPlatformQueues,
            allQueueStates = allQueueStates,
            allChildren = allChildren,
            allCarriages = allCarriages,
            random = rand
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct TrainStateDecider : IJobEntity
{
    public float deltaTime;

    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> random;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<Platform> allPlatforms;
    [NativeDisableContainerSafetyRestriction] public BufferLookup<PlatformEntity> allStations;
    [NativeDisableContainerSafetyRestriction] public BufferLookup<PlatformQueue> allPlatformQueues;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<QueueState> allQueueStates;
    [NativeDisableContainerSafetyRestriction] public BufferLookup<Child> allChildren;
    [NativeDisableContainerSafetyRestriction] public ComponentLookup<Carriage> allCarriages;

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
                    trainScheduling.train.ValueRW.currentPlatform = Entity.Null;
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
                    SetRandomWaitTime(trainScheduling);
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
        trainScheduling.train.ValueRW.currentPlatform = lineStations[currentStationIndex].Station;
        
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
        var arrivalPlatform = allPlatforms.GetRefRW(trainScheduling.train.ValueRW.currentPlatform, false);
        arrivalPlatform.ValueRW.ParkedTrain = trainScheduling.train.ValueRW.entity;
        var platformQueues = allPlatformQueues[trainScheduling.train.ValueRW.currentPlatform];
        foreach (var platformQueue in platformQueues)
        {
            allQueueStates.GetRefRW(platformQueue.Queue, false).ValueRW.IsOpen = true;
        }

        allChildren.TryGetBuffer(trainScheduling.train.ValueRW.entity, out var trainChildrenBuffer);
        foreach (var trainChild in trainChildrenBuffer)
        {
            if (allCarriages.HasComponent(trainChild.Value))
            {
                allCarriages.GetRefRW(trainChild.Value, false).ValueRW.CurrentPlatform =
                    trainScheduling.train.ValueRW.currentPlatform;
            }
        }
    }

    private void InformLeftStation(TrainSchedulingAspect trainScheduling)
    {
        var departurePlatform = allPlatforms.GetRefRW(trainScheduling.train.ValueRW.currentPlatform, false);
        departurePlatform.ValueRW.ParkedTrain = Entity.Null;
        var platformQueues = allPlatformQueues[trainScheduling.train.ValueRW.currentPlatform];
        foreach (var platformQueue in platformQueues)
        {
            allQueueStates.GetRefRW(platformQueue.Queue, false).ValueRW.IsOpen = false;
        }

        allChildren.TryGetBuffer(trainScheduling.train.ValueRW.entity, out var trainChildrenBuffer);
        foreach (var trainChild in trainChildrenBuffer)
        {
            if (allCarriages.HasComponent(trainChild.Value))
            {
                allCarriages.GetRefRW(trainChild.Value, false).ValueRW.CurrentPlatform = Entity.Null;
            }
        }
    }

    private void SetRandomWaitTime(TrainSchedulingAspect trainScheduling)
    {
        if (this.random.IsValid == false)
        {
            Debug.Log("No Random Component Fonnd please add a object with RandomAuthoring to induce some randomness");
            trainScheduling.schedule.ValueRW.stationWaitTime = trainScheduling.schedule.ValueRO.baseStationWaitTime;
            return;
        }
        
        float varianceLimit = trainScheduling.schedule.ValueRO.waitTimeRandomVariance;
        float randomtime = this.random.ValueRW.random.NextFloat(-varianceLimit, varianceLimit);
        float baseWaitTime = trainScheduling.schedule.ValueRO.baseStationWaitTime;
        trainScheduling.schedule.ValueRW.stationWaitTime = baseWaitTime + randomtime;
    }
}