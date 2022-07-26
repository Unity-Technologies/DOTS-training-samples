using Unity.Entities;

class BeeStateAttackingAuthoring : UnityEngine.MonoBehaviour
{
}

class BeeStateAttackingBaker : Baker<BeeStateAttackingAuthoring>
{
    public override void Bake(BeeStateAttackingAuthoring authoring)
    {
        AddComponent(new BeeStateAttacking());
    }
}