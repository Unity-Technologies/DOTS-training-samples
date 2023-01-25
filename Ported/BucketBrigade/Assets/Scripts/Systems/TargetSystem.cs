using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct TargetSystem : ISystem
{
    ComponentLookup<Position> m_PositionLookup;
    ComponentLookup<Target> m_TargetLookup;
    EntityQuery m_OmniWorkerQuery;
    EntityQuery m_FlameCellQuery;
    EntityQuery m_WaterCellQuery;
    
    
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_PositionLookup = state.GetComponentLookup<Position>();
        m_TargetLookup = state.GetComponentLookup<Target>();
        // todo replace managed arrays in queries to allow burst compile
        m_OmniWorkerQuery = state.GetEntityQuery(ComponentType.Exclude<TeamInfo>(), ComponentType.ReadWrite<Target>(), ComponentType.ReadOnly<Position>());
        m_FlameCellQuery = state.GetEntityQuery(ComponentType.ReadOnly<OnFireTag>(), ComponentType.ReadOnly<Position>());
        m_WaterCellQuery = state.GetEntityQuery(ComponentType.ReadOnly<WaterAmount>(), ComponentType.ReadOnly<Position>());
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
            var flameTarget = workerPosition;
            var waterTarget = workerPosition;
            foreach (var flameCellPosition in m_FlameCellQuery.ToComponentDataArray<Position>(Allocator.Temp))
            {
                var dist = math.distance(workerPosition, flameCellPosition.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    flameTarget = flameCellPosition.position;
                }
            }
            minDist = math.INFINITY;
            foreach (var waterCellPosition in m_WaterCellQuery.ToComponentDataArray<Position>(Allocator.Temp))
            {
                var dist = math.distance(workerPosition, waterCellPosition.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    waterTarget = waterCellPosition.position;
                }
            }

            m_TargetLookup.GetRefRW(worker, false).ValueRW.waterCellPosition = waterTarget;
            m_TargetLookup.GetRefRW(worker, false).ValueRW.flameCellPosition = flameTarget;
        }
    }
}
