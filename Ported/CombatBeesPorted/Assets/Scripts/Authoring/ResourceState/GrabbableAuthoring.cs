using Unity.Entities;

class GrabbableAuthoring : UnityEngine.MonoBehaviour
{
}

class GrabbableBaker : Baker<GrabbableAuthoring>
{
    public override void Bake(GrabbableAuthoring authoring)
    {
        AddComponent(new Grabbable());
    }
}