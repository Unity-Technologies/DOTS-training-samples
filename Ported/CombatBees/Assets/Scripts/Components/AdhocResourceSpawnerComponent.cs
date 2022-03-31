using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AdhocResourceSpawnerComponent : IComponentData
{
	public Entity ResourcePrefab;
	public int ResourceCount;
	public float3 ResourceSpawnPosition;
}
