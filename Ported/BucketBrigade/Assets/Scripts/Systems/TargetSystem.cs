using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct TargetSystem : ISystem
{
    ComponentLookup<Position> m_PositionLookup;
    ComponentLookup<Target> m_TargetLookup;
    EntityQuery m_OmniWorkerQuery;
    EntityQuery m_FlameCellQuery;
    EntityQuery m_WaterCellQuery;
    
    
    [BurstCompile]
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
        foreach (var worker in m_OmniWorkerQuery.ToEntityArray(Allocator.Temp))
        {
            var minDist = math.INFINITY;
            var flameTarget = Entity.Null;
            var waterTarget = Entity.Null;
            foreach (var flameCell in m_FlameCellQuery.ToEntityArray(Allocator.Temp))
            {
                var dist = math.distance(m_PositionLookup.GetRefRO(worker).ValueRO.position, m_PositionLookup.GetRefRO(flameCell).ValueRO.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    flameTarget = flameCell;
                }
            }
            minDist = math.INFINITY;
            foreach (var waterCell in m_WaterCellQuery.ToEntityArray(Allocator.Temp))
            {
                var dist = math.distance(m_PositionLookup.GetRefRO(worker).ValueRO.position, m_PositionLookup.GetRefRO(waterCell).ValueRO.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    waterTarget = waterCell;
                }
            }

            var target = m_TargetLookup.GetRefRW(worker, false).ValueRW;
            target.waterCellPosition = m_PositionLookup.GetRefRO(waterTarget).ValueRO.position;
            target.flameCellPosition = m_PositionLookup.GetRefRO(flameTarget).ValueRO.position;
        }
    }
}
