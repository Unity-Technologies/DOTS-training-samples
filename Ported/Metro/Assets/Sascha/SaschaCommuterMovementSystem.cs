using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))] // Make sure WorldTransforms are already updated this frame.
partial struct SaschaCommuterMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (commuter, commuterTransform, targetDestination, movementQueue) in SystemAPI.Query<RefRW<Commuter>, RefRW<LocalTransform>, RefRW<TargetDestination>, RefRW<SaschaMovementQueue>>())
        {
            switch (commuter.ValueRW.State)
            {
                case CommuterState.Idle:
                    PickNextPlatformDestination(ref state, commuter, movementQueue);
                    break;
                case CommuterState.MoveToDestination:
                    UpdateMoveToDestination(ref state, commuter, commuterTransform, targetDestination, movementQueue);
                    break;
            }
        }
    }

    [BurstCompile]
    private void UpdateMoveToDestination(ref SystemState state, RefRW<Commuter> commuter, RefRW<LocalTransform> commuterTransform, RefRW<TargetDestination> targetDestination, RefRW<SaschaMovementQueue> movementQueue)
    {
        float3 currentDestination;
        if (targetDestination.ValueRO.IsActive)
        {
            currentDestination = targetDestination.ValueRO.TargetPosition;
        }
        else
        {
            if (movementQueue.ValueRO.QueuedInstructions.IsCreated == false ||
                movementQueue.ValueRW.QueuedInstructions.TryDequeue(out var nextInstruction) == false)
            {
                commuter.ValueRW.State = CommuterState.Idle;
                return;
            }

            currentDestination = nextInstruction.Destination;
            commuter.ValueRW.CurrentPlatform = nextInstruction.Platform;
            targetDestination.ValueRW.TargetPosition = nextInstruction.Destination;
            targetDestination.ValueRW.IsActive = true;
        }

        float movementMagnitude = commuter.ValueRO.Velocity * SystemAPI.Time.DeltaTime;
        float movementMagnitudeSquared = movementMagnitude * movementMagnitude;
        var remainingPathVector = currentDestination - commuterTransform.ValueRO.Position;
        float remainingPathLengthSquared = math.lengthsq(remainingPathVector);
        if (remainingPathLengthSquared < movementMagnitudeSquared)
        {
            commuterTransform.ValueRW.Position = currentDestination;
            targetDestination.ValueRW.IsActive = false;
            // TODO: the remaining distance that can be traveled during this tick is not applied towards next waypoint
        }
        else
        {
            var directionalVector = math.normalize(remainingPathVector);
            commuterTransform.ValueRW.Position += (directionalVector * movementMagnitude);
        }

    }

    [BurstCompile]
    private void PickNextPlatformDestination(ref SystemState state, RefRW<Commuter> commuter, RefRW<SaschaMovementQueue> movementQueue)
    {
        if (commuter.ValueRO.CurrentPlatform == Entity.Null || state.EntityManager.Exists(commuter.ValueRO.CurrentPlatform) == false)
        {
            Debug.LogError("Commuter doesn't have a current platform");
            return;
        }
        
        var parent = state.EntityManager.GetComponentData<Parent>(commuter.ValueRO.CurrentPlatform);
        var stationPlatforms = state.EntityManager.GetBuffer<StationPlatform>(parent.Value);
        var randomPlatformIndex = commuter.ValueRW.Random.NextInt(0, stationPlatforms.Length - 1); // max is excluded, but use length - 1 as one platform in the list is the same as the current

        for (int i = 0; i <= randomPlatformIndex; ++i)
        {
            if (stationPlatforms[i].Platform == commuter.ValueRO.CurrentPlatform)
            {
                ++randomPlatformIndex;
                break;
            }
        }

        var currentPlatform = state.EntityManager.GetComponentData<Platform>(commuter.ValueRO.CurrentPlatform);
        var currentPlatformStairs = state.EntityManager.GetComponentData<Stair>(currentPlatform.Stairs);
        var destinationPlatformEntity = stationPlatforms[randomPlatformIndex].Platform;
        var destinationPlatform = state.EntityManager.GetComponentData<Platform>(destinationPlatformEntity);
        var destinationPlatformStairs = state.EntityManager.GetComponentData<Stair>(destinationPlatform.Stairs);

        if (movementQueue.ValueRW.QueuedInstructions.IsCreated)
        {
            movementQueue.ValueRW.QueuedInstructions.Clear();
        }
        else
        {
            movementQueue.ValueRW.QueuedInstructions = new NativeQueue<SaschaMovementQueueInstruction>(Allocator.Persistent);
        }

        movementQueue.ValueRW.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = state.EntityManager.GetComponentData<WorldTransform>(currentPlatformStairs.BottomWaypoint).Position,
            Platform = commuter.ValueRO.CurrentPlatform
        });
        
        movementQueue.ValueRW.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = state.EntityManager.GetComponentData<WorldTransform>(currentPlatformStairs.TopWaypoint).Position,
            Platform = Entity.Null
        });
        
        movementQueue.ValueRW.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = state.EntityManager.GetComponentData<WorldTransform>(currentPlatformStairs.TopWalkwayWaypoint).Position,
            Platform = Entity.Null
        });
        
        movementQueue.ValueRW.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = state.EntityManager.GetComponentData<WorldTransform>(destinationPlatformStairs.TopWalkwayWaypoint).Position,
            Platform = Entity.Null
        });
        
        movementQueue.ValueRW.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = state.EntityManager.GetComponentData<WorldTransform>(destinationPlatformStairs.TopWaypoint).Position,
            Platform = Entity.Null
        });
        
        movementQueue.ValueRW.QueuedInstructions.Enqueue(new SaschaMovementQueueInstruction()
        {
            Destination = state.EntityManager.GetComponentData<WorldTransform>(destinationPlatformStairs.BottomWaypoint).Position,
            Platform = destinationPlatformEntity
        });
        
        commuter.ValueRW.State = CommuterState.MoveToDestination;

    }
}