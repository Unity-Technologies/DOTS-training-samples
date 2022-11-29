using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public class ResourceAuthoring : UnityEngine.MonoBehaviour
{
}

class ResourceBaker : Baker<ResourceAuthoring>
{
    public override void Bake(ResourceAuthoring authoring)
    {
        AddComponent<LocalToWorld>();
        AddComponent<ResourceComponent>();
        AddComponent<ResourceCarriedComponent>();
        AddComponent<ResourceDroppedComponent>();
    }
}