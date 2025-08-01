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

            // �߽ɰ� ���� ���� ��� �Լ�
            float AngleDiff(float a, float b)
            {
                float diff = abs(a - b);
                return min(diff, 360.0 - diff);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2.0 - 1.0; // �߽� ���� [-1,1] ��ȯ
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float dist = length(uv);

                // ���� ������ ���� (0~360��)
                float angle = degrees(atan2(uv.y, uv.x));
                if (angle < 0.0) angle += 360.0;

                // �߽� ���� ��Ī ���� ���
                float halfAngle = _Angle * 0.5;
                float deltaAngle = AngleDiff(angle, 0.0); // 0�� �߽� ����

                // �ݰ� �Ǵ� ���� ������ �ʰ��ϸ� ����
                if (deltaAngle > halfAngle || dist > _Radius)
                    discard;

                return _Color;
            }
            ENDCG
        }
    }
}
