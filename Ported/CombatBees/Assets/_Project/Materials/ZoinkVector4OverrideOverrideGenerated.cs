using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_Zoink", MaterialPropertyFormat.Float4)]
    struct ZoinkVector4Override : IComponentData
    {
        public float4 Value;
    }
}
