using ECSExamples;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class EatenSystem : ComponentSystem
{
    private BoardSystem m_Board;

    protected override void OnCreate()
    {
        m_Board = World.GetExistingSystem<BoardSystem>();
    }

    protected override void OnUpdate()
    {
        var board = GetSingleton<BoardDataComponent>();
        var cellMap = m_Board.CellMap;
        var homebaseMap = m_Board.HomeBaseMap;
        
        Entities.ForEach((Entity entity, ref Translation position, ref EatenComponentTag eaten) =>
        {
            var localPt = new float2(position.Value.x, position.Value.z);
            localPt += board.cellSize * 0.5f; // offset by half cellsize
            var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
            var cellIndex = (int)(cellCoord.y * board.size.x + cellCoord.x);
            CellComponent cell = new CellComponent();
            if (!cellMap.TryGetValue(cellIndex, out cell))
                return;

            if ((cell.data & CellData.HomeBase) == CellData.HomeBase)
            {
                var playerId = homebaseMap[cellIndex];
                PostUpdateCommands.DestroyEntity(entity);
                PlayerCursor.GetPlayerData(playerId).mouseCount++;
            }

            if ((cell.data & CellData.Eater) == CellData.Eater)
            {
                PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}
