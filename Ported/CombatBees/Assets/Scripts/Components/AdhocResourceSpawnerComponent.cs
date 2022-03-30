using Unity.Entities;
using Unity.Mathematics;

public struct AdhocResourceSpawnerComponent : IComponentData
{
	public Entity ResourcePrefab;
	public int ResourceCount;
	public float3 ResourceSpawnPosition;
}
