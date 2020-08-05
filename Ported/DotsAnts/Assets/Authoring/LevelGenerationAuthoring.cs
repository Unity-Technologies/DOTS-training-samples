using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct LevelGeneration : IComponentData
{
    public int obstacleRingCount;
    public float obstaclesPerRing;
    public float obstacleRadius;
}
