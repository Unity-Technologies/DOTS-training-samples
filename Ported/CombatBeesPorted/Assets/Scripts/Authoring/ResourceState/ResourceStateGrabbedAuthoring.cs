using Unity.Entities;

class ResourceStateGrabbedAuthoring : UnityEngine.MonoBehaviour
{
}

class ResourceStateGrabbedBaker : Baker<ResourceStateGrabbedAuthoring>
{
    public override void Bake(ResourceStateGrabbedAuthoring authoring)
    {
        AddComponent(new ResourceStateGrabbed());
    }
}