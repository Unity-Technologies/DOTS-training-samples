using Unity.Entities;
#if HDRP_EXISTS
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif

namespace Unity.Rendering
{
    class LightConversion : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((UnityEngine.Light unityLight) =>
            {
                var entity = GetPrimaryEntity(unityLight);

                LightComponent light;
                light.type                      = unityLight.type;
                light.color                     = unityLight.color;
                light.colorTemperature          = unityLight.colorTemperature;
                light.range                     = unityLight.range;
                light.intensity                 = unityLight.intensity;
                light.cullingMask               = unityLight.cullingMask;
                light.renderingLayerMask        = unityLight.renderingLayerMask;
                light.spotAngle                 = unityLight.spotAngle;
                light.innerSpotAngle            = unityLight.innerSpotAngle;
                light.shadows                   = unityLight.shadows;
                light.shadowCustomResolution    = unityLight.shadowCustomResolution;
                light.shadowNearPlane           = unityLight.shadowNearPlane;
                light.shadowBias                = unityLight.shadowBias;
                light.shadowNormalBias          = unityLight.shadowNormalBias;
                light.shadowStrength            = unityLight.shadowStrength;
                DstEntityManager.AddComponentData(entity, light);

                if (unityLight.cookie)
                {
                    LightCookie cookie;
                    cookie.texture = unityLight.cookie;
                    DstEntityManager.AddSharedComponentData(entity, cookie);
                }

#if HDRP_EXISTS
                // Optional dependency to com.unity.render-pipelines.high-definition
                var unityHdData = unityLight.GetComponent<HDAdditionalLightData>();
                HDLightData hdData;
                hdData.lightTypeExtent          = unityHdData.lightTypeExtent;
                hdData.intensity                = unityHdData.intensity;
                hdData.lightDimmer              = unityHdData.lightDimmer;
                hdData.fadeDistance             = unityHdData.fadeDistance;
                hdData.affectDiffuse            = unityHdData.affectDiffuse;
                hdData.affectSpecular           = unityHdData.affectSpecular;
                hdData.shapeWidth               = unityHdData.shapeWidth;
                hdData.shapeHeight              = unityHdData.shapeHeight;
                hdData.aspectRatio              = unityHdData.aspectRatio;
                hdData.shapeRadius              = unityHdData.shapeRadius;
                hdData.maxSmoothness            = unityHdData.maxSmoothness;
                hdData.applyRangeAttenuation    = unityHdData.applyRangeAttenuation;
                hdData.spotLightShape           = unityHdData.spotLightShape;
                hdData.enableSpotReflector      = unityHdData.enableSpotReflector;
                hdData.innerSpotPercent         = unityHdData.m_InnerSpotPercent;
                DstEntityManager.AddComponentData(entity, hdData);

                var unityShadowData = unityLight.GetComponent<AdditionalShadowData>();
                HDShadowData hdShadow;
                hdShadow.shadowResolution         = unityShadowData.shadowResolution;
                hdShadow.shadowDimmer             = unityShadowData.shadowDimmer;
                hdShadow.volumetricShadowDimmer   = unityShadowData.volumetricShadowDimmer;
                hdShadow.shadowFadeDistance       = unityShadowData.shadowFadeDistance;
                hdShadow.contactShadows           = unityShadowData.contactShadows;
                hdShadow.viewBiasMin              = unityShadowData.viewBiasMin;
                hdShadow.viewBiasMax              = unityShadowData.viewBiasMax;
                hdShadow.viewBiasScale            = unityShadowData.viewBiasScale;
                hdShadow.normalBiasMin            = unityShadowData.normalBiasMin;
                hdShadow.normalBiasMax            = unityShadowData.normalBiasMax;
                hdShadow.normalBiasScale          = unityShadowData.normalBiasScale;
                hdShadow.sampleBiasScale          = unityShadowData.sampleBiasScale;
                hdShadow.edgeLeakFixup            = unityShadowData.edgeLeakFixup;
                hdShadow.edgeToleranceNormal      = unityShadowData.edgeToleranceNormal;
                hdShadow.edgeTolerance            = unityShadowData.edgeTolerance;
                DstEntityManager.AddComponentData(entity, hdShadow);
#endif
            });
        }
    }
}
