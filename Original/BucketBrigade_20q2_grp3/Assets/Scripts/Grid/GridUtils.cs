using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class GridUtils
{
    public static GridData CreateGrid(int width, int height)
    {
        GridData.Instance = new GridData { Width = width, Height = height, Heat = new NativeArray<byte>(width * height, Allocator.Persistent) };
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
}
