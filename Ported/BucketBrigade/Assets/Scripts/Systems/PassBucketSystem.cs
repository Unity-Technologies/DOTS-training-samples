using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct PassBucketSystem : ISystem
{
    ComponentLookup<Position> m_PositionLookup;
    ComponentLookup<CarriedBucket> m_CarriedBucketLookup;
    EntityQuery m_WorkerDroppingQuery;
    EntityQuery m_WorkerPickingUpQuery;
    EntityQuery m_AvailableBucketQuery;

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_PositionLookup = state.GetComponentLookup<Position>();
        m_CarriedBucketLookup = state.GetComponentLookup<CarriedBucket>();
        // todo replace managed arrays in queries to allow burst compile
        m_WorkerDroppingQuery = state.GetEntityQuery(ComponentType.ReadOnly<CarriesBucketTag>(), ComponentType.ReadOnly<HasReachedDestinationTag>(), ComponentType.ReadOnly<Position>());
        m_WorkerPickingUpQuery = state.GetEntityQuery(ComponentType.Exclude<CarriesBucketTag>(), ComponentType.ReadOnly<HasReachedDestinationTag>(), ComponentType.ReadOnly<Position>());
        m_AvailableBucketQuery = state.GetEntityQuery(ComponentType.Exclude<PickedUpTag>(), ComponentType.ReadOnly<Position>(), ComponentType.ReadOnly<BucketTag>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_PositionLookup.Update(ref state);
        m_CarriedBucketLookup.Update(ref state);
        
        // todo define update frequency
        foreach (var worker in m_WorkerDroppingQuery.ToEntityArray(Allocator.Temp))
        {
            state.EntityManager.SetComponentEnabled<CarriesBucketTag>(worker, false);
            state.EntityManager.SetComponentEnabled<HasReachedDestinationTag>(worker, false);
            var carriedBucket = m_CarriedBucketLookup.GetRefRW(worker, false).ValueRW;
            state.EntityManager.SetComponentEnabled<PickedUpTag>(carriedBucket.bucket, false);
            carriedBucket.bucket = Entity.Null;
        }
        
        foreach (var worker in m_WorkerPickingUpQuery.ToEntityArray(Allocator.Temp))
        {
            foreach (var bucket in m_AvailableBucketQuery.ToEntityArray(Allocator.Temp))
            {
                if (math.distance(m_PositionLookup.GetRefRO(worker).ValueRO.position, m_PositionLookup.GetRefRO(bucket).ValueRO.position) < 1)
                {
                    state.EntityManager.SetComponentEnabled<CarriesBucketTag>(worker, true);
                    state.EntityManager.SetComponentEnabled<HasReachedDestinationTag>(worker, false);
                    state.EntityManager.SetComponentEnabled<PickedUpTag>(bucket, true);
                    m_CarriedBucketLookup.GetRefRW(worker, false).ValueRW.bucket = bucket;
                }
            }
        }

    }
}