using UnityEngine;
using Unity.Collections;

public static class TileUtils
{
    // Start is called before the first frame update
    public static byte GetTile(NativeArray<byte> tiles, int x, int y, int width)
    {
        int i = y*width + x;
        Debug.Assert(i < tiles.Length);

        if (i < tiles.Length)
            return tiles[i];
        else
            return 0;
    }

    public static bool IsHole(byte tile)
    {
        return (tile & (1 << 4)) != 0;
    }

    // returns -1 for no base,
    // returns the base number 0-3 if it exists
    public static int BaseId(byte tile)
    {
        int id = ((int)tile >> 5);
        return (id > 0) ? id - 1 : -1;
    }
}
