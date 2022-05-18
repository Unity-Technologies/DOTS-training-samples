using Unity.Entities;
using Unity.Rendering;

class RailAuthoring : UnityEngine.MonoBehaviour
{
}

class RailBaker : Baker<RailAuthoring>
{
    public override void Bake(RailAuthoring authoring)
    {
        // By default, components are zero-initialized.
        // So in this case, the Speed field in CannonBall will be float3.zero.
        AddComponent<Rail>();
        AddComponent<URPMaterialPropertyBaseColor>();
    }
}