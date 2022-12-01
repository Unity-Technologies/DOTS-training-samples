using Unity.Entities;
using Unity.Transforms;

class BeeStateAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeStateBaker : Baker<BeeStateAuthoring>
{
    public override void Bake(BeeStateAuthoring authoring)
    {
        AddComponent<BeeState>();
        AddComponent<LocalToWorld>();
    }
}
