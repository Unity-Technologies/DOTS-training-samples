using Unity.Entities;

public struct TornadoSpawner : IComponentData
{
    public Entity particlePrefab;
    public int particleCount;
}