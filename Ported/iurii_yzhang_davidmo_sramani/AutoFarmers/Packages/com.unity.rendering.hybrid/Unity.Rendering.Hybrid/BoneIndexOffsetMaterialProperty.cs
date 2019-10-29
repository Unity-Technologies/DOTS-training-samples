using Unity.Entities;

namespace Unity.Rendering
{
    /// <summary>
    /// Material property that holds the index offset at which the skin matrices of are stored. The value is linked with the "_SkinMatricesOffset" shader property.
    /// </summary>
    [MaterialProperty("_SkinMatricesOffset", MaterialPropertyFormat.Float)]
    struct BoneIndexOffsetMaterialProperty : IComponentData
    {
        public float Value;
    }
}
