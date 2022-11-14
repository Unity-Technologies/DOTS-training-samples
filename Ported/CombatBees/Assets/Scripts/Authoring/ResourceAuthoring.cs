using Unity.Entities;

/// <summary>
/// Resources are the food items
/// </summary>
class ResourceAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject ResourcePrefab;
    public int InitialCount;
    public float SpawnRadius; // Todo confine within field container rather than hard-coded radius
}

class ResourceBaker : Baker<ResourceAuthoring>
{
    public override void Bake(ResourceAuthoring authoring)
    {
        AddComponent(new ResourceConfig
        {
            ResourcePrefab = GetEntity(authoring.ResourcePrefab),
            InitialCount = authoring.InitialCount,
            SpawnRadius = authoring.SpawnRadius
        });
    }
}