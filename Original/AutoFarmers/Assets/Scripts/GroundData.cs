using Unity.Entities;

public struct GroundData : IComponentData
{
	public Entity groundEntity;
	public int fieldSizeX;
	public int fieldSizeY;
}