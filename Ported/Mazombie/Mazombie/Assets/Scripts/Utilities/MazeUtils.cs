using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

public struct GridCellWorldBounds
{
    public float3 Left;
    public float3 Top;
    public float3 Right;
    public float3 Bottom;
}

public struct MazeUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float3 GridPositionToWorld(int x, int y)
    {
        return new (x + 0.5f, 0, y + 0.5f);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 WorldPositionToGrid(float3 worldPos)
    {
        if (worldPos.x < 0) worldPos.x--;
        if (worldPos.z < 0) worldPos.z--;
        
        return new ((int)worldPos.x, (int)worldPos.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GridCellWorldBounds GetGridCellWorldBounds(int x, int y)
    {
        var pos = GridPositionToWorld(x, y);
        return new GridCellWorldBounds
        {
            Left = pos + math.left() * 0.5f,
            Top = pos + math.forward() * 0.5f,
            Right = pos + math.right() * 0.5f,
            Bottom = pos + math.back() * 0.5f
        };
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CellIdxFromPos(int2 gridPos, int gridSize)
    {
        return gridPos.x + gridPos.y * gridSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CellIdxFromPos(int x, int y, int gridSize)
    {
        return x + y * gridSize;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float3 PositionFromIndex(int index, int gridSize)
    {
        return GridPositionToWorld(index % gridSize, index / gridSize);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int2 CellFromIndex(int index, int gridSize)
    {
        return new int2(index % gridSize, index / gridSize);
    }

    public static bool HasFlag(WallFlags flags, WallFlags flag)
    {
        return (flags & flag) != 0;
    }

    public static void DrawGridCell(int2 gridPos, byte wallFlags = 0)
    {
        var bounds = GetGridCellWorldBounds(gridPos.x, gridPos.y);

        var points = new NativeArray<float3>(4, Allocator.Temp);
        points[0] = new float3(bounds.Left.x, -0.4f, bounds.Top.z);
        points[1] = new float3(bounds.Right.x, -0.4f, bounds.Top.z);
        points[2] = new float3(bounds.Right.x, -0.4f, bounds.Bottom.z);
        points[3] = new float3(bounds.Left.x, -0.4f, bounds.Bottom.z);

        var indices = new NativeArray<int>(8, Allocator.Temp);
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 3;
        indices[4] = 1;
        indices[5] = 2;
        indices[6] = 3;
        indices[7] = 0;

        for (int i = 0; i < indices.Length; i += 2)
        {
            bool hasWall = (wallFlags & (1 << (i / 2))) != 0;
            UnityEngine.Debug.DrawLine(points[indices[i]], points[indices[i + 1]], hasWall ? UnityEngine.Color.blue : UnityEngine.Color.red, 0.4f);
        }
    }
}