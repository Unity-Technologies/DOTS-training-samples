using Unity.Entities;
using Unity.Mathematics;

public struct BeeSpawnerComponent : IComponentData
{
	public Entity BeePrefab;
	public int BeeCount;
	public float3 BeeSpawnPosition;
	public TeamTag BeeTeamTag;
}
