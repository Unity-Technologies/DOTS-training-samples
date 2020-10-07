public static class BoardHelper
{
	public const int MaxFireValue = 100;
	
	public static bool TryGet2DArrayIndex(int x, int z, int mapSizeX, int mapSizeZ, out int index)
	{
		if (x < 0 || x >= mapSizeX || z < 0 || z >= mapSizeZ)
		{
			index = -1;
			return false;
		}

		index = x + z * mapSizeX;
		return true;
	}
}
