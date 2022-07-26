using Unity.Entities;

class BeeStateReturningAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeStateReturningBaker : Baker<BeeStateReturningAuthoring>
{
    public override void Bake(BeeStateReturningAuthoring authoring)
    {
        AddComponent(new BeeStateReturning());
    }
}