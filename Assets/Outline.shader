Shader "Custom/SimpleOutline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0,1,0,1)    // ���� �������
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.02 // ������� �������
    }
    SubShader
    {
        // ������� ������ ������ (�����������), ��� ����������� ������
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Name "Outline"
            Cull Front             // �������� ����������� ��������, ����� ��������� ���� �������
            ZWrite Off             // �� ������ �������
            ZTest Always           // �������� ������
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            float _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                // ������������ ������� ����� �������
                float3 normWorld = UnityObjectToWorldNormal(v.normal);
                v.vertex.xyz += normWorld * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
