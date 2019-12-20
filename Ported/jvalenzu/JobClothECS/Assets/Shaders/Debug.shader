Shader "Debug/Debug"
{
  Properties
  {
    _MainTex ("MainTex (RGB)", 2D) = "white" {}
  }
  SubShader
  {
    Lighting Off

    Pass
    {
      CGPROGRAM
            
#pragma vertex vert
#pragma fragment frag
#pragma target 5.0

#include "UnityCG.cginc"
#include "UnityStandardCore.cginc"
#pragma multi_compile _ STENCIL_ON

      struct vertOut
      {
        float4 m_Pos:SV_POSITION;
        float4 m_Color:COLOR;
      };

      vertOut vert(appdata_base v)
      {
        vertOut o;
        o.m_Pos = UnityObjectToClipPos(v.vertex);
        // o.m_Color = float4(0.5f * (float3(1,1,1)+ v.normal.xyz), 1);
        if (v.texcoord.x >= 0.75f)
          o.m_Color = float4(1,0,0,1);
        else if (v.texcoord.x >= 0.5f)
          o.m_Color = float4(0,1,0,1);
        else if (v.texcoord.x >= 0.25f)
          o.m_Color = float4(0,0,1,1);
        else
          o.m_Color = float4(0,0,0,1);
        return o;
      }

      float4 frag(vertOut IN):SV_Target
      {
        return IN.m_Color;
      }

      ENDCG
     }
  }
}
