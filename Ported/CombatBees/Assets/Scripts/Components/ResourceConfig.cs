using Unity.Entities;

struct ResourceConfig : IComponentData
{
    public Entity resourcePrefab;
    public float resourceSize;
    public float snapStiffness;
    public float carryStiffness;
    public float spawnRate;
    public int beesPerResource;
    public int startResourceCount;
}
