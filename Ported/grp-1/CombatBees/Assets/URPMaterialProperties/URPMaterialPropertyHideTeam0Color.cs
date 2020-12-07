using Unity.Entities;
using Unity.Mathematics;

#if ENABLE_HYBRID_RENDERER_V2
namespace Unity.Rendering
{
    [MaterialProperty("_Team0Color", MaterialPropertyFormat.Float4)]
    [GenerateAuthoringComponent]
    public struct URPMaterialPropertyHideTeam0Color : IComponentData
    {
        public float4 Value;
    }
}
#endif
