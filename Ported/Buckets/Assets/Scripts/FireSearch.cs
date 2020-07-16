using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FireSearch
{
    public struct Neighbors
    {
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;
    }

    public static Neighbors GetNeighboringIndicies(int index, int width, int height)
    {
        return new Neighbors
        {
            Top = index > width - 1 ? index - height : -1,
            Bottom = (width * height) - index > width ? index + height : -1,
            Left = index % width != 0 ? index - 1 : -1,
            Right = index % width - 1 != 0 ? index + 1 : -1
        };
    }
}
