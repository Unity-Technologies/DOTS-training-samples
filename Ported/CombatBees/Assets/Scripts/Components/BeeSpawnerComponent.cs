using Unity.Entities;

public struct BeeSpawnerComponent : IComponentData
{
	public Entity BeePrefab;
	public int BeeCount;
}
