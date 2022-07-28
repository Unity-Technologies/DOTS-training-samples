using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

class BucketAuthoring : UnityEngine.MonoBehaviour
{
    public float Volume = 0.0f;
}

class BucketBaker : Baker<BucketAuthoring>
{
    public override void Bake(BucketAuthoring authoring)
    {
        float2 wsPosition = new float2(authoring.transform.position.x, authoring.transform.position.z);
        AddComponent(new Position { Value = wsPosition });
        AddComponent(new Volume { Value = authoring.Volume });
        AddComponent(new BucketId { Value = 0 });
        AddComponent(new BucketInfo { IsTaken = false,
                                      Position = float2.zero });
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}