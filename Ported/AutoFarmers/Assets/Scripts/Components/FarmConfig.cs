using Unity.Entities;
using Unity.Mathematics;

public struct FarmConfig : IComponentData
{
	public int MapSizeX;
	public int MapSizeY;

	public Entity GroundPrefab;
	public float4 TilledGroundColor;
}
