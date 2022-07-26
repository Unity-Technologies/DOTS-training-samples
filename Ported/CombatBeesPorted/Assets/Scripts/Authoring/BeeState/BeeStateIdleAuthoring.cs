using Unity.Entities;

class BeeStateIdleAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeStateIdleBaker : Baker<BeeStateIdleAuthoring>
{
    public override void Bake(BeeStateIdleAuthoring authoring)
    {
        AddComponent(new BeeStateIdle());
    }
}