Shader "Custom/FieldContainer" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Team0Color ("Team 0 Color", Color) = (1,1,1,1)
		_Team1Color ("Team 1 Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_HivePosition ("Hive Position", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Cull Front

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		float _HivePosition;
		fixed4 _Color;
		fixed4 _Team0Color;
		fixed4 _Team1Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v) {
			v.normal=-v.normal;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color;
			c=lerp(c,_Team0Color,smoothstep(-_HivePosition,-_HivePosition-.5f,IN.worldPos.x));
			c=lerp(c,_Team1Color,smoothstep(_HivePosition,_HivePosition+.5f,IN.worldPos.x));
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
