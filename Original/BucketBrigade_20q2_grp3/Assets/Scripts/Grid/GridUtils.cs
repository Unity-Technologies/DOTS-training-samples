using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public static class GridUtils
{
    public static GridPlane GridPlane;

    public static GridData CreateGrid(int width, int height, float cellSize)
    {
        GridData.Instance = new GridData
        {
            Width = width,
            Height = height,
            CellSize = cellSize,
            Heat = new NativeArray<byte>(width * height, Allocator.Persistent),
            Color = new NativeArray<Color>(width * height, Allocator.Persistent),
        };
        return GridData.Instance;
    }

    public static int GetIndex(this GridData data, Vector2Int address)
    {
        return address.x + data.Width * address.y;
    }

    public static int GetIndex(this GridData data, (int x, int y) address)
    {
        return address.x + data.Width * address.y;
    }

    public static Vector2Int GetAddress(this GridData data, int index)
    {
        return new Vector2Int(index % data.Width, index / data.Width);
    }

    public static bool InBounds(this GridData data, Vector2Int address)
    {
        return address.x >= 0 && address.y >= 0 && address.x < data.Width && address.y < data.Height;
    }

    public static IEnumerable<Vector2Int> GetAddressInRange(this GridData data, Vector2Int address, int distance)
    {
        yield return address;
    }

    public static bool TryGetAddressFromWorldPosition(this GridData data, Vector3 position, out Vector2Int address)
    {
        address = default;
        var local = position / data.CellSize;
        if (local.x < 0 || local.x < 0 || local.x > data.Width || local.z > data.Height)
            return false;

        address.x = (int) local.x;
        address.y = (int) local.z;
        return true;
    }

    public static bool TryGetAddressFromWorldPosition(this GridData data, Vector3 position, out int index)
    {
        if (data.TryGetAddressFromWorldPosition(position, out Vector2Int address))
        {
            index = data.GetIndex(address);
            return true;
        }

        index = -1;
        return false;
    }
}
