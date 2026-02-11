// Assets/Shaders/Hidden_PriestVisionMemoryTrailBlit.shader
Shader "Hidden/PriestVisionMemoryTrailBlit"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Overlay" }

        // PASS 0: Sense paint + fade (your existing behavior)
        Pass
        {
            Name "MemoryTrailBlit"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_PrevTex);
            SAMPLER(sampler_PrevTex);

            float2 _PaintUV;
            float2 _PaintDirUV;

            // x = side radius, y = forward radius, z = back radius
            float4 _PaintRadiiUV;

            // x = side softness, y = forward softness, z = back softness
            float4 _PaintSoftnessUV;

            float _FadeMul; // 0..1
            float _Fade;    // 0..1

            struct Attributes { uint vertexID : SV_VertexID; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                float2 uv = float2((v.vertexID << 1) & 2, v.vertexID & 2);
                o.uv = uv;
                o.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                if (_ProjectionParams.x < 0.0)
                    o.uv.y = 1.0 - o.uv.y;
                #endif
                return o;
            }

            float SoftOrientedEgg(float2 uv, float2 center, float2 dirUV, float3 radii, float3 softness)
            {
                float2 d = uv - center;

                float2 f = dirUV;
                float fLen = max(length(f), 1e-5);
                f /= fLen;

                float2 r = float2(-f.y, f.x);

                float u = dot(d, r);
                float v = dot(d, f);

                float sideR = max(radii.x, 1e-5);
                float foreR = max(radii.y, 1e-5);
                float backR = max(radii.z, 1e-5);

                float rv = (v >= 0.0) ? foreR : backR;

                float2 t = float2(u / sideR, v / rv);
                float dist = length(t);

                float sideS = max(softness.x, 0.0);
                float foreS = max(softness.y, 0.0);
                float backS = max(softness.z, 0.0);

                float sv = (v >= 0.0) ? foreS : backS;

                float softN = max(max(sideS / sideR, sv / rv), 1e-5);

                return 1.0 - smoothstep(1.0 - softN, 1.0, dist);
            }

            half4 Frag(Varyings i) : SV_Target
            {
                float prev = SAMPLE_TEXTURE2D(_PrevTex, sampler_PrevTex, i.uv).r;

                // Fade existing memory.
                prev = saturate(prev * saturate(_FadeMul));
                prev = saturate(prev - saturate(_Fade));

                float3 radii = _PaintRadiiUV.xyz;
                float3 soft  = _PaintSoftnessUV.xyz;

                float paint = SoftOrientedEgg(i.uv, _PaintUV, _PaintDirUV, radii, soft);

                float outMask = max(prev, paint);
                return half4(outMask, outMask, outMask, 1.0);
            }
            ENDHLSL
        }

        // PASS 1: Sound stamp paint ONLY (no fade)
        Pass
        {
            Name "SoundStampBlit"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment FragSound

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_PrevTex);
            SAMPLER(sampler_PrevTex);

            float2 _SoundCenterUV;

            // Ellipse radii in UV that represent a WORLD circle (accounts for non-square world rect).
            float2 _SoundRadiiUV;

            // Softness in UV (same axis scaling as radii)
            float2 _SoundSoftnessUV;

            // How strongly this stamp writes into memory (0..1)
            float _SoundStampStrength;

            struct Attributes { uint vertexID : SV_VertexID; };
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                float2 uv = float2((v.vertexID << 1) & 2, v.vertexID & 2);
                o.uv = uv;
                o.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);

                #if UNITY_UV_STARTS_AT_TOP
                if (_ProjectionParams.x < 0.0)
                    o.uv.y = 1.0 - o.uv.y;
                #endif
                return o;
            }

            float SoftWorldCircleInUV(float2 uv, float2 center, float2 radiiUV, float2 softUV)
            {
                float2 d = uv - center;

                float rx = max(radiiUV.x, 1e-6);
                float ry = max(radiiUV.y, 1e-6);

                float2 t = float2(d.x / rx, d.y / ry);
                float dist = length(t); // 0..1 inside

                float sx = max(softUV.x, 0.0);
                float sy = max(softUV.y, 0.0);

                // Convert softness into a normalized feather in "dist" space.
                float softN = max(max(sx / rx, sy / ry), 1e-6);

                return 1.0 - smoothstep(1.0 - softN, 1.0, dist);
            }

            half4 FragSound(Varyings i) : SV_Target
            {
                float prev = SAMPLE_TEXTURE2D(_PrevTex, sampler_PrevTex, i.uv).r;

                float paint = SoftWorldCircleInUV(i.uv, _SoundCenterUV, _SoundRadiiUV, _SoundSoftnessUV);
                paint *= saturate(_SoundStampStrength);

                float outMask = max(prev, paint);
                return half4(outMask, outMask, outMask, 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
