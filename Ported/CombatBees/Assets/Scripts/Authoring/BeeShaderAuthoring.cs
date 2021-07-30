    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using UnityEngine;

    public class BeeShaderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Range(0, 1)] public float Threshold = 0.4f;
        public Color BeeColor = Color.yellow;
        public float Period = 20;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BeeShaderOverrideColor {Value = (Vector4) BeeColor});
            dstManager.AddComponentData(entity, new BeeShaderOverridePeriod {Value = Period});
            dstManager.AddComponentData(entity, new BeeShaderOverrideThreshold {Value = Threshold});
        }
        
    }
    
    [MaterialProperty("Color_b5f6e476e34e4699a387d4119c632d45", MaterialPropertyFormat.Float4)]
    public struct BeeShaderOverrideColor : IComponentData
    {
        public float4 Value;
    }

    [MaterialProperty("Vector1_7234e949b23949b082caa362ec876b37", MaterialPropertyFormat.Float)]
    public struct BeeShaderOverridePeriod : IComponentData
    {
        public float Value;
    }

    [MaterialProperty("Vector1_3a392b733a6d402e8cc73aa87833ce78", MaterialPropertyFormat.Float)]
    public struct BeeShaderOverrideThreshold : IComponentData
    {
        public float Value;
    }