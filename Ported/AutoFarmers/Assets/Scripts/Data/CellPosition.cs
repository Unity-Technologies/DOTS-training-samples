using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CellPosition : IComponentData
{
    public int2 Value;
}