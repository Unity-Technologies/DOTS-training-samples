using Unity.Mathematics;

public static class TerrainUtility
{
	public static int2 BoxFromLocalPosition(float3 localPos)
	{
		return new int2((int)math.floor(localPos.x / Constants.SPACING + 0.5f), (int)math.floor(localPos.z / Constants.SPACING + 0.5f));
	}

	public static float3 LocalPositionFromBox(int col, int row, float yPosition = 0)
	{
		return new float3(col * Constants.SPACING, yPosition, row * Constants.SPACING);
	}
}
