using Unity.Entities;

public struct WaterDrop : IComponentData
{
	public int X;
	public int Z;
	public int Range;
	public float Strength;
}
