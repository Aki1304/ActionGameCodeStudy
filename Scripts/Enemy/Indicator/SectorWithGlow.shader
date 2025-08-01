Shader "Custom/SectorWithGlow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Angle ("Angle (Degrees)", Range(0,360)) = 90
        _Range ("Radius", Range(0, 1)) = 0.5
        _GlowWidth ("Glow Width", Range(0.01, 0.3)) = 0.1
        _GlowSpeed ("Glow Speed", Float) = 1.0
        _GlowDirection ("Glow Direction", Vector) = (0, -1, 0, 0)
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
            float _Angle;
            float _Range;
            float _GlowWidth;
            float _GlowSpeed;
            float4 _GlowDirection; // xy만 사용

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

            float AngleDiff(float a, float b)
            {
                float diff = abs(a - b);
                return min(diff, 360.0 - diff);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; // 중심 기준 [-1,1]로 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 거리 및 각도 검사
                float dist = length(uv);
                float angle = degrees(atan2(uv.y, uv.x));
                if (angle < 0.0) angle += 360.0;

                float halfAngle = _Angle * 0.5;
                float deltaAngle = AngleDiff(angle, 0.0);

                if (deltaAngle > halfAngle || dist > _Range)
                    discard;

                // 글로우 방향 계산
                float2 dir = normalize(_GlowDirection.xy);
                float glowPos = dot(uv, dir); // 현재 uv에서 방향으로의 투영값

                float glowCenter = fmod(_Time.y * _GlowSpeed, 2.0) - 1.0; // [-1,1] 주기
                float glow = smoothstep(glowCenter + _GlowWidth, glowCenter, glowPos) *
                             smoothstep(glowCenter - _GlowWidth, glowCenter, glowPos);

                // 최종 출력
                float alpha = _Color.a + glow;
                fixed3 finalColor = _Color.rgb + glow;

                return fixed4(finalColor, saturate(alpha));
            }
            ENDCG
        }
    }
}
