using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ObstaclesRadius : IComponentData
{
    public float value;
}