using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public struct FireGridCell : IBufferElementData
{
    public float Temperature;
}

public struct FireGrid : IComponentData
{
    
}

[MaterialProperty("_BaseColor", MaterialPropertyFormat.Float3)]
public struct CubeColor : IComponentData
{
    public float3 Color;
}

public struct GridCellIndex : IComponentData
{
    public int2 CellPos;
    public int Index;
}

