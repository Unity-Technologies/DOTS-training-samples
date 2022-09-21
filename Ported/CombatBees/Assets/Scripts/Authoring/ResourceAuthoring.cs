using Unity.Entities;

class ResourceAuthoring : UnityEngine.MonoBehaviour
{}

class ResourceBaker : Baker<ResourceAuthoring>
{
    public override void Bake(ResourceAuthoring authoring)
    {
        AddComponent<Holder>();
    }
}
