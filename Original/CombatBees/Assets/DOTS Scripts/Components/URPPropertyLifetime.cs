using Unity.Entities;

#if ENABLE_HYBRID_RENDERER_V2
namespace Unity.Rendering
{
    [MaterialProperty("Vector1_b008ca34c44d45c6bcc3d00f1b9bfcd3", MaterialPropertyFormat.Float)]
    [GenerateAuthoringComponent]
    public struct URPPropertyLifetime : IComponentData
    {
        public float Value;
    }
}
#endif