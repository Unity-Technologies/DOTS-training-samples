using Unity.Entities;
using UnityEngine;

struct TerrainCellConfig : IComponentData
{
    public Entity Prefab;
    public int GridSize;
    public float CellSize;
    public Color NeutralCol;
    public Color CoolCol;
    public Color HotCol;
}
