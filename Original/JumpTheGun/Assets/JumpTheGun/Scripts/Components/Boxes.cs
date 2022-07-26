using Unity.Entities;

// An empty component is called a "tag component".
struct Box : IComponentData
{
    public Entity boxSpawn;

    public Entity boxPrefab;

    public float boxHeight;

    public int boxHeightDamage;
}