using Unity.Entities;

class ResourceSpawnerAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject ResourcePrefab;
    public UnityEngine.Transform ResourceSpawn;
    public int ResourceCount;
}

class ResourceSpawnerBaker : Baker<ResourceSpawnerAuthoring>
{
    public override void Bake(ResourceSpawnerAuthoring authoring)
    {
        AddComponent(new ResourceSpawner
        {
            ResourceSpawn = GetEntity(authoring.ResourceSpawn),
            ResourcePrefab = GetEntity(authoring.ResourcePrefab),
            ResourceCount = authoring.ResourceCount,
        });
    }
}