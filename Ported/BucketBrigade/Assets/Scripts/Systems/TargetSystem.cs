using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TargetSystemData : IComponentData
{
    public double NextUpdateTime;
}

[UpdateAfter(typeof(FireSimSystem))]
[BurstCompile]
partial struct TargetSystem : ISystem
{
    ComponentLookup<Position> m_PositionLookup;
    ComponentLookup<Target> m_TargetLookup;
    EntityQuery m_OmniWorkerQuery;
    EntityQuery m_FlameCellQuery;
    EntityQuery m_WaterCellQuery;

    const double k_TimeBetweenUpdates = 5;
    

    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_PositionLookup = state.GetComponentLookup<Position>();
        m_TargetLookup = state.GetComponentLookup<Target>();
        // todo replace managed arrays in queries to allow burst compile
        m_OmniWorkerQuery = state.GetEntityQuery(ComponentType.Exclude<TeamInfo>(), ComponentType.ReadWrite<Target>(), ComponentType.ReadOnly<Position>());
        m_FlameCellQuery = state.GetEntityQuery(ComponentType.ReadOnly<OnFireTag>(), ComponentType.ReadOnly<Position>());
        m_WaterCellQuery = state.GetEntityQuery(ComponentType.Exclude<BucketTag>(), ComponentType.ReadOnly<WaterAmount>(), ComponentType.ReadOnly<Position>());
        state.EntityManager.AddComponent<TargetSystemData>(state.SystemHandle);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var currentTime = SystemAPI.Time.ElapsedTime;
        if (currentTime < SystemAPI.GetComponent<TargetSystemData>(state.SystemHandle).NextUpdateTime) return;
        SystemAPI.SetComponent(state.SystemHandle, new TargetSystemData() { NextUpdateTime = currentTime + k_TimeBetweenUpdates});
        
        m_PositionLookup.Update(ref state);
        m_TargetLookup.Update(ref state);
        foreach (var worker in m_OmniWorkerQuery.ToEntityArray(Allocator.Temp))
        {
            var workerPosition = m_PositionLookup.GetRefRO(worker).ValueRO.position;
            var minDist = math.INFINITY;
            var flameTarget = workerPosition;
            var waterTarget = workerPosition;
            Entity fireTargetEntity = new();
            int2 flameTargetIndex = new();
            foreach (var flameEntity in m_FlameCellQuery.ToEntityArray(Allocator.Temp))
            {
                var flameCellPosition = SystemAPI.GetComponent<Position>(flameEntity);
                var dist = math.distance(workerPosition, flameCellPosition.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    flameTarget = flameCellPosition.position;
                    var cellInfo = SystemAPI.GetComponent<CellInfo>(flameEntity);
                    fireTargetEntity = flameEntity;
                    flameTargetIndex = new int2(cellInfo.indexX,cellInfo.indexY);
                }
            }
            m_TargetLookup.GetRefRW(worker, false).ValueRW.fireTargetEntity = fireTargetEntity;

            minDist = math.INFINITY;
            Entity waterTargetEntity = new();
            foreach (var waterEntity in m_WaterCellQuery.ToEntityArray(Allocator.Temp))
            {
                var waterCellPosition = SystemAPI.GetComponent<Position>(waterEntity);
                var dist = math.distance(workerPosition, waterCellPosition.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    waterTarget = waterCellPosition.position;
                    waterTargetEntity = waterEntity;
                }
            }
            m_TargetLookup.GetRefRW(worker, false).ValueRW.waterTargetEntity = waterTargetEntity;


            m_TargetLookup.GetRefRW(worker, false).ValueRW.waterCellPosition = waterTarget;
            m_TargetLookup.GetRefRW(worker, false).ValueRW.flameCellPosition = flameTarget;
            m_TargetLookup.GetRefRW(worker, false).ValueRW.targetIndex = flameTargetIndex;
        }
    }
}
