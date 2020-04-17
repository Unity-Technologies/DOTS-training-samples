using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_UnlitColor"           , MaterialPropertyFormat.Float4)]
    [GenerateAuthoringComponent]
    public struct HDRPMaterialPropertyUnlitColor            : IComponentData { public float4 Value; }
}
