using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MapResolution : IComponentData
{
    public int value;
}