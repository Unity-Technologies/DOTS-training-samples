using Unity.Entities;

class ResourceConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject resourcePrefab;
    public float resourceSize = 0.75f;
    public float snapStiffness = 2f;
    public float carryStiffness = 15f;
    public float spawnRate = 10f;
    public int beesPerResource = 8;
    public int startResourceCount = 500;
}

class ResourceConfigBaker : Baker<ResourceConfigAuthoring>
{
    public override void Bake(ResourceConfigAuthoring authoring)
    {
        AddComponent(new ResourceConfig
        {
            resourcePrefab = GetEntity(authoring.resourcePrefab),
            resourceSize = authoring.resourceSize,
            snapStiffness = authoring.snapStiffness,
            carryStiffness = authoring.carryStiffness,
            spawnRate = authoring.spawnRate,
            beesPerResource = authoring.beesPerResource,
            startResourceCount = authoring.startResourceCount
});
    }
}
