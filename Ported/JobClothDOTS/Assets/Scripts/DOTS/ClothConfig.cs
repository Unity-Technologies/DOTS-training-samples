using Unity.Mathematics;

public static class ClothConfig
{
	public static readonly float3 worldGravity = new float3(0.0f, -1.0f, 0.0f);
	public static readonly float worldGroundHeight = 0.0f;

	public static readonly int solverIterations = 10;
	public static readonly float solverSORFactor = 2.0f;// [1..2]
}
