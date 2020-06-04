using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[GenerateAuthoringComponent]
[MaterialProperty("_MyColor", MaterialPropertyFormat.Float4)]
public struct BucketColor : IComponentData
{
    public float4 Value;
}