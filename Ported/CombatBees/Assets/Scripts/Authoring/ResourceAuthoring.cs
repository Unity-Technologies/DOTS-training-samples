using Unity.Entities;

/// <summary>
/// Resources are the food items
/// </summary>
class ResourceAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject ResourcePrefab;
    public int InitialCount;
}

class ResourceBaker : Baker<ResourceAuthoring>
{
    public override void Bake(ResourceAuthoring authoring)
    {
        AddComponent(new ResourceConfig
        {
            ResourcePrefab = GetEntity(authoring.ResourcePrefab),
            InitialCount = authoring.InitialCount
        });
    }
}