Shader "Custom/InstancedColorMelt" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)

	}
	SubShader {
		Tags { "RenderType"="Opaque" "LightMode"="UniversalForward" "RenderPipeline"="UniversalPipeline" }
		LOD 200
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			struct Attributes
			{
				float4 positionOS   : POSITION;
				float3 normal		: NORMAL;
			};

			struct Varyings
			{
				float4 positionHCS  : SV_POSITION;
				float3 positionWS	: TEXCOORD0;
				float3 normal		: NORMAL;
			};

			CBUFFER_START(UnityPerMaterial)
				half4 _Color;
			CBUFFER_END

			Varyings vert(Attributes input)
			{
				Varyings output;
				output.positionHCS = TransformObjectToHClip(input.positionOS);
				output.positionWS = TransformObjectToWorld(input.positionOS);
				output.normal = input.normal;
				return output;
			}

			half4 frag(Varyings input) : SV_TARGET
			{
				// Albedo comes from a texture tinted by color
				half4 c = _Color;

				float3 worldPos = input.positionWS * 2.0f;
				clip(c.a - (sin(worldPos.x) * cos(worldPos.y) * sin(worldPos.z) * .5f + .5f));

				Light mainLight = GetMainLight();
				float ndotl = max(dot(input.normal, mainLight.direction), 0.0);
				c.rgb = c.rgb + (c.rgb * mainLight.color * ndotl) + SampleSH(input.normal);

				return c;
			}
			ENDHLSL
		}
	}
}
