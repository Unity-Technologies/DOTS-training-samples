using Unity.Entities;

struct WaterCellConfig : IComponentData
{
    public Entity TerrainCellPrefab;
    public int CellCount;
}
