using Unity.Entities;

public struct ObstacleSpawner : IComponentData
{
    public Entity ObstaclePrefab;
    public int obstacleRingCount;
    public float ObstaclesPerRing;
    public float ObstacleRadius;
}
