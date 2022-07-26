using Unity.Entities;

class ResourceStateStackedAuthoring : UnityEngine.MonoBehaviour
{
}

class ResourceStateStackedBaker : Baker<ResourceStateStackedAuthoring>
{
    public override void Bake(ResourceStateStackedAuthoring authoring)
    {
        AddComponent(new ResourceStateStacked());
    }
}