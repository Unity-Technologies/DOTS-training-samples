using Unity.Entities;

struct ResourceSpawner : IComponentData
{
    public Entity ResourceSpawn;
 
    public Entity ResourcePrefab;

    public int ResourceCount;
}