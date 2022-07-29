using Unity.Entities;
using UnityEngine;

struct TerrainCellConfig : IComponentData
{
    public Entity Prefab;
    public int GridSize;
    public float CellSize;
    public int CoolRadius;
    public float FlickerRate;
    public float FlickerRange;
    public Color NeutralCol;
    public Color CoolCol;
    public Color HotCol;
}
