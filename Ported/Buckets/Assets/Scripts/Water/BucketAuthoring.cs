using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Water
{
    [RequiresEntityConversion]
    public class BucketAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public UnityEngine.Color EmptyColor = UnityEngine.Color.magenta;
        public UnityEngine.Color FullColor = UnityEngine.Color.blue;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FillAmount { Value = 0f });

            // Add water color
            dstManager.AddComponentData(entity, new WaterColor { Value = new float4(EmptyColor.r, EmptyColor.g, EmptyColor.b, 1) });
            dstManager.AddComponentData(entity, new WaterColorPalette
            {
                EmptyColor = new float4(EmptyColor.r, EmptyColor.g, EmptyColor.b, 1),
                FullColor = new float4(FullColor.r, FullColor.g, FullColor.b, 1),
            });
        }
    }

    [MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
    public struct WaterColor : IComponentData
    {
        public float4 Value;
    }

    public struct WaterColorPalette : IComponentData
    {
        public float4 EmptyColor;
        public float4 FullColor;
    }

    public struct FillAmount : IComponentData
    {
        public float Value;
    }
}
