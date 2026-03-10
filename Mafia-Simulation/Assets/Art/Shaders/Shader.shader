Shader "Hidden/PSX/Fullscreen"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _PixelScale ("Pixel Scale", Float) = 240
        _PosterizeSteps ("Posterize Steps", Float) = 32
        _DitherStrength ("Dither Strength", Range(0,1)) = 0.35
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.05
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Overlay" }
        ZWrite Off
        ZTest Always
        Cull Off
        Blend Off

        Pass
        {
            Name "Shader"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _PixelScale;
            float _PosterizeSteps;
            float _DitherStrength;
            float _NoiseStrength;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float Bayer4x4(int2 p)
            {
                int x = p.x & 3;
                int y = p.y & 3;

                int v =
                    (y == 0) ? ((x == 0) ? 0  : (x == 1) ? 8  : (x == 2) ? 2  : 10) :
                    (y == 1) ? ((x == 0) ? 12 : (x == 1) ? 4  : (x == 2) ? 14 : 6 ) :
                    (y == 2) ? ((x == 0) ? 3  : (x == 1) ? 11 : (x == 2) ? 1  : 9 ) :
                               ((x == 0) ? 15 : (x == 1) ? 7  : (x == 2) ? 13 : 5 );

                return (v / 16.0);
            }

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float4 Frag (Varyings IN) : SV_Target
            {
                float2 screenPx = IN.uv * _ScreenParams.xy;

                float px = max(_PixelScale, 8.0);
                float2 grid = (_ScreenParams.xy / px);

                float2 snapped = floor(screenPx / grid) * grid;
                float2 uvSnap = snapped / _ScreenParams.xy;

                float3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvSnap).rgb;

                float n = Hash21(uvSnap * _ScreenParams.xy);
                col += (n - 0.5) * _NoiseStrength;

                float d = Bayer4x4(int2(screenPx));
                col += (d - 0.5) * _DitherStrength / max(_PosterizeSteps, 2.0);

                float steps = max(_PosterizeSteps, 2.0);
                col = floor(col * steps) / steps;

                return float4(saturate(col), 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}