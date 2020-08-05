using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AntCount : IComponentData
{
    public int value;
}