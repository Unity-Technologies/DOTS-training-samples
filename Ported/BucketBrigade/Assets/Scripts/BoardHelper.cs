using Unity.Mathematics;

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
	
	public static void ApplySpiralOffset(int n, ref float posX, ref float posZ)
	{
		float k = math.ceil((math.sqrt(n) - 1.0f) / 2.0f);
		float t = 2.0f * k;
		float m = (t + 1f) * (t + 1f);
		if (n >= m - t)
		{
			posX += k - (m - n);
			posZ += -k;
			return;
		}
        
		m -= t;

		if (n >= m - t)
		{
			posX += -k;
			posZ += -k + (m - n);
			return;
		}
        
		m -= t;

		if (n >= m - t)
		{
			posX += -k + (m-n);
			posZ += k;
			return;
		}
    
		posX += k;
		posZ += k - (m - n - t);
	}
}
