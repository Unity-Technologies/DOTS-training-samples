// Shader targeted for low end devices. Single Pass Forward Rendering.
Shader "Universal Render Pipeline/Custom/Instanced Color Melt"
{
    // Keep properties of StandardSpecular shader for upgrade reasons.
    Properties
    {
        [MainTexture] _BaseMap("Base Map (RGB) Smoothness / Alpha (A)", 2D) = "white" {}
        [MainColor]   _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        _Cutoff("Alpha Clipping", Range(0.0, 1.0)) = 0.5

        _SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _SpecGlossMap("Specular Map", 2D) = "white" {}
        [Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessSource("Smoothness Source", Float) = 0.0
        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0

        [HideInInspector] _BumpScale("Scale", Float) = 1.0
        [NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}

        [HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
        [NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        [ToggleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0

            // Editmode props
            [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
            [HideInInspector] _Smoothness("Smoothness", Float) = 0.5

            // ObsoleteProperties
            [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
            [HideInInspector] _Color("Base Color", Color) = (1, 1, 1, 1)
            [HideInInspector] _Shininess("Smoothness", Float) = 0.0
            [HideInInspector] _GlossinessSource("GlossinessSource", Float) = 0.0
            [HideInInspector] _SpecSource("SpecularHighlights", Float) = 0.0

            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "SimpleLit" "IgnoreProjector" = "True" "ShaderModel" = "4.5"}
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

        // Use same blending / depth states as Standard shader
        Blend[_SrcBlend][_DstBlend]
        ZWrite[_ZWrite]
        Cull[_Cull]

        HLSLPROGRAM
        #pragma exclude_renderers gles gles3 glcore
        #pragma target 4.5

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
        #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
        #pragma shader_feature_local _NORMALMAP
        #pragma shader_feature_local_fragment _EMISSION
        #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

        // -------------------------------------
        // Universal Pipeline keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile_fog

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #pragma vertex LitPassVertexSimple
        #pragma fragment LitPassFragmentSimple
        #define BUMP_SCALE_NOT_SUPPORTED 1

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
       // #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"



        #ifndef UNIVERSAL_SIMPLE_LIT_PASS_INCLUDED
#define UNIVERSAL_SIMPLE_LIT_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
    float4 positionOS    : POSITION;
    float3 normalOS      : NORMAL;
    float4 tangentOS     : TANGENT;
    float2 texcoord      : TEXCOORD0;
    float2 lightmapUV    : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv                       : TEXCOORD0;
    DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

    float3 posWS                    : TEXCOORD2;    // xyz: posWS

#ifdef _NORMALMAP
    float4 normal                   : TEXCOORD3;    // xyz: normal, w: viewDir.x
    float4 tangent                  : TEXCOORD4;    // xyz: tangent, w: viewDir.y
    float4 bitangent                : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
#else
    float3  normal                  : TEXCOORD3;
    float3 viewDir                  : TEXCOORD4;
#endif

    half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord              : TEXCOORD7;
#endif

    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData.positionWS = input.posWS;

#ifdef _NORMALMAP
    half3 viewDirWS = half3(input.normal.w, input.tangent.w, input.bitangent.w);
    inputData.normalWS = TransformTangentToWorld(normalTS,
        half3x3(input.tangent.xyz, input.bitangent.xyz, input.normal.xyz));
#else
    half3 viewDirWS = input.viewDir;
    inputData.normalWS = input.normal;
#endif

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    viewDirWS = SafeNormalize(viewDirWS);

    inputData.viewDirectionWS = viewDirWS;

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    inputData.shadowCoord = input.shadowCoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
#else
    inputData.shadowCoord = float4(0, 0, 0, 0);
#endif

    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
}

///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

// Used in Standard (Simple Lighting) shader
Varyings LitPassVertexSimple(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.posWS.xyz = vertexInput.positionWS;
    output.positionCS = vertexInput.positionCS;

#ifdef _NORMALMAP
    output.normal = half4(normalInput.normalWS, viewDirWS.x);
    output.tangent = half4(normalInput.tangentWS, viewDirWS.y);
    output.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);
#else
    output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
    output.viewDir = viewDirWS;
#endif

    OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
    OUTPUT_SH(output.normal.xyz, output.vertexSH);

    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowCoord = GetShadowCoord(vertexInput);
#endif

    return output;
}

// Used for StandardSimpleLighting shader
half4 LitPassFragmentSimple(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    float2 uv = input.uv;
   // half4 diffuseAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    half4 diffuse = _BaseColor.rgba;

   // half alpha = diffuseAlpha.a * _BaseColor.a;
   // AlphaDiscard(alpha, _Cutoff);

    /*#ifdef _ALPHAPREMULTIPLY_ON
        diffuse *= alpha;
    #endif*/

    //half3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
    //half3 emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
    //half4 specular = SampleSpecularSmoothness(uv, alpha, _SpecColor, TEXTURE2D_ARGS(_SpecGlossMap, sampler_SpecGlossMap));
   // half smoothness = specular.a;

    InputData inputData;
   // InitializeInputData(input, normalTS, inputData);



    inputData.positionWS = input.posWS;

    half3 viewDirWS = input.viewDir;
    inputData.normalWS = input.normal;

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    viewDirWS = SafeNormalize(viewDirWS);

    inputData.viewDirectionWS = viewDirWS;
    inputData.shadowCoord = float4(0, 0, 0, 0);

    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);

    half4 color = UniversalFragmentBlinnPhong(inputData, diffuse, /*specular*/1, /*smoothness*/1, /*emission*/1, 1);
   // color.rgb = MixFog(color.rgb, inputData.fogCoord);
    //color.a = OutputAlpha(color.a, _Surface);


    input.posWS *= 2.f;
    clip(diffuse.a - (sin(input.posWS.x) * cos(input.posWS.y) * sin(input.posWS.z) * .5f + .5f));

    return float4(diffuse.rgb,1);
}

#endif











        ENDHLSL
    }

    Pass
    {
        Name "ShadowCaster"
        Tags{"LightMode" = "ShadowCaster"}

        ZWrite On
        ZTest LEqual
        ColorMask 0
        Cull[_Cull]

        HLSLPROGRAM
        #pragma exclude_renderers gles gles3 glcore
        #pragma target 4.5

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #pragma vertex ShadowPassVertex
        #pragma fragment ShadowPassFragment

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "GBuffer"
        Tags{"LightMode" = "UniversalGBuffer"}

        ZWrite[_ZWrite]
        ZTest LEqual
        Cull[_Cull]

        HLSLPROGRAM
        #pragma exclude_renderers gles gles3 glcore
        #pragma target 4.5

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        //#pragma shader_feature _ALPHAPREMULTIPLY_ON
        #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
        #pragma shader_feature_local _NORMALMAP
        #pragma shader_feature_local_fragment _EMISSION
        #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

        // -------------------------------------
        // Universal Pipeline keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile _ _SHADOWS_SOFT
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #pragma vertex LitPassVertexSimple
        #pragma fragment LitPassFragmentSimple
        #define BUMP_SCALE_NOT_SUPPORTED 1

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitGBufferPass.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "DepthOnly"
        Tags{"LightMode" = "DepthOnly"}

        ZWrite On
        ColorMask 0
        Cull[_Cull]

        HLSLPROGRAM
        #pragma exclude_renderers gles gles3 glcore
        #pragma target 4.5

        #pragma vertex DepthOnlyVertex
        #pragma fragment DepthOnlyFragment

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
        ENDHLSL
    }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local _NORMALMAP
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
        ENDHLSL
    }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{ "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaSimple

            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Universal2D"
            Tags{ "LightMode" = "Universal2D" }
            Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
            ENDHLSL
        }
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "SimpleLit" "IgnoreProjector" = "True" "ShaderModel" = "2.0"}
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

        // Use same blending / depth states as Standard shader
        Blend[_SrcBlend][_DstBlend]
        ZWrite[_ZWrite]
        Cull[_Cull]

        HLSLPROGRAM
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma target 2.0

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
        #pragma shader_feature_local_fragment _ _SPECGLOSSMAP _SPECULAR_COLOR
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA
        #pragma shader_feature_local _NORMALMAP
        #pragma shader_feature_local_fragment _EMISSION
        #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

        // -------------------------------------
        // Universal Pipeline keywords
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION


        // -------------------------------------
        // Unity defined keywords
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile_fog

        #pragma vertex LitPassVertexSimple
        #pragma fragment LitPassFragmentSimple
        #define BUMP_SCALE_NOT_SUPPORTED 1

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitForwardPass.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "ShadowCaster"
        Tags{"LightMode" = "ShadowCaster"}

        ZWrite On
        ZTest LEqual
        ColorMask 0
        Cull[_Cull]

        HLSLPROGRAM
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma target 2.0

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

        #pragma vertex ShadowPassVertex
        #pragma fragment ShadowPassFragment

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
        ENDHLSL
    }

    Pass
    {
        Name "DepthOnly"
        Tags{"LightMode" = "DepthOnly"}

        ZWrite On
        ColorMask 0
        Cull[_Cull]

        HLSLPROGRAM
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma target 2.0

        #pragma vertex DepthOnlyVertex
        #pragma fragment DepthOnlyFragment

        // Material Keywords
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
        ENDHLSL
    }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

        // -------------------------------------
        // Material Keywords
        #pragma shader_feature_local _NORMALMAP
        #pragma shader_feature_local_fragment _ALPHATEST_ON
        #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

        //--------------------------------------
        // GPU Instancing
        #pragma multi_compile_instancing

        #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
        ENDHLSL
    }

        // This pass it not used during regular rendering, only for lightmap baking.
        Pass
        {
            Name "Meta"
            Tags{ "LightMode" = "Meta" }

            Cull Off

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMetaSimple

            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitMetaPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "Universal2D"
            Tags{ "LightMode" = "Universal2D" }
            Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
            ENDHLSL
        }
    }
        Fallback "Hidden/Universal Render Pipeline/FallbackError"
        CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.SimpleLitShader"
}
