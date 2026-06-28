Shader "Custom/Wake"
{
    SubShader
    {
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
        ENDHLSL

        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "FullScreen"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            float4 _WakeShader_StartWorldPos;
            float4 _WakeShader_EndWorldPos;
            float _WakeShader_Radius;
            float _WakeShader_DistortionAmplitude;
            float _WakeShader_DistortionFrequency;
            float _WakeShader_DistortionSpeed;
            Texture2D _WakeShader_WakeTexture;

            float DistanceToSegment(float3 p, float3 a, float3 b, out float t)
            {
                float3 pa = p - a;
                float3 ba = b - a;
                t = saturate(dot(pa, ba) / dot(ba, ba));
                return length(pa - ba * t);
            }

            float SmoothNoise2D(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = frac(sin(dot(i + float2(0,0), float2(127.1, 311.7))) * 43758.5453123);
                float b = frac(sin(dot(i + float2(1,0), float2(127.1, 311.7))) * 43758.5453123);
                float c = frac(sin(dot(i + float2(0,1), float2(127.1, 311.7))) * 43758.5453123);
                float d = frac(sin(dot(i + float2(1,1), float2(127.1, 311.7))) * 43758.5453123);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float4 Frag (Varyings input) : SV_Target
            {
                float2 UV = input.positionCS.xy / _ScaledScreenParams.xy;
                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(UV);
                #else
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(UV));
                #endif
                float3 worldPos = ComputeWorldSpacePosition(UV, depth, UNITY_MATRIX_I_VP);

                float t;
                float dist = DistanceToSegment(worldPos, _WakeShader_StartWorldPos, _WakeShader_EndWorldPos, t);

                float2 noiseUV = float2(t * _WakeShader_DistortionFrequency, _Time.y * _WakeShader_DistortionSpeed);
                float noise = SmoothNoise2D(noiseUV) * 2.0 - 1.0;

                float distortedRadius = _WakeShader_Radius + (noise * _WakeShader_DistortionAmplitude);

                float4 sceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgba;
                float4 wakeColor = SAMPLE_TEXTURE2D(_WakeShader_WakeTexture, sampler_LinearClamp, input.texcoord).rgba;
                return lerp(sceneColor, wakeColor, saturate(distortedRadius - dist));
            }

            ENDHLSL
        }
    }
}
