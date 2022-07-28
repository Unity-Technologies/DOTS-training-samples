using Unity.Entities;

class BloodAnimAuthoring : UnityEngine.MonoBehaviour
{
}

class BloodAnimBaker : Baker<BloodAnimAuthoring>
{
    public override void Bake(BloodAnimAuthoring authoring)
    {
        AddComponent(new BloodAnim());
    }
}