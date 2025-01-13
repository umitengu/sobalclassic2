/*
* Copyright 2022-2023 Sony Corporation
*/
/// Shader: Show only shadow
/// reference: https://nuxr.jp/tech-cast-shadow/
Shader "OnlyShadow"
{
    SubShader
    {
        Pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
                "RenderType" = "Opeque"
                "Queue" = "Geometry+1"
                "ForceNoShadowCasting" = "True"
            }

            Blend Zero SrcColor
            ZWrite On
            CGPROGRAM
 
            #include "UnityCG.cginc"
            #include "AutoLight.cginc" 
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                SHADOW_COORDS(1)
            };
 
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }
 
            fixed4 frag(v2f i) : COLOR
            {
                float attenuation = LIGHT_ATTENUATION(i);
                fixed4 rtn = fixed4(1.0 * attenuation ,1.0 * attenuation,1.0 * attenuation ,1.0) ;
                return rtn;
            }

            ENDCG
        }
    }
    Fallback "VertexLit"
}