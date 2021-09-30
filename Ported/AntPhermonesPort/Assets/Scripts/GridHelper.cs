using Unity.Mathematics;
using UnityEngine;

public struct GridHelper
{
    public float2 worldLowerLeft;
    public float2 worldUpperRight;

    public int gridDimLength;
    public float worldDimLength;

    public GridHelper(int _gridDimLength, float _worldDimLength)
    {
        gridDimLength = _gridDimLength;
        worldDimLength = _worldDimLength;

        // World centered at 0,0
        var halfSize = _worldDimLength / 2;

        float2 cellCenterOffset = new float2((_worldDimLength / _gridDimLength), (_worldDimLength / _gridDimLength)) / 2;

        worldLowerLeft = new float2(-halfSize, -halfSize) + cellCenterOffset;
        worldUpperRight = new float2(halfSize, halfSize) + cellCenterOffset;
    }

    // float x, y
    public void WorldToCellSpace(ref float x, ref float y)
    {
        x = (int)((Mathf.Clamp(x, -worldDimLength, worldDimLength) + worldDimLength / 2) * gridDimLength / worldDimLength);
        y = (int)((Mathf.Clamp(y, -worldDimLength, worldDimLength) + worldDimLength / 2) * gridDimLength / worldDimLength);
    }

    public void CellToWorldSpace(ref float x, ref float y)
    {
        x = (x - worldDimLength * 2) / gridDimLength * worldDimLength;
        y = (y - worldDimLength * 2) / gridDimLength * worldDimLength;
    }

    public int GetIndex1D(float2 index)
        => Mathf.RoundToInt(index.y) * gridDimLength + Mathf.RoundToInt(index.x);

    public float2 Index1DToWorldSpace(int index)
    {
        float cellWidth = worldDimLength / gridDimLength;

        int x = index % gridDimLength;
        int y = index / gridDimLength;

        float2 gridPos = new float2(x * cellWidth, y * cellWidth);
        return gridPos + worldLowerLeft;
    }

    /// <summary>
    /// Returns the nearest index to a point in world space. The originOffset is used
    /// to align the [0,0] index with the start of the grid (since the grid origin in
    /// worldspace might be [-5.0, -5.0] for example if the plane of size 5 is at [0,0])
    /// </summary>
    /// <param name="xy"></param>
    /// <param name="originOffset"></param>
    /// <returns></returns>
    public int GetNearestIndex(float2 xy)
    {
        if (xy.x < worldLowerLeft.x || xy.y < worldLowerLeft.y || xy.x > worldUpperRight.x || xy.y > worldUpperRight.y)
        {
            //TBD: Warnings are disabled currently because ant move might jump out past the boundary cell
            //Debug.LogError("[Cell Map] Trying to get index out of range");
            return -1;
        }

        float2 gridRelativeXY = xy - worldLowerLeft;
        float2 gridIndexScaleFactor = (worldUpperRight - worldLowerLeft) / gridDimLength;

        float2 gridIndexXY = gridRelativeXY / gridIndexScaleFactor;

        return GetIndex1D(gridIndexXY);
    }

    // DEBUGGING
    public void DrawDebugRay(int index, Color color, float time)
    {
        float2 worldPos = Index1DToWorldSpace(index);
        Debug.DrawRay(new Vector3(worldPos.x, 0, worldPos.y), Vector3.up, color, time);
    }

}
