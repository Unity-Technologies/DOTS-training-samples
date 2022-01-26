using Unity.Entities;

public struct RailRoadTieSpawner : IComponentData
{
    public Entity TiePrefab;
    public float Frequency;
}
