using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace ShaderGraph
{
    public class ShaderOverridesAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Range(0, 1)] public float CenterSize = 0.5f;
        public Color LeftColor = Color.red;
        public Color RightColor = Color.blue;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new ShaderOverrideCenterSize { Value = CenterSize });
            dstManager.AddComponentData(entity, new ShaderOverrideLeftColor { Value = (Vector4)LeftColor });
            dstManager.AddComponentData(entity, new ShaderOverrideRightColor { Value = (Vector4)RightColor });
        }
    }

    [MaterialProperty("Vector1_d334671a210a44d3b58d89879b1dceae", MaterialPropertyFormat.Float)]
    public struct ShaderOverrideCenterSize : IComponentData
    {
        public float Value;
    }

    [MaterialProperty("Color_d9b47626e873463fbd997c9a6a857bf2", MaterialPropertyFormat.Float4)]
    public struct ShaderOverrideLeftColor : IComponentData
    {
        public float4 Value;
    }

    [MaterialProperty("Color_5e06a8bc7fbe4284bef1dcf16b184948", MaterialPropertyFormat.Float4)]
    public struct ShaderOverrideRightColor : IComponentData
    {
        public float4 Value;
    }
}