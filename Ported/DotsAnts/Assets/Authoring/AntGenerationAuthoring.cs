using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AntGeneration : IComponentData
{
    public Entity AntPrefab;
}