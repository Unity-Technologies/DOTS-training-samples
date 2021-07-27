using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnBeeConfig : IComponentData
{
    public int Team;
    public Entity BeePrefab;
    public float3 SpawnLocation;
    public float3 SpawnAreaSize;
    public int BeeCount;
}
