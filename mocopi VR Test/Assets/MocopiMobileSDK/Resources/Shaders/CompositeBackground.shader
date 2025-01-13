Shader "Hidden/Sony/Composite/CompositeBackground"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
        _BackgroundTex ("Background", 2D) = "black" {}
        _BackgroundRect ("BackgroundRect", Vector) = (0, 0, 1, 1)
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off 
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BackgroundTex;
            float4 _BackgroundTex_TexelSize;
            float4 _BackgroundRect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
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

                float bgU = (i.uv.x * _ScreenParams.x * _BackgroundTex_TexelSize.x);
                float bgV = (i.uv.y * _ScreenParams.y * _BackgroundTex_TexelSize.y);
                
                float3 bgUv = float3(bgU, bgV, 1);
               
                float3 mulUv = mul(bgUv, posisionMatrix) - pivot;
                mulUv = mul(mulUv,  scaleMatrix);
                bgUv = mulUv + pivot;

                fixed4 bg = fixed4(0, 0, 0, 1);
                if (bgUv.x >= 0 && bgUv.x <= 1 && bgUv.y >= 0 && bgUv.y <= 1)
                {
                    bg = tex2D(_BackgroundTex, bgUv);
                }

                col = (col.a == 0 && !(col.r == 1 && col.g == 1 && col.b == 1)) ? float4(0, 0, 0, 1 - col.r) : col;
                col.rgb = lerp(bg.rgb, col.rgb, col.a);

                //fixed alpha_threshold = 0.5;
                //col.rgb = lerp(bg.rgb, col.rgb, col.a > alpha_threshold? 1 : lerp(0, 1, col.a / alpha_threshold));

                //col.rgb = lerp(bg.rgb, col.rgb, col.a > 0? 1 : 0);
                
                return col;
            }

            ENDCG
        }
    }
}
