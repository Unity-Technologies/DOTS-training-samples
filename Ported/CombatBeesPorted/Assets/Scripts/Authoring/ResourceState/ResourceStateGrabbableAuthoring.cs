using Unity.Entities;

class ResourceStateGrabbableAuthoring : UnityEngine.MonoBehaviour
{
}

class ResourceStateGrabbableBaker : Baker<ResourceStateGrabbableAuthoring>
{
    public override void Bake(ResourceStateGrabbableAuthoring authoring)
    {
        AddComponent(new ResourceStateGrabbable());
    }
}