using Unity.Entities;

class ResourceBelowAuthoring : UnityEngine.MonoBehaviour
{
}

class ResourceBelowBaker : Baker<ResourceBelowAuthoring>
{
    public override void Bake(ResourceBelowAuthoring authoring)
    {
        AddComponent<ResourceBelow>();
    }
}