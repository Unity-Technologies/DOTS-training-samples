using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(PierreDebug_CarriageInitSystem))] // Make sure some initialising is already done
public partial struct CommuterStateSystem : ISystem
{
    private BufferLookup<StationPlatform> m_StationPlatformLookupRO;
    private BufferLookup<PlatformStairs> m_PlatformStairsLookupRO;
    private ComponentLookup<Parent> m_ParentLookupRO;
    private ComponentLookup<Stair> m_StairsLookupRO;
    private ComponentLookup<WorldTransform> m_WorldTransformLookupRO;
    private ComponentLookup<Carriage> m_CarriageLookupRO;
    private BufferLookup<PlatformQueue> m_PlatformQueueLookupRO;

    private ComponentLookup<QueueState> m_QueueStateLookupRW;
    private ComponentLookup<Seat> m_SeatLookupRW;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_StationPlatformLookupRO = state.GetBufferLookup<StationPlatform>(true);
        m_PlatformStairsLookupRO = state.GetBufferLookup<PlatformStairs>(true);
        m_ParentLookupRO = state.GetComponentLookup<Parent>(true);
        m_StairsLookupRO = state.GetComponentLookup<Stair>(true);
        m_WorldTransformLookupRO = state.GetComponentLookup<WorldTransform>(true);
        m_CarriageLookupRO = state.GetComponentLookup<Carriage>(true);
        m_PlatformQueueLookupRO = state.GetBufferLookup<PlatformQueue>(true);
        
        m_QueueStateLookupRW = state.GetComponentLookup<QueueState>(false);
        m_SeatLookupRW = state.GetComponentLookup<Seat>(false);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_StationPlatformLookupRO.Update(ref state);
        m_PlatformStairsLookupRO.Update(ref state);
        m_ParentLookupRO.Update(ref state);
        m_StairsLookupRO.Update(ref state);
        m_WorldTransformLookupRO.Update(ref state);
        m_CarriageLookupRO.Update(ref state);
        m_PlatformQueueLookupRO.Update(ref state);

        m_QueueStateLookupRW.Update(ref state);
        m_SeatLookupRW.Update(ref state);

        var updateCommuterStateParallelJob = new UpdateCommuterStateParallelJob()
        {
            StationPlatformLookup = m_StationPlatformLookupRO,
            ParentLookup = m_ParentLookupRO,
            StairsLookup = m_StairsLookupRO,
            PlatformStairsLookup = m_PlatformStairsLookupRO,
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        state.Dependency = updateCommuterStateParallelJob.ScheduleParallel(state.Dependency);
        

        var updateCommuterStateSingularJob = new UpdateCommuterStateSingularJob()
        {
            WorldTransformLookupRO = m_WorldTransformLookupRO,
            CarriageLookupRO = m_CarriageLookupRO,
            PlatformQueueLookupRO = m_PlatformQueueLookupRO,
            QueueStateLookupRW = m_QueueStateLookupRW,
            SeatLookupRW = m_SeatLookupRW,
        };
        state.Dependency = updateCommuterStateSingularJob.Schedule(state.Dependency);

    }
}

[BurstCompile]
public partial struct UpdateCommuterStateSingularJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<WorldTransform> WorldTransformLookupRO;
    [ReadOnly] public ComponentLookup<Carriage> CarriageLookupRO;
    [ReadOnly] public BufferLookup<PlatformQueue> PlatformQueueLookupRO;

    public ComponentLookup<QueueState> QueueStateLookupRW;
    public ComponentLookup<Seat> SeatLookupRW;

    [BurstCompile]
    public void Execute(ref Commuter commuter,
        ref SaschaMovementQueue movementQueue, ref QueueingData queueingData, ref SeatReservation seatReservation)
    {
        switch (commuter.State)
        {
            case CommuterState.PickQueue:
            {
                PlatformQueueLookupRO.TryGetBuffer(commuter.CurrentPlatform, out var platformQueues);
                if (platformQueues.Length > 0)
                {
                    commuter.State = CommuterState.Queueing;

                    var targetQueue = platformQueues[commuter.Random.NextInt(0, platformQueues.Length)].Queue;
                    var queueState = QueueStateLookupRW.GetRefRW(targetQueue, false);
                    queueingData.TargetQueue = targetQueue;
                    queueingData.PositionInQueue = queueState.ValueRO.QueueSize;

                    ++queueState.ValueRW.QueueSize;
                }

                break;
            }

            case CommuterState.Queueing:
            {
                if (seatReservation.TargetSeat != Entity.Null)
                {
                    commuter.State = CommuterState.Boarding;
                }

                break;
            }


            case CommuterState.InTrainReadyToUnboard:
            {
                //TODO: wait for train to be at the right station

                WorldTransformLookupRO.TryGetComponent(seatReservation.TargetCarriage, out var carriageTransform);
                SeatLookupRW.GetRefRW(seatReservation.TargetSeat, false).ValueRW.IsTaken = false;

                var (targetPlatform, targetQueue) =
                    FindTargetPlatformQueue(seatReservation.TargetCarriage);
                WorldTransformLookupRO.TryGetComponent(targetQueue, out var queueTransform);

                seatReservation.TargetSeat = Entity.Null;
                seatReservation.TargetCarriage = Entity.Null;

                movementQueue.QueuedInstructions.Clear();
                movementQueue.QueuedInstructions.Enqueue(new()
                {
                    Destination = carriageTransform.Position
                });
                movementQueue.QueuedInstructions.Enqueue(new()
                {
                    Destination = queueTransform.Position,
                    Platform = targetPlatform
                });

                commuter.State = CommuterState.Unboarding;
                break;
            }
        }
    }

    [BurstCompile]
    private (Entity, Entity) FindTargetPlatformQueue(Entity carriageEntity)
    {
        if (CarriageLookupRO.TryGetComponent(carriageEntity, out var carriage) &&
            PlatformQueueLookupRO.TryGetBuffer(carriage.CurrentPlatform, out var platformQueues) &&
            platformQueues.Length > carriage.CarriageNumber)
        {
            return (carriage.CurrentPlatform, platformQueues[carriage.CarriageNumber].Queue);
        }

        return (Entity.Null, Entity.Null);
    }
}


[BurstCompile]
public partial struct UpdateCommuterStateParallelJob : IJobEntity
{
    [ReadOnly] public BufferLookup<StationPlatform> StationPlatformLookup;
    [ReadOnly] public ComponentLookup<Parent> ParentLookup;
    [ReadOnly] public ComponentLookup<Stair> StairsLookup;
    [ReadOnly] public BufferLookup<PlatformStairs> PlatformStairsLookup;

    [ReadOnly] public float DeltaTime;

    [BurstCompile]
    public void Execute(ref Commuter commuter, ref LocalTransform commuterTransform, ref TargetDestination targetDestination,
        ref SaschaMovementQueue movementQueue)
    {
        switch (commuter.State)
        {
            case CommuterState.Idle:
                if (commuter.Random.NextBool())
                    PickNextPlatformDestination(ref commuter, ref commuterTransform, ref movementQueue);
                else
                    commuter.State = CommuterState.PickQueue;
                break;
            case CommuterState.MoveToDestination:
            case CommuterState.Queueing:
            case CommuterState.Boarding:
            case CommuterState.Unboarding:
                UpdateMoveToDestination(ref commuter, ref commuterTransform, ref targetDestination, ref movementQueue);
                break;
        }
    }

    [BurstCompile]
    private void UpdateMoveToDestination(ref Commuter commuter, ref LocalTransform commuterTransform, ref TargetDestination targetDestination, ref SaschaMovementQueue movementQueue)
    {
        // Find current move destination
        float3 currentDestination;
        if (targetDestination.IsActive)
        {
            // Commuter needs to move to the active TargetDestination
            currentDestination = targetDestination.TargetPosition;
        }
        else
        {
            if (movementQueue.QueuedInstructions.IsCreated == false ||
                movementQueue.QueuedInstructions.TryDequeue(out var nextInstruction) == false)
            {
                // No active TargetDestination and no further movement instructions in the queue. We need to switch states to determine our next actions, and stop moving for the time being

                commuter.State = commuter.State switch
                {
                    CommuterState.Boarding => CommuterState.InTrain,
                    CommuterState.Unboarding => CommuterState.Idle,
                    CommuterState.MoveToDestination => CommuterState.PickQueue
                };
                return;
            }

            // Update the TargetDestination and commuter's platform location with the new instruction
            currentDestination = nextInstruction.Destination;
            commuter.CurrentPlatform = nextInstruction.Platform;
            targetDestination.TargetPosition = nextInstruction.Destination;
            targetDestination.IsActive = true;
        }

        // Calculate the distance that the commuter should move this update
        float movementMagnitude = commuter.Velocity * DeltaTime;
        float movementMagnitudeSquared = movementMagnitude * movementMagnitude;
        
        // Calculate the distance remaining to current destination
        var remainingPathVector = currentDestination - commuterTransform.Position;
        float remainingPathLengthSquared = math.lengthsq(remainingPathVector);
        if (remainingPathLengthSquared < movementMagnitudeSquared)
        {
            // Commuter can move to/further than current destination, set commuter to destination position and mark TargetDestination inactive
            commuterTransform.Position = currentDestination;
            targetDestination.IsActive = false;
            // TODO: the remaining distance that can be traveled during this tick is not applied towards next waypoint
        }
        else
        {
            // Commuter won't reach current destination this update, move the full distance
            var directionalVector = math.normalize(remainingPathVector);
            commuterTransform.Position += (directionalVector * movementMagnitude);
        }

    }

    [BurstCompile]
    private void PickNextPlatformDestination(ref Commuter commuter, ref LocalTransform commuterTransform, ref SaschaMovementQueue movementQueue)
    {
        if (commuter.CurrentPlatform == Entity.Null)
        {
            Debug.LogError("Commuter doesn't have a current platform");
            return;
        }
        
        // Find the station of the current platform the player is on
        ParentLookup.TryGetComponent(commuter.CurrentPlatform, out var parent);
        
        // Get all the platforms that are in the current station, and pick a random different platform then the current one
        StationPlatformLookup.TryGetBuffer(parent.Value, out var stationPlatforms);
        var randomPlatformIndex = commuter.Random.NextInt(0, stationPlatforms.Length - 1); // max is excluded, but use length - 1 as one platform in the list is the same as the current

        for (int i = 0; i <= randomPlatformIndex; ++i)
        {
            if (stationPlatforms[i].Platform == commuter.CurrentPlatform)
            {
                ++randomPlatformIndex;
                break;
            }
        }

        // Find the stairs components for current platform and the destination platform
        // First find the closest stairs on the current platform
        PlatformStairsLookup.TryGetBuffer(commuter.CurrentPlatform, out var currentPlatformStairsBuffer);
        int platformStairsIndex = 0;
        float closestStairsDistanceSq = float.MaxValue;
        for (int i = 0; i < currentPlatformStairsBuffer.Length; ++i)
        {
            StairsLookup.TryGetComponent(currentPlatformStairsBuffer[i].Stairs, out var stairs);
            float distanceSq = math.distancesq(commuterTransform.Position, stairs.BottomWaypoint);
            if (distanceSq < closestStairsDistanceSq)
            {
                platformStairsIndex = i;
                closestStairsDistanceSq = distanceSq;
                
            }
        }
        StairsLookup.TryGetComponent(currentPlatformStairsBuffer[platformStairsIndex].Stairs, out var currentPlatformStairs);
        
        // Then get the matching index stairs for the destination platform
        var destinationPlatformEntity = stationPlatforms[randomPlatformIndex].Platform;
        PlatformStairsLookup.TryGetBuffer(destinationPlatformEntity, out var destinationPlatformStairsBuffer);
        StairsLookup.TryGetComponent(destinationPlatformStairsBuffer[platformStairsIndex].Stairs, out var destinationPlatformStairs);

        // Ensure the movementQueue is initialised and in a cleared state
        if (movementQueue.QueuedInstructions.IsCreated)
        {
            movementQueue.QueuedInstructions.Clear();
        }
        else
        {
            movementQueue.QueuedInstructions = new NativeQueue<SaschaMovementQueueInstruction>(Allocator.Persistent);
        }

        // Queue the movements
        // Movement 1: Go towards the current platform stairs
        movementQueue.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = currentPlatformStairs.BottomWaypoint,
            Platform = commuter.CurrentPlatform
        });
        
        // Movement 2: Go up the stairs
        movementQueue.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = currentPlatformStairs.TopWaypoint,
            Platform = Entity.Null
        });
        
        // Movement 3: Go into the walkway by the current stairs
        movementQueue.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = currentPlatformStairs.TopWalkwayWaypoint,
            Platform = Entity.Null
        });
        
        // Movement 4: Go to the walkway waypoint of the stairs by the destination platform
        movementQueue.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = destinationPlatformStairs.TopWalkwayWaypoint,
            Platform = Entity.Null
        });
        
        // Movement 5: Go to the top of the stair
        movementQueue.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = destinationPlatformStairs.TopWaypoint,
            Platform = Entity.Null
        });
        
        // Movement 6: Go down the stairs
        movementQueue.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = destinationPlatformStairs.BottomWaypoint,
            Platform = destinationPlatformEntity
        });
        
        // Set the current commuter state to MoveToDestination to start moving the commuter
        commuter.State = CommuterState.MoveToDestination;
    }
}
