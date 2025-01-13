Shader "Sony/Composite/CompositeBackgroundUI"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _BackgroundRect ("BackgroundRect", Vector) = (0, 0, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-500" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _BackgroundRect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 pivot = fixed3 (0.5, 0.5, 0.0);

                float3x3 posisionMatrix = {
                  1, 0, 0,
                  0, 1, 0,
                  _BackgroundRect.x, _BackgroundRect.y, 1
                };

                float3x3 scaleMatrix = {
                  1/_BackgroundRect.z, 0, 0,
                  0, 1/_BackgroundRect.z, 0,
                  0, 0, 1
                };

                float u = (i.uv.x * _ScreenParams.x * _MainTex_TexelSize.x);
                float v = (i.uv.y * _ScreenParams.y * _MainTex_TexelSize.y);
                
                float3 uv = float3(u, v, 1);
               
                float3 mulUv = mul(uv, posisionMatrix) - pivot;
                mulUv = mul(mulUv,  scaleMatrix);
                uv = mulUv + pivot;

                fixed4 col = fixed4(0, 0, 0, 1);
                if (uv.x >= 0 && uv.x <= 1 && uv.y >= 0 && uv.y <= 1)
                {
                    col = tex2D(_MainTex, uv);
                }

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
