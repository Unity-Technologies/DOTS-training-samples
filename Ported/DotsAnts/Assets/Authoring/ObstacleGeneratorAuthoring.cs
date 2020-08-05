using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ObstacleGeneratorAuthoring : IComponentData
{
    public int obstacleRingCount;
    public float obstaclesPerRing;
    public float obstacleRadius;
}
