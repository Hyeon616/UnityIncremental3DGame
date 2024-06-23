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
        _FlowColor ("Flow Color", Color) = (1, 1, 1, 1) // 添加 Flow Color 属性用于叠加 _FlowTexture 的颜色
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
                float2 flowUV : TEXCOORD1; // 新增的用于_FlowTexture 的UV坐标
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
            fixed4 _FlowColor; // 添加 _FlowColor 属性

            v2f vert(appdata_t input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
               
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);


                // 计算UV坐标相对于图像中心的偏移量
                float2 offset = input.uv - float2(0.5, 0.5);

                // 设置旋转角度（以弧度为单位）
                float rotationAngle = radians(_Time.y * _RotSpeed);

                // 计算旋转后的UV坐标
                float2 rotatedUV;
                rotatedUV.x = offset.x * cos(rotationAngle) - offset.y * sin(rotationAngle);
                rotatedUV.y = offset.x * sin(rotationAngle) + offset.y * cos(rotationAngle);

                // 将旋转后的UV坐标加上图像中心的坐标，得到最终UV坐标
               // output.flowUV =rotatedUV + float2(0.5, 0.5) ;
                
                output.flowUV = TRANSFORM_TEX(rotatedUV + float2(0.5, 0.5), _FlowTexture);
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, input.uv);
                fixed4 flowColor = tex2D(_FlowTexture, input.flowUV) * _FlowColor; // 使用 _FlowColor 属性来叠加 _FlowTexture 的颜色

                float flow = _FlowStrength * (sin(input.flowUV.x) + cos(input.flowUV.y));

                // 计算最终的流光效果
                flow = saturate(flow); // 将流光效果限制在[0, 1]范围内
                flow = lerp(_MinBrightness, _MaxBrightness, flow); // 根据最暗和最亮的限制设置流光的亮度范围

                fixed4 finalColor = baseColor + flowColor * flow;
                finalColor.a = baseColor.a; // 保持透明度不变

                return finalColor;
            }
            ENDCG
        }
    }
}