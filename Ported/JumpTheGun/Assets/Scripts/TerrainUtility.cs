using Unity.Mathematics;

public static class TerrainUtility
{
	public static int2 BoxFromLocalPosition_Unsafe(float3 localPos)
	{
		return new int2((int)math.floor(localPos.x / Constants.SPACING + 0.5f), (int)math.floor(localPos.z / Constants.SPACING + 0.5f));
	}

	public static int2 BoxFromLocalPosition(float3 localPos, int terrainWidth, int terrainLength)
	{
		int2 boxPos = new int2((int)math.floor(localPos.x / Constants.SPACING + 0.5f), (int)math.floor(localPos.z / Constants.SPACING + 0.5f));

		boxPos.x = math.clamp(boxPos.x, 0, terrainWidth - 1);
		boxPos.y = math.clamp(boxPos.y, 0, terrainLength - 1);
		return boxPos;
	}

	public static float3 LocalPositionFromBox(int col, int row, float yPosition = 0)
	{
		return new float3(col * Constants.SPACING, yPosition, row * Constants.SPACING);
	}
}
