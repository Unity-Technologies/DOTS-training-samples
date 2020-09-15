using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Position : IComponentData
{
    public int2 Value;
}
