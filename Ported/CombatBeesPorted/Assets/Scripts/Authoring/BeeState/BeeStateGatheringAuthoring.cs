using Unity.Entities;

class BeeStateGatheringAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeStateGatheringBaker : Baker<BeeStateGatheringAuthoring>
{
    public override void Bake(BeeStateGatheringAuthoring authoring)
    {
        AddComponent(new BeeStateGathering());
    }
}