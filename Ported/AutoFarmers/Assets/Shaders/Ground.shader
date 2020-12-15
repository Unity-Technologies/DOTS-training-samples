Shader "Custom/Ground" 
{
    Properties
    {
       [NoScaleOffset] _BaseMap("Albedo", 2D) = "white" {}
       [NoScaleOffset] _TilledTex("Tilled", 2D) = "white" {}
       _BaseColor("Color", Color) = (1,1,1,1)
    }
        SubShader
       {
           Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }

           Pass
           {
               Name "Lit"
               Tags{"LightMode" = "UniversalForward"}
               HLSLPROGRAM
               #pragma vertex vert
               #pragma fragment frag
               #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

               struct Attributes
               {
                   float4 vertex           : POSITION;
                   float3 normal           : NORMAL;
                   float2 uv               : TEXCOORD0;
               };

               struct Varyings
               {
                   float2 uv               : TEXCOORD0;
                   float4 vertex           : SV_POSITION;
                   half4 color             : COLOR;
               };

               Varyings vert(Attributes input)
               {
                   Varyings o;

                   VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
                   o.vertex = vertexInput.positionCS;

                   VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normal);
                   half3 worldNormal = normalInputs.normalWS;
                   half normalLight = max(0, dot(worldNormal, _MainLightPosition.xyz));
                   o.color = normalLight * (_MainLightColor);
                   o.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                   return o;
               }
               
               TEXTURE2D(_TilledTex);       
               SAMPLER(sampler_TilledTex);

               UNITY_INSTANCING_BUFFER_START(Props)
                   UNITY_DEFINE_INSTANCED_PROP(float, _Tilled)
               UNITY_INSTANCING_BUFFER_END(Props)

               half4 frag(Varyings input) : SV_Target
               {
                   half4 tex1 = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                   half4 tex2 = SAMPLE_TEXTURE2D(_TilledTex, sampler_TilledTex, input.uv);
                   half4 col = lerp(tex1, tex2, UNITY_ACCESS_INSTANCED_PROP(Props, _Tilled)) * _BaseColor;
                   col *= input.color;
                   return col;
               }
               ENDHLSL
           }
       }
}