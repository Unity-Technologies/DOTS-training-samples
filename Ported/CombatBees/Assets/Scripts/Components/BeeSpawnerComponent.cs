using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BeeSpawnerComponent : IComponentData
{
	public Entity BeePrefab;
	public int BeeCount;
	public float3 BeeSpawnPosition;
	public int Process;
}
