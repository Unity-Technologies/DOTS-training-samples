using Unity.Entities;
using UnityEngine;

struct FireFighterConfig : IComponentData
{
    public Color WaterBringersColor;
    public Color BucketBringersColor;
    public Color BucketFillerFetcherColor;
    public Color WaterDumperColor;
    public Entity FireFighterPrefab;
    public float GridSize;
    public float CellSize;
    public int LinesCount;
    public int PerLinesCount;
    public float Speed;
}
