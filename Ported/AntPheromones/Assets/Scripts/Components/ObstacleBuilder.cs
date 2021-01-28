using Unity.Entities;
using Unity.Mathematics;

public struct ObstacleBuilder : IComponentData
{
    public int2 dimensions;


    public Entity obstaclePrefab;
    public float obstacleRadius;
    public float4 obstacleColor;

    public int numberOfRings;
    public float openingDegrees;
}
