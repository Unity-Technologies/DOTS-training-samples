using Unity.Entities;

class StackedAuthoring : UnityEngine.MonoBehaviour
{
}

class StackedBaker : Baker<StackedAuthoring>
{
    public override void Bake(StackedAuthoring authoring)
    {
        AddComponent(new Stacked());
    }
}