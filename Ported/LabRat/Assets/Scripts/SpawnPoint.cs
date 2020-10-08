using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnPoint : IComponentData
{
    public Entity spawnPrefab;
    public byte direction;
    public int spawnCount;
    public float spawnFrequency;
    public float2 speedRange;
}
