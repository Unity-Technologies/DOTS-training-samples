using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SpawnPoint : IComponentData
{
    public int spawnType;
    public byte direction;
    public int spawnCount;
    public float spawnDelay;
}

public struct SpawnType : IBufferElementData
{
    public Entity spawnPrefab;
    public int spawnMax;
    public float spawnFrequency;
    public float2 speedRange;
}