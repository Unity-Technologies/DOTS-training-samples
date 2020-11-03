using Unity.Entities;

public struct Spawner : IComponentData
{
    public Entity ScooperPrefab;
    public Entity BucketPrefab;
    public int BucketCount;
    public Entity FireCell;
    public int FireGridDimension;
    public Entity WaterCell;
    public int WaterCellCount;
}


