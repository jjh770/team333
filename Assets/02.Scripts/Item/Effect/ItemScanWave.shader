Shader "Custom/ItemScanWave"
{
    Properties
    {
        _ScanColor ("Scan Color", Color) = (1, 1, 1, 1)
        _ScanPosition ("Scan Position", Range(0, 1)) = 0
        _ScanWidth ("Scan Width", Range(0.01, 0.5)) = 0.1
        _ScanSoftness ("Scan Softness", Range(0.01, 0.5)) = 0.05
        _Intensity ("Intensity", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent+1"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "ScanWave"

            Blend One One // Additive
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionOS : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ScanColor;
                float _ScanPosition;
                float _ScanWidth;
                float _ScanSoftness;
                float _Intensity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // 3D 대각선 방향 (왼쪽 위 뒤 → 오른쪽 아래 앞)
                // X + Y + Z를 조합하면 3D 공간에서 대각선 축이 됨
                float diagonal = input.positionOS.x - input.positionOS.y + input.positionOS.z;

                // 스캔 위치와의 거리 계산
                float dist = abs(diagonal - _ScanPosition);

                // 스캔 띠 영역 계산 (부드러운 경계)
                float scanBand = 1.0 - smoothstep(_ScanWidth - _ScanSoftness, _ScanWidth + _ScanSoftness, dist);

                // 최종 색상
                half4 color = _ScanColor * scanBand * _Intensity;

                return color;
            }
            ENDHLSL
        }
    }
}
