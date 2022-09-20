using Unity.Entities;

public class TargetAuthoring : UnityEngine.MonoBehaviour
{
}

class TargetBaker : Baker<TargetAuthoring>
{
    public override void Bake(TargetAuthoring authoring)
    {
        AddComponent<Target>();
    }
}