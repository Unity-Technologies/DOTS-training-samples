using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Water;

namespace Fire
{
    [RequiresEntityConversion]
    public class FireAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
    {
        public UnityEngine.Color UnlitColor = UnityEngine.Color.green;
        public UnityEngine.Color LitLowColor = UnityEngine.Color.yellow;
        public UnityEngine.Color LitHighColor = UnityEngine.Color.red;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TemperatureComponent
            {
                Value = 0f,
                Velocity = 0f,
                StartVelocity = 0f,
                IgnitionVariance = 0f,
                GridIndex = 0
            });

            dstManager.AddComponentData(entity, new StartHeight { Value = 0f });
            dstManager.AddComponentData(entity, new BoundsComponent
            {
                SizeXZ = transform.localScale.x,
                SizeY = transform.localScale.y
            });

            // Add fire color
            dstManager.AddComponentData(entity, new FireColor { Value = new float4(UnlitColor.r, UnlitColor.g, UnlitColor.b, 1) });
            dstManager.AddComponentData(entity, new FireColorPalette
            {
                UnlitColor = new float4(UnlitColor.r, UnlitColor.g, UnlitColor.b, 1),
                LitLowColor = new float4(LitLowColor.r, LitLowColor.g, LitLowColor.b, 1),
                LitHighColor = new float4(LitHighColor.r, LitHighColor.g, LitHighColor.b, 1)
            });
        }
    }

    [MaterialProperty("_BaseColor", MaterialPropertyFormat.Float4)]
    public struct FireColor : IComponentData
    {
        public float4 Value;
    }

    public struct FireColorPalette : IComponentData
    {
        public float4 UnlitColor;
        public float4 LitLowColor;
        public float4 LitHighColor;
    }

    public struct StartHeight : IComponentData
    {
        public float Value;
        public float Variance;
    }

    public struct TemperatureComponent : IComponentData
    {
        public float Value;
        public float Velocity;
        public float StartVelocity;
        public float IgnitionVariance;
        public int GridIndex;
    }

    // TODO make bounds readonly once initialized
    public struct BoundsComponent : IComponentData
    {
        public float SizeXZ;
        public float SizeY;
    }
}
