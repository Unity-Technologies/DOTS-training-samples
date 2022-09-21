using Unity.Entities;

class BeeAuthoring : UnityEngine.MonoBehaviour
{}

class BeeBaker : Baker<BeeAuthoring>
{
    public override void Bake(BeeAuthoring authoring)
    {
        AddComponent<IsAttacking>();
        AddComponent<IsHolding>();
    }
}