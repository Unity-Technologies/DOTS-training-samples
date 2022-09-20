using Unity.Entities;

struct ResourceConfig : IComponentData
{
    //public Mesh resourceMesh;
    //public Material resourceMaterial;
    //public Color resourceColor;
    public float resourceSize;
    public float snapStiffness;
    public float carryStiffness;
    public float spawnRate;
    public int beesPerResource;
    public int startResourceCount;
}
