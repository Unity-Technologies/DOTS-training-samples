using Unity.Entities;

struct BucketConfig : IComponentData
{
    public Entity Prefab;
    public float GridSize;
    public int Count;
}
