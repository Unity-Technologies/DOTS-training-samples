using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
#if HDRP_EXISTS
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif

namespace Unity.Rendering
{
    public struct LightComponent : IComponentData
    {
        public LightType type;
        public Color color;
        public float colorTemperature;
        public float range;
        public float intensity;
        public int cullingMask;
        public int renderingLayerMask;

        // Spot specific
        public float spotAngle;
        public float innerSpotAngle;

        // Shadow settings
        public LightShadows shadows;
        public int shadowCustomResolution;
        public float shadowNearPlane;
        public float shadowBias;
        public float shadowNormalBias;
        public float shadowStrength;
    }

    [Serializable]
    public struct LightCookie : ISharedComponentData, IEquatable<LightCookie>
    {
        public UnityEngine.Texture texture;

        public bool Equals(LightCookie other)
        {
            return texture == other.texture;
        }

        public override int GetHashCode()
        {
            return (texture != null ? texture.GetHashCode() : 0);
        }
    }

#if HDRP_EXISTS
    // Optional dependency to com.unity.render-pipelines.high-definition
    public struct HDLightData : IComponentData
    {
        public LightTypeExtent lightTypeExtent;

        public float intensity;
        public float lightDimmer;
        public float fadeDistance;
        public bool affectDiffuse;
        public bool affectSpecular;

        public float shapeWidth;
        public float shapeHeight;
        public float aspectRatio;
        public float shapeRadius;
        public float maxSmoothness;
        public bool applyRangeAttenuation;

        // Spot specific
        public SpotLightShape spotLightShape;
        public bool enableSpotReflector;
        public float innerSpotPercent;
    }

    public struct HDShadowData : IComponentData
    {
        public int shadowResolution;
        public float shadowDimmer;
        public float volumetricShadowDimmer;
        public float shadowFadeDistance;
        public bool contactShadows;
        public float viewBiasMin;
        public float viewBiasMax;
        public float viewBiasScale;
        public float normalBiasMin;
        public float normalBiasMax;
        public float normalBiasScale;
        public bool sampleBiasScale;
        public bool edgeLeakFixup;
        public bool edgeToleranceNormal;
        public float edgeTolerance;
    }
#endif
}
