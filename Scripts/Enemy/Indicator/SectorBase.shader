Shader "Custom/SectorBase"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Angle ("Angle (Degrees)", Range(0,360)) = 90
        _Radius ("Radius", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
            float _Radius;

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

            // 중심각 기준 차이 계산 함수
            float AngleDiff(float a, float b)
            {
                float diff = abs(a - b);
                return min(diff, 360.0 - diff);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; // 중심 기준 [-1,1] 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float dist = length(uv);

                // 현재 벡터의 각도 (0~360도)
                float angle = degrees(atan2(uv.y, uv.x));
                if (angle < 0.0) angle += 360.0;

                // 중심 기준 대칭 각도 계산
                float halfAngle = _Angle * 0.5;
                float deltaAngle = AngleDiff(angle, 0.0); // 0도 중심 기준

                // 반경 또는 각도 범위를 초과하면 버림
                if (deltaAngle > halfAngle || dist > _Radius)
                    discard;

                return _Color;
            }
            ENDCG
        }
    }
}
