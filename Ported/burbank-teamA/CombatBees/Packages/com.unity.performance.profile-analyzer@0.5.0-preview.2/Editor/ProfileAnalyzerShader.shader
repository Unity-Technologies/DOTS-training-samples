Shader "Unlit/ProfileAnalyzerShader"
{
	Properties
	{
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        _ClipRect ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	}
    
	SubShader
	{
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
		//Tags { "RenderType"="Transparent" }
		LOD 100

        //ZWrite Off
        //Blend SrcAlpha OneMinusSrcAlpha
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
           
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

			struct appdata
			{
				float4 vertex : POSITION;
                fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 worldPosition : TEXCOORD1;
			};
            
            bool _UseClipRect;
            float4 _ClipRect;

            v2f vert (appdata v)
			{
				v2f o;
                o.worldPosition = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.color.rgba = v.color;
				return o;
			}

            //fixed4 frag (v2f i) : SV_Target { return i.color; }
            
            fixed4 frag (v2f i) : SV_Target
            {
                half4 color = i.color;
             
                #ifdef UNITY_UI_CLIP_RECT
                if (_UseClipRect)
                    color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif
             
                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
 
                return color;
            }
            
			ENDCG
		}
	}
}
