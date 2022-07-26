using Unity.Entities;

class GrabbedAuthoring : UnityEngine.MonoBehaviour
{
}

class GrabbedBaker : Baker<GrabbedAuthoring>
{
    public override void Bake(GrabbedAuthoring authoring)
    {
        AddComponent(new Grabbed());
    }
}