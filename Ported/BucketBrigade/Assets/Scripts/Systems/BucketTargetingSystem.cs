using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct BucketTargetingSystem : ISystem
{
    ComponentLookup<Position> m_PositionLookup;
    ComponentLookup<BucketTargetPosition> m_TargetLookup;
    EntityQuery m_OmniWorkerQuery;
    EntityQuery m_BucketQuery;
    
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_PositionLookup = state.GetComponentLookup<Position>();
        m_TargetLookup = state.GetComponentLookup<BucketTargetPosition>();
        // todo replace managed arrays in queries to allow burst compile
        m_OmniWorkerQuery = state.GetEntityQuery(ComponentType.Exclude<TeamInfo>(), ComponentType.ReadWrite<BucketTargetPosition>(), ComponentType.ReadOnly<Position>());
        m_BucketQuery = state.GetEntityQuery(ComponentType.ReadOnly<BucketTag>(), ComponentType.ReadOnly<WaterAmount>(), ComponentType.ReadOnly<Position>(), ComponentType.Exclude<TargetedTag>(), ComponentType.Exclude<PickedUpTag>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_PositionLookup.Update(ref state);
        m_TargetLookup.Update(ref state);
        foreach (var worker in m_OmniWorkerQuery.ToEntityArray(Allocator.Temp))
        {
            var workerPosition = m_PositionLookup.GetRefRO(worker).ValueRO.position;
            var minDist = math.INFINITY;
            var bucketTargetPosition = workerPosition;
            var bucketTarget = Entity.Null;
            foreach (var bucket in m_BucketQuery.ToEntityArray(Allocator.Temp))
            {
                var bucketPosition = m_PositionLookup.GetRefRO(bucket).ValueRO;
                var dist = math.distance(workerPosition, bucketPosition.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    bucketTargetPosition = bucketPosition.position;
                    bucketTarget = bucket;
                }
            }

            if (bucketTarget != Entity.Null)
            {
                m_TargetLookup.GetRefRW(worker, false).ValueRW.position = bucketTargetPosition;
                state.EntityManager.SetComponentEnabled<TargetedTag>(bucketTarget, true);
            }
        }
    }
    
}
