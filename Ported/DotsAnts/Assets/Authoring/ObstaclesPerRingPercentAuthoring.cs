using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ObstaclesPerRingPercent : IComponentData
{
    public float value;
}