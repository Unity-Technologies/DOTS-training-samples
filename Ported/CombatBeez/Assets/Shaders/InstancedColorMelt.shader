Shader "Custom/InstancedColorMelt" {
	Properties {
		_BaseColor ("Color", Color) = (1,1,1,1)

	}
	SubShader {
		Tags { "RenderType"="Opaque" "LightMode"="UniversalForward" "RenderPipeline"="UniversalPipeline" }
		LOD 200
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float3 normal		: NORMAL;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionHCS  : SV_POSITION;
				float3 positionWS	: TEXCOORD0;
				float3 normal		: NORMAL;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			CBUFFER_START(UnityPerMaterial)
				half4 _BaseColor;
			CBUFFER_END
			#ifdef UNITY_DOTS_INSTANCING_ENABLED
				UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
				UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
				UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

			#define _BaseColor          UNITY_ACCESS_DOTS_INSTANCED_PROP_WITH_DEFAULT(float4 , _BaseColor)
			#endif

			Varyings vert(Attributes input)
			{
				Varyings output;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);

				output.positionHCS = TransformObjectToHClip(input.positionOS);
				output.positionWS = TransformObjectToWorld(input.positionOS);
				output.normal = input.normal;
				return output;
			}

			half4 frag(Varyings input) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);

				// Albedo comes from a texture tinted by color
				half4 c = _BaseColor;

				float3 worldPos = input.positionWS * 2.0f;
				clip(c.a - (sin(worldPos.x) * cos(worldPos.y) * sin(worldPos.z) * .5f + .5f));

				Light mainLight = GetMainLight();
				float ndotl = max(dot(input.normal, mainLight.direction), 0.0);
				c.rgb = (c.rgb * mainLight.color * ndotl) + SampleSH(input.normal);

				return c;
			}
			ENDHLSL
		}
	}
}
