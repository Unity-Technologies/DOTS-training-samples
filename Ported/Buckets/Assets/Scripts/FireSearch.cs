using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    /// <summary>
    /// Finds indicies for neighbors. If they do not exist, will return -1
    /// </summary>
    /// <param name="index"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Neighbors GetNeighboringIndicies(int index, int width, int height)
    {
        int indexCol = index % width;
        int indexRow = index / height;

        int topRow = math.clamp(indexRow - 1, 0, height - 1);
        int bottomRow = math.clamp(indexRow + 1, 0, height - 1);

        int leftCol = math.clamp(indexCol - 1, 0, width - 1);
        int rightCol = math.clamp(indexCol + 1, 0, width - 1);

        int topIndex = width * topRow + indexCol;
        int bottomIndex = width * bottomRow + indexCol;
        int leftIndex = width * indexRow + leftCol;
        int rightIndex = width * indexRow + rightCol;

        return new Neighbors
        {
            // If neighbor index matches current index, then that means the neighbor doesn't exist
            Top = topIndex == index ?  -1 : topIndex,
            Bottom = bottomIndex == index ? -1 : bottomIndex,
            Left = leftIndex == index ? -1 : leftIndex,
            Right = rightIndex == index ? -1 : rightIndex
        };
    }
}
