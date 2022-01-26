using Unity.Entities;
using Unity.Mathematics;

public struct Spawner : IComponentData
{
    public Entity AntPrefab;
    public Entity ColonyPrefab;
    public Entity GroundPrefab;
    public Entity ObstaclePrefab;
    public Entity ResourcePrefab;
}
