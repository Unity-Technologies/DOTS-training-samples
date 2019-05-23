Shader "Custom/PitFresnel" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_FadeDepth ("Fade Depth", Float) = 50
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Lighting Off
		Cull Off

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float3 worldPos;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		half _FadeDepth;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
			o.Emission = clamp(IN.worldPos.y/_FadeDepth,-1,0);

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = float3(0,0,1);

			half fresnel = dot(IN.viewDir,o.Normal)+sin((IN.worldPos.x+IN.worldPos.y+IN.worldPos.z)*2.f)*.2f;
			fresnel-=clamp(1.f-length(_WorldSpaceCameraPos-IN.worldPos)/20.f,0,1);
			fresnel*=clamp(-.5-IN.worldPos.y,0,1);
			clip(fresnel);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
