using Unity.Entities;

class ExplosionAnimAuthoring : UnityEngine.MonoBehaviour
{
}

class ExplosionAnimBaker : Baker<ExplosionAnimAuthoring>
{
    public override void Bake(ExplosionAnimAuthoring authoring)
    {
        AddComponent(new ExplosionAnim());
    }
}