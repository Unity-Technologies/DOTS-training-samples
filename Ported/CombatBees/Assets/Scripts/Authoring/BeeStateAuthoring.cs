using Unity.Entities;

class BeeStateAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeStateBaker : Baker<BeeStateAuthoring>
{
    public override void Bake(BeeStateAuthoring authoring)
    {
        AddComponent<BeeState>();
    }
}
