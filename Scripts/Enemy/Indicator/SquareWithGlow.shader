Shader "Custom/GlowOnly"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _GlowWidth ("Glow Width", Range(0.01, 0.3)) = 0.1
        _GlowSpeed ("Glow Speed", Float) = 1.0
        _GlowDirection ("Glow Direction", Vector) = (0, -1, 0, 0)
        _Range ("Fill Height", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _GlowWidth;
            float _GlowSpeed;
            float4 _GlowDirection;
            float _Range;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // UV를 (x: -1 ~ 1, y: 0 ~ 2)로 변경한 후 아래를 기준으로 y = 0이 되도록 정규화
                float2 uv = v.uv * 2.0 - 1.0;
                uv.y = (uv.y + 1.0) * 0.5; // [0,1]로 변환 (아래 기준)

                o.uv = uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 높이 초과 영역은 제거 (Fill Amount처럼 동작)
                if (uv.y > _Range)
                    discard;

                float2 dir = normalize(_GlowDirection.xy);
                float2 centeredUV = float2(uv.x, uv.y * 2.0 - 1.0); // 다시 중심 기준으로 (-1 ~ 1) 변환
                float glowPos = dot(centeredUV, dir);

                float glowCenter = fmod(_Time.y * _GlowSpeed, 2.0) - 1.0;
                float glow = smoothstep(glowCenter + _GlowWidth, glowCenter, glowPos) *
                             smoothstep(glowCenter - _GlowWidth, glowCenter, glowPos);

                float alpha = _Color.a + glow;
                fixed3 finalColor = _Color.rgb + glow;

                return fixed4(finalColor, saturate(alpha));
            }
            ENDCG
        }
    }
}
