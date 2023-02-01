using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SaschaCommuterMovementSystem : ISystem
{
    private BufferLookup<StationPlatform> m_StationPlatformLookup;
    private ComponentLookup<Parent> m_ParentLookup;
    private ComponentLookup<Platform> m_PlatformLookup;
    private ComponentLookup<Stair> m_StairsLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_StationPlatformLookup = state.GetBufferLookup<StationPlatform>(true);
        m_ParentLookup = state.GetComponentLookup<Parent>(true);
        m_PlatformLookup = state.GetComponentLookup<Platform>(true);
        m_StairsLookup = state.GetComponentLookup<Stair>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_StationPlatformLookup.Update(ref state);
        m_ParentLookup.Update(ref state);
        m_PlatformLookup.Update(ref state);
        m_StairsLookup.Update(ref state);

        var updateCommuterStateJob = new UpdateCommuterStateJob()
        {
            StationPlatformLookup = m_StationPlatformLookup,
            ParentLookup = m_ParentLookup,
            PlatformLookup = m_PlatformLookup,
            StairsLookup = m_StairsLookup,
            DeltaTime = SystemAPI.Time.DeltaTime
        };
        state.Dependency = updateCommuterStateJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct UpdateCommuterStateJob : IJobEntity
{
    [ReadOnly] public BufferLookup<StationPlatform> StationPlatformLookup;
    [ReadOnly] public ComponentLookup<Parent> ParentLookup;
    [ReadOnly] public ComponentLookup<Platform> PlatformLookup;
    [ReadOnly] public ComponentLookup<Stair> StairsLookup;

    [ReadOnly] public float DeltaTime;

    [BurstCompile]
    public void Execute(ref Commuter commuter, ref LocalTransform commuterTransform, ref TargetDestination targetDestination,
        ref SaschaMovementQueue movementQueue)
    {
        switch (commuter.State)
        {
            case CommuterState.Idle:
                // TODO: idle should mean commuter is standing on platform but not in a queue. Could join a queue on local platform or switch platform
                PickNextPlatformDestination(ref commuter, ref movementQueue);
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
    private void PickNextPlatformDestination(ref Commuter commuter, ref SaschaMovementQueue movementQueue)
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
        PlatformLookup.TryGetComponent(commuter.CurrentPlatform, out var currentPlatform);
        StairsLookup.TryGetComponent(currentPlatform.Stairs, out var currentPlatformStairs);
        var destinationPlatformEntity = stationPlatforms[randomPlatformIndex].Platform;
        PlatformLookup.TryGetComponent(destinationPlatformEntity, out var destinationPlatform);
        StairsLookup.TryGetComponent(destinationPlatform.Stairs, out var destinationPlatformStairs);

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