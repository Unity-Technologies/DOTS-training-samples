using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ObstacleRingCount : IComponentData
{
    public int value;
}