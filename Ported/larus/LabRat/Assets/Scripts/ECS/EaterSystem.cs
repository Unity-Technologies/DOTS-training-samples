using ECSExamples;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct LastPositionComponent : IComponentData
{
    public int Index;
}

public class EaterSystem : ComponentSystem
{
    private BoardSystem m_Board;

    protected override void OnCreate()
    {
        m_Board = World.GetExistingSystem<BoardSystem>();
    }

    protected override void OnUpdate()
    {
        var board = GetSingleton<BoardDataComponent>();
        var catMap = m_Board.CatMap;
        var cellMap = m_Board.CellMap;
        var homebaseMap = m_Board.HomeBaseMap;

        Entities.ForEach((Entity entity, ref EaterComponentTag eater, ref Translation position, ref LastPositionComponent lastPosition) =>
        {
            var localPt = new float2(position.Value.x, position.Value.z);
            localPt += board.cellSize * 0.5f; // offset by half cellsize
            var cellCoord = new float2(Mathf.FloorToInt(localPt.x / board.cellSize.x), Mathf.FloorToInt(localPt.y / board.cellSize.y));
            var cellIndex = (int)(cellCoord.y * board.size.x + cellCoord.x);

            var lastIndex = lastPosition.Index;
            if (lastIndex == cellIndex)
                return;

            CellComponent cell = new CellComponent();
            if (catMap.ContainsKey(lastIndex))
            {
                catMap[lastIndex]--;
                if (catMap[lastIndex] == 0)
                {
                    cell = cellMap[lastIndex];
                    cell.data &= ~CellData.Eater;
                    cellMap[lastIndex] = cell;
                    catMap.Remove(lastIndex);
                }
            }

            if (catMap.ContainsKey(cellIndex))
            {
                catMap[cellIndex] = catMap[cellIndex] + 1;
            }
            else
                catMap.Add(cellIndex, 1);
            if (!cellMap.ContainsKey(cellIndex))
                cellMap.Add(cellIndex, new CellComponent());
            cell = cellMap[cellIndex];
            cell.data |= CellData.Eater;
            cellMap[cellIndex] = cell;

            lastPosition.Index = cellIndex;

            if ((cell.data & CellData.HomeBase) == CellData.HomeBase)
            {
                var playerId = homebaseMap[cellIndex];
                PostUpdateCommands.DestroyEntity(entity);
                var playerData = PlayerCursor.GetPlayerData(playerId);
                playerData.mouseCount = (int)(playerData.mouseCount * 0.6666f);
            }
        });
    }
}
