using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using UnityCamera = UnityEngine.Camera;
using UnityGameObject = UnityEngine.GameObject;
using UnityInput = UnityEngine.Input;
using UnityKeyCode = UnityEngine.KeyCode;
using UnityMeshRenderer = UnityEngine.MeshRenderer;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

public static class Utils
{
    public static int WorldPositionToCellIndex(Vector3 worldPosition, GameConfig gameConfig)
    {
        var localPt3D = new float3(worldPosition.x, worldPosition.y, worldPosition.z);
        var localPt = new Vector2(localPt3D.x, localPt3D.z);

        localPt += new Vector2(0.5f, 0.5f); // offset by half cellsize
        var cellCoord = new Vector2Int(Mathf.FloorToInt(localPt.x / 1), Mathf.FloorToInt(localPt.y / 1));
        return Utils.CellCoordinatesToCellIndex(gameConfig, cellCoord.x, cellCoord.y);
    }
    
    public static int CellCoordinatesToCellIndex(GameConfig c, int x, int y)
    {
        if (x >= c.BoardDimensions.x || y >= c.BoardDimensions.y || x < 0 || y < 0)
            return -1;

        return (y * c.BoardDimensions.x) + x;
    }

    public static float3 CellCoordinatesToWorldPosition(int x, int z)
    {
        return new float3(x, -0.5f, z);
    }
}