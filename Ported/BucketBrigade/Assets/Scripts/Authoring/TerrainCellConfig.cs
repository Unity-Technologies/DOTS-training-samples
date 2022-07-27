using Unity.Entities;

struct TerrainCellConfig : IComponentData
{
    public Entity Prefab;
    public int GridSize;
    public float CellSize;
}
