using ECSExamples;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class EatenSystem : JobComponentSystem
{
    private BoardSystem m_Board;
    private EndSimulationEntityCommandBufferSystem m_Buffer;

    protected override void OnCreate()
    {
        m_Board = World.GetExistingSystem<BoardSystem>();
        m_Buffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var board = GetSingleton<BoardDataComponent>();
        var cellMap = m_Board.CellMap;
        var homebaseMap = m_Board.HomeBaseMap;
        var ecb = m_Buffer.CreateCommandBuffer().ToConcurrent();

        var job = Entities.WithAll<EatenComponentTag>().ForEach((Entity entity, int entityInQueryIndex, in Translation position) =>
        {
            var localPt = new float2(position.Value.x, position.Value.z);
            localPt += board.cellSize * 0.5f; // offset by half cellsize
            var cellCoord = new float2(math.floor(localPt.x / board.cellSize.x),
                math.floor(localPt.y / board.cellSize.y));
            var cellIndex = (int) (cellCoord.y * board.size.x + cellCoord.x);
            CellComponent cell = new CellComponent();
            if (!cellMap.TryGetValue(cellIndex, out cell))
                return;

            if ((cell.data & CellData.HomeBase) == CellData.HomeBase)
            {
                var playerId = homebaseMap[cellIndex];
                ecb.DestroyEntity(entityInQueryIndex, entity);
                PlayerCursor.GetPlayerData(playerId).mouseCount++;
            }

            if ((cell.data & CellData.Eater) == CellData.Eater)
            {
                ecb.DestroyEntity(entityInQueryIndex, entity);
            }
        }).WithReadOnly(cellMap).WithReadOnly(homebaseMap).Schedule(inputDeps);
        return job;
    }
}