using Unity.Entities;
using Unity.Mathematics;

class PlatformAuthoring : UnityEngine.MonoBehaviour
{
}

class PlatformBaker : Baker<PlatformAuthoring>
{
    public override void Bake(PlatformAuthoring authoring)
    {
        AddComponent<PlatformStaticInfo>();
        AddComponent<PlatformDynamicInfo>();
    }
}