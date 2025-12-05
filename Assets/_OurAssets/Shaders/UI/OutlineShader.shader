Shader "UI/OutlineSafe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 texcol = tex2D(_MainTex, i.uv) * _Color;

                // Si el pixel actual es opaco → dibujar normalmente
                if (texcol.a > 0.01)
                    return texcol;

                float2 texel = _OutlineSize / _ScreenParams.xy;

                // Chequeo 4 direcciones manualmente
                float4 c1 = tex2D(_MainTex, i.uv + float2( texel.x, 0));
                if (c1.a > 0.01) return _OutlineColor;

                float4 c2 = tex2D(_MainTex, i.uv + float2(-texel.x, 0));
                if (c2.a > 0.01) return _OutlineColor;

                float4 c3 = tex2D(_MainTex, i.uv + float2(0,  texel.y));
                if (c3.a > 0.01) return _OutlineColor;

                float4 c4 = tex2D(_MainTex, i.uv + float2(0, -texel.y));
                if (c4.a > 0.01) return _OutlineColor;

                return float4(0,0,0,0);
            }
            ENDCG
        }
    }
}