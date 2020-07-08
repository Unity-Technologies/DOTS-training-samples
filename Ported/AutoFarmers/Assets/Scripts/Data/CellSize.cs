using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CellSize : IComponentData
{
    public int2 Value;
}