using Unity.Entities;

class TargetObjectAuthoring : UnityEngine.MonoBehaviour
{
}

class TargetObjectBaker : Baker<TargetObjectAuthoring>
{
    public override void Bake(TargetObjectAuthoring authoring)
    {
        AddComponent<TargetObject>();
    }
}
