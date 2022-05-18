using Unity.Entities;
using Unity.Rendering;

class BucketAuthoring : UnityEngine.MonoBehaviour
{
}

class BucketBaker : Baker<BucketAuthoring>
{
    public override void Bake(BucketAuthoring authoring)
    {
        AddComponent<Bucket>();
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}