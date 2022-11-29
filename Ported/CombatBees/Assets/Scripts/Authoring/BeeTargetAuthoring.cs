using Unity.Entities;

class BeeTargetAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeTargetBaker : Baker<BeeTargetAuthoring>
{
    public override void Bake(BeeTargetAuthoring authoring)
    {
        AddComponent(new BeeTarget
        { 
            target = Entity.Null 
        });
    }
}
