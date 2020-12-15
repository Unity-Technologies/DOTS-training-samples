using Unity.Entities;

public struct FloatGrid : IComponentData
{
	public DynamicBuffer<float> cells;
	public int width;

	public float this[int x, int y] => cells[y * width + x];
}