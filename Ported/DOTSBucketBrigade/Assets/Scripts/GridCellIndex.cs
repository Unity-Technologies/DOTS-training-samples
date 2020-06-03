using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct GridCellIndex : IComponentData
{
    //public int2 CellPos;
    public int Index;
}

