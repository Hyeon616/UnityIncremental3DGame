Shader "UUIFX/RotateFlowingUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlowTexture ("Flow Texture", 2D) = "white" {}
        _RotSpeed ("Rotate Speed", float) = 180
        _FlowStrength ("Flow Strength", Range(0, 1)) = 0.5
        _MinBrightness ("Min Brightness", Range(0, 1)) = 0.1
        _MaxBrightness ("Max Brightness", Range(0, 1)) = 0.9
        _FlowColor ("Flow Color", Color) = (1, 1, 1, 1) // ��� Flow Color �������ڵ��� _FlowTexture ����ɫ
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 flowUV : TEXCOORD1; // ����������_FlowTexture ��UV����
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _FlowTexture;
            float4 _MainTex_ST;
            float4 _FlowTexture_ST;
            float _RotSpeed;
            float _FlowStrength;
            float _MinBrightness;
            float _MaxBrightness;
            fixed4 _FlowColor; // ��� _FlowColor ����

            v2f vert(appdata_t input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
               
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);


                // ����UV���������ͼ�����ĵ�ƫ����
                float2 offset = input.uv - float2(0.5, 0.5);

                // ������ת�Ƕȣ��Ի���Ϊ��λ��
                float rotationAngle = radians(_Time.y * _RotSpeed);

                // ������ת���UV����
                float2 rotatedUV;
                rotatedUV.x = offset.x * cos(rotationAngle) - offset.y * sin(rotationAngle);
                rotatedUV.y = offset.x * sin(rotationAngle) + offset.y * cos(rotationAngle);

                // ����ת���UV�������ͼ�����ĵ����꣬�õ�����UV����
               // output.flowUV =rotatedUV + float2(0.5, 0.5) ;
                
                output.flowUV = TRANSFORM_TEX(rotatedUV + float2(0.5, 0.5), _FlowTexture);
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, input.uv);
                fixed4 flowColor = tex2D(_FlowTexture, input.flowUV) * _FlowColor; // ʹ�� _FlowColor ���������� _FlowTexture ����ɫ

                float flow = _FlowStrength * (sin(input.flowUV.x) + cos(input.flowUV.y));

                // �������յ�����Ч��
                flow = saturate(flow); // ������Ч��������[0, 1]��Χ��
                flow = lerp(_MinBrightness, _MaxBrightness, flow); // �����������������������������ȷ�Χ

                fixed4 finalColor = baseColor + flowColor * flow;
                finalColor.a = baseColor.a; // ����͸���Ȳ���

                return finalColor;
            }
            ENDCG
        }
    }
}