using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GameSceneDataVisualizer : MonoBehaviour
{
    [Header("References")]
    public GameGlobalDataAuthoring GlobalData;
    public GameSceneDataAuthoring SceneData;

    private void OnDrawGizmos()
    {
        var gridCharacteristics = new GridCharacteristics(
            float3.zero, 
            SceneData.FloorSizeX,
            SceneData.TeamFloorSizeX, 
            SceneData.FloorSizeZ, 
            GlobalData.GridCellSize,
            SceneData.TeamZoneHeight);

        for (int i = 0; i < gridCharacteristics.GetTotalCellCount(); i++)
        {
            int2 cellCoords = gridCharacteristics.GetCoordinatesOfCellIndex(i);
            CellType cellType = gridCharacteristics.GetTypeOfCell(cellCoords);
            
            Color cellColor = default;
            switch (cellType)
            {
                case CellType.Floor:
                    cellColor = GlobalData.FloorColor;
                    break;
                case CellType.TeamA:
                    cellColor = GlobalData.TeamAColor;
                    break;
                case CellType.TeamB:
                    cellColor = GlobalData.TeamBColor;
                    break;
            }

            Gizmos.color = cellColor;
            Gizmos.DrawCube(gridCharacteristics.GetPositionOfCell(cellCoords), Vector3.one * GlobalData.GridCellSize);
        }
        
        // ResourcesSpawnBox
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(SceneData.ResourcesSpawnBox.Center, SceneData.ResourcesSpawnBox.Extents * 2f);
        
        // Bounds
        {
            Gizmos.color = GlobalData.TeamAColor;
            Box teamABox = gridCharacteristics.TeamABounds;
            Gizmos.DrawWireCube(teamABox.Center, teamABox.Extents * 2f);

            Gizmos.color = GlobalData.TeamBColor;
            Box teamBBox = gridCharacteristics.TeamBBounds;
            Gizmos.DrawWireCube(teamBBox.Center, teamBBox.Extents * 2f);
            
            Gizmos.color = GlobalData.FloorColor;
            Box levelBox = gridCharacteristics.LevelBounds;
            Gizmos.DrawWireCube(levelBox.Center, levelBox.Extents * 2f);
        }
    }
}
