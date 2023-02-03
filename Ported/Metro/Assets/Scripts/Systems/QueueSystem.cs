using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateAfter(typeof(CommuterStateSystem))]
public partial struct QueueSystem : ISystem
{
    private ComponentLookup<WorldTransform> m_WorldTransformLookupRO;
    private ComponentLookup<Queue> m_QueueLookupRO;

    Random m_Random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_Random = Random.CreateFromIndex(1234);

        m_WorldTransformLookupRO = state.GetComponentLookup<WorldTransform>(true);
        m_QueueLookupRO = state.GetComponentLookup<Queue>(true);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_WorldTransformLookupRO.Update(ref state);
        m_QueueLookupRO.Update(ref state);

        NativeList<Entity> queuesToUpdate = new(Allocator.TempJob);

        var updateCommuterDestinationJob = new UpdateCommuterDestination()
        {
            WorldTransformLookupRO = m_WorldTransformLookupRO,
            QueueLookupRO = m_QueueLookupRO
        };
        updateCommuterDestinationJob.ScheduleParallel(state.Dependency).Complete();

        foreach (var (queueingData, targetDestination, movementQueue, seatReservation, destinationAspect) in
                 SystemAPI.Query<RefRW<QueueingData>, RefRW<TargetDestination>, RefRW<SaschaMovementQueue>, RefRW<SeatReservation>, DestinationAspect>())
        {
            if (queueingData.ValueRO.TargetQueue != Entity.Null)
            {
                var queueState = SystemAPI.GetComponent<QueueState>(queueingData.ValueRO.TargetQueue);

                if (queueState.IsOpen && queueingData.ValueRO.PositionInQueue == 0 && destinationAspect.IsAtDestination())
                {
                    Entity availableSeatEntity = FindAvailableSeat(ref state, queueState.FacingCarriage);
                    if (availableSeatEntity != Entity.Null)
                    {
                        var carriageTransform = SystemAPI.GetComponent<WorldTransform>(queueState.FacingCarriage);
                        var seatTransform = SystemAPI.GetComponent<WorldTransform>(availableSeatEntity);
                        targetDestination.ValueRW.IsActive = false;

                        if (movementQueue.ValueRW.QueuedInstructions.IsCreated)
                        {
                            movementQueue.ValueRW.QueuedInstructions.Clear();
                        }
                        else
                        {
                            movementQueue.ValueRW.QueuedInstructions = new NativeQueue<SaschaMovementQueueInstruction>(Allocator.Persistent);
                        }

                        movementQueue.ValueRW.QueuedInstructions.Enqueue(new() { Destination = carriageTransform.Position });
                        movementQueue.ValueRW.QueuedInstructions.Enqueue(new() { Destination = seatTransform.Position });
                        
                        seatReservation.ValueRW.TargetSeat = availableSeatEntity;
                        seatReservation.ValueRW.TargetCarriage = queueState.FacingCarriage;

                        SystemAPI.SetComponent(availableSeatEntity, new Seat(){ IsTaken = true });

                        queuesToUpdate.Add(queueingData.ValueRO.TargetQueue);
                        queueingData.ValueRW.TargetQueue = Entity.Null;
                    }
                }
            }
        }

        var updateQueueSize = new UpdateQueueSize()
        {
            QueuesToUpdate = queuesToUpdate
        };
        state.Dependency = updateQueueSize.ScheduleParallel(state.Dependency);

        var updateCommuterQueuePosition = new UpdateCommuterQueuePosition()
        {
            QueuesToUpdate = queuesToUpdate
        };
        state.Dependency = updateCommuterQueuePosition.ScheduleParallel(state.Dependency);

    }

    [BurstCompile]
    private Entity FindAvailableSeat(ref SystemState state, Entity carriage)
    {
        var seats = SystemAPI.GetBuffer<CarriageSeat>(carriage);
        foreach (var carriageSeat in seats)
        {
            var seat = SystemAPI.GetComponent<Seat>(carriageSeat.Seat);
            if (!seat.IsTaken)
            {
                return carriageSeat.Seat;
            }
        }
        return Entity.Null;
    }
}

[BurstCompile]
public partial struct UpdateCommuterDestination : IJobEntity
{
    [ReadOnly] public ComponentLookup<WorldTransform> WorldTransformLookupRO;
    [ReadOnly] public ComponentLookup<Queue> QueueLookupRO;

    [BurstCompile]
    public void Execute(ref QueueingData queueingData, ref TargetDestination targetDestination)
    {
        if (queueingData.TargetQueue != Entity.Null)
        {
            var queue = QueueLookupRO.GetRefRO(queueingData.TargetQueue);
            var queueTransform = WorldTransformLookupRO.GetRefRO(queueingData.TargetQueue);

            targetDestination.IsActive = true;
            int maxSingleQueueLength = queue.ValueRO.QueueCapacity;
            int queuePositionInDirection = queueingData.PositionInQueue % maxSingleQueueLength;
            int queuePositionOrthogonalDirection = queueingData.PositionInQueue / maxSingleQueueLength;
            if ((queuePositionOrthogonalDirection & 1) != 0)
            {
                queuePositionInDirection = maxSingleQueueLength - queuePositionInDirection - 1;
            }

            targetDestination.TargetPosition = queueTransform.ValueRO.Position + (queuePositionInDirection * queue.ValueRO.QueueDirection) + (queuePositionOrthogonalDirection * queue.ValueRO.QueueDirectionOrthogonal);
        }
    }
}

[BurstCompile]
public partial struct UpdateQueueSize : IJobEntity
{
    [ReadOnly] public NativeList<Entity> QueuesToUpdate;

    [BurstCompile]
    public void Execute(ref QueueState queue, Entity queueEntity)
    {
        if (QueuesToUpdate.Contains(queueEntity))
        {
            --queue.QueueSize;
        }
    }
}

[BurstCompile]
public partial struct UpdateCommuterQueuePosition : IJobEntity
{
    [ReadOnly] public NativeList<Entity> QueuesToUpdate;

    [BurstCompile]
    public void Execute(ref QueueingData queueingData)
    {
        if (QueuesToUpdate.Contains(queueingData.TargetQueue))
        {
            --queueingData.PositionInQueue;
        }
    }
}
