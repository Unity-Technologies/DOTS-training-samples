using Unity.Entities;

struct ResourceConfig : IComponentData
{
    public Entity ResourcePrefab;
    public int InitialCount;
}