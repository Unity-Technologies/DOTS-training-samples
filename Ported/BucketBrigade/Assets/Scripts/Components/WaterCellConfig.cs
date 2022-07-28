using Unity.Entities;

struct WaterCellConfig : IComponentData
{
    public Entity WaterCellPrefab;
    public int CellCount;
    public float GridSize;
    public float CellSize;
}
