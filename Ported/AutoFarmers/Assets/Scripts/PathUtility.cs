using Unity.Mathematics;
using UnityEngine;

public class PathUtility
{
    public static int Hash(int x, int y, int mapWidth)
    {
		return y * mapWidth + x;
	}

	public static void Unhash(int hash, int mapWidth, int mapHeight, out int x, out int y) 
    {
		y = hash / mapWidth;
		x = hash % mapHeight;
	}
}