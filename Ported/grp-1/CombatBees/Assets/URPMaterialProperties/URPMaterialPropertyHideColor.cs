using Unity.Entities;
using Unity.Mathematics;

#if ENABLE_HYBRID_RENDERER_V2
namespace Unity.Rendering
{
    [MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
    [GenerateAuthoringComponent]
    public struct URPMaterialPropertyHideColor : IComponentData
    {
        public float4 Value;
    }
}
#endif
