using Unity.Entities;

public struct GridDimensions : IComponentData
{
    public int CellsX;
    public int CellsZ;
    public float CellSize;
}