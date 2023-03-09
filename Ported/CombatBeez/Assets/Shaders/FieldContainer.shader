Shader "Custom/FieldContainer" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Team0Color ("Team 0 Color", Color) = (1,1,1,1)
		_Team1Color ("Team 1 Color", Color) = (1,1,1,1)
		_HivePosition ("Hive Position", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "LightMode"="UniversalForward" "RenderPipeline"="UniversalPipeline"}
		LOD 200
		Cull Front

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
				float _HivePosition;
				half4 _Color;
				half4 _Team0Color;
				half4 _Team1Color;
			CBUFFER_END

			Varyings vert(Attributes input) {
				Varyings output;
				output.positionHCS = TransformObjectToHClip(input.positionOS);
				output.positionWS = TransformObjectToWorld(input.positionOS);
				output.normal = -input.normal;
				return output;
			}

			half4 frag(Varyings input) : SV_TARGET
			{
				// Albedo comes from a texture tinted by color
				half4 c = _Color;
				c = lerp(c,_Team0Color,smoothstep(-_HivePosition,-_HivePosition - .5f, input.positionWS.x));
				c = lerp(c,_Team1Color,smoothstep(_HivePosition,_HivePosition + .5f, input.positionWS.x));

				Light mainLight = GetMainLight();
				float ndotl = max(dot(input.normal, mainLight.direction), 0.0);
				c.rgb = c.rgb + (c.rgb * mainLight.color * ndotl) + SampleSH(input.normal);

				return half4(c.rgb, 1.0);
			}
			ENDHLSL
		}
	}
}
