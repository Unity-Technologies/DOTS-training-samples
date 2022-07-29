using Unity.Entities;

struct BucketConfig : IComponentData
{
    public Entity Prefab;
    public float GridSize;
    public float CellSize;
    public int Count;
    public float Capacity;
}
