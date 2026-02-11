// Assets/Shaders/PriestVisionFullscreen.shader
//
// Copy-paste ready.
// Change included in this version:
// - Sound blob brightness matches memory brightness:
//   SoundBlobMask multiplies by _MemoryStrength (so strength only affects radius, not brightness).
//
// Assumes you already have the "no roof reveal" height-gate in place (player-height only).
// If your current file has extra stuff, replace the whole shader with this.

Shader "PriestVisionFullscreen"
{
    Properties
    {
        _VisionColor ("Vision Color", Color) = (0.55, 0.60, 0.65, 1.0)

        // ---- OUTLINE ----
        _OutlineIntensity ("Outline Intensity", Range(0, 2)) = 1.0
        _OutlineThickness ("Outline Thickness (px)", Range(0.5, 3.0)) = 1.2

        _DepthEdgeWeight ("Depth Edge Weight", Range(0, 3)) = 1.6
        _DepthEdgeThreshold ("Depth Edge Threshold", Range(0.00001, 0.05)) = 0.0014
        _DepthEdgeFeather ("Depth Edge Feather", Range(0.00001, 0.05)) = 0.0010

        _PlaneReject ("Plane Reject", Range(0.00001, 0.02)) = 0.0018
        _OcclusionBias ("Occlusion Bias", Range(0.0001, 0.05)) = 0.006

        _NormalEdgeWeight ("Normal Edge Weight", Range(0, 1)) = 0.08
        _NormalEdgeThreshold ("Normal Edge Threshold", Range(0.0001, 1.0)) = 0.18
        _NormalEdgeFeather ("Normal Edge Feather", Range(0.0001, 1.0)) = 0.10

        _OutlineClamp ("Outline Clamp", Range(0, 1)) = 0.70

        // ---- DETAIL ----
        _DetailIntensity ("Detail Intensity", Range(0, 2)) = 0.65
        _DetailThickness ("Detail Sample Radius (px)", Range(0.5, 3.0)) = 1.25
        _DetailThreshold ("Detail Threshold", Range(0.0001, 2.0)) = 0.16
        _DetailFeather ("Detail Feather", Range(0.0001, 2.0)) = 0.10
        _DetailFlatAllowance ("Detail Flat Allowance", Range(0, 1)) = 0.10

        _DetailKillStart ("Detail Kill Start", Range(0, 1)) = 0.55
        _DetailKillEnd   ("Detail Kill End",   Range(0, 1)) = 0.85

        _DetailNearDepth ("Detail Near Depth (m)", Range(0.01, 2.0)) = 0.25
        _DetailFarDepth  ("Detail Far Depth (m)",  Range(0.5, 20.0)) = 6.0
        _DetailCloseStepScale ("Close Step Scale", Range(1.0, 6.0)) = 2.4

        // Sense (ELLIPSOID / EGG SHAPE)
        _SenseCenterWS ("Sense Center WS", Vector) = (0,0,0,1)
        _SenseLightDirWS ("Sense Light Dir WS", Vector) = (0,0,1,0)

        _SenseSideRadius ("Sense Side Radius (m)", Float) = 2.0
        _SenseForwardRadius ("Sense Forward Radius (m)", Float) = 3.5
        _SenseBackRadius ("Sense Back Radius (m)", Float) = 0.8
        _SenseUpRadius ("Sense Up Radius (m)", Float) = 0.6
        _SenseDownRadius ("Sense Down Radius (m)", Float) = 2.0

        _SenseSoftness ("Sense Softness (m)", Float) = 0.75

        // ---- MEMORY TRAIL ----
        _MemoryStrength ("Memory Strength", Range(0, 1)) = 0.75
        _MemoryCutoff ("Memory Cutoff", Range(0, 0.2)) = 0.04

        // Height gate for memory + sound (player-height only)
        _MemoryHeightHalfRange ("Memory Height Half Range (m)", Range(0.05, 2.0)) = 0.60
        _MemoryHeightSoftness ("Memory Height Softness (m)", Range(0.0, 1.0)) = 0.10

        // ---- SOUND (BLOB) ----
        _SoundPulseGlobal ("Sound Blob Global", Range(0, 1)) = 1.0
        _SoundBlobExpandSeconds ("Sound Blob Expand Time (s)", Range(0.01, 0.5)) = 0.10
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Overlay" }

        Pass
        {
            Name "PriestVisionFullscreen"
            ZWrite Off
            ZTest Always
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

            TEXTURE2D(_MemoryTex);
            SAMPLER(sampler_MemoryTex);

            float4 _VisionColor;

            float _OutlineIntensity;
            float _OutlineThickness;

            float _DepthEdgeWeight;
            float _DepthEdgeThreshold;
            float _DepthEdgeFeather;

            float _PlaneReject;
            float _OcclusionBias;

            float _NormalEdgeWeight;
            float _NormalEdgeThreshold;
            float _NormalEdgeFeather;

            float _OutlineClamp;

            float _DetailIntensity;
            float _DetailThickness;
            float _DetailThreshold;
            float _DetailFeather;
            float _DetailFlatAllowance;

            float _DetailKillStart;
            float _DetailKillEnd;

            float _DetailNearDepth;
            float _DetailFarDepth;
            float _DetailCloseStepScale;

            float3 _SenseCenterWS;
            float3 _SenseLightDirWS;

            float _SenseSideRadius;
            float _SenseForwardRadius;
            float _SenseBackRadius;
            float _SenseUpRadius;
            float _SenseDownRadius;
            float _SenseSoftness;

            float2 _MemoryWorldOriginXZ;
            float2 _MemoryWorldSizeXZ;
            float _MemoryStrength;
            float _MemoryCutoff;

            float _MemoryHeightHalfRange;
            float _MemoryHeightSoftness;

            float _HasMemory;

            // -------- SOUND BLOBS --------
            #define SOUND_BLOB_MAX 16

            float _SoundPulseCount;            // 0..SOUND_BLOB_MAX
            float _SoundPulseGlobal;           // master knob 0..1
            float _SoundBlobExpandSeconds;     // expand time (seconds)
            float _SoundFadeSeconds;           // fade time (seconds) - set by script to match memory

            // _SoundPulseData[i] = (centerX, centerY, centerZ, startTime)
            float4 _SoundPulseData[SOUND_BLOB_MAX];

            // _SoundPulseParams[i] = (radiusMeters, intensity01, lastHeardTime, edgeSoftnessMeters)
            float4 _SoundPulseParams[SOUND_BLOB_MAX];

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

            inline float LinearDepthFromRaw(float rawDepth)
            {
                return LinearEyeDepth(rawDepth, _ZBufferParams);
            }

            inline float3 WorldPosFromDepth(float2 uv, float rawDepth)
            {
                return ComputeWorldSpacePosition(uv, rawDepth, UNITY_MATRIX_I_VP);
            }

            inline float DepthEdgeRobust_Neighbors(
                float2 uv,
                float2 texel,
                float thicknessPx,
                float dC,
                out float dR, out float dL, out float dU, out float dD)
            {
                float2 o = texel * thicknessPx;

                dR = LinearDepthFromRaw(SampleSceneDepth(uv + float2(o.x, 0)));
                dL = LinearDepthFromRaw(SampleSceneDepth(uv - float2(o.x, 0)));
                dU = LinearDepthFromRaw(SampleSceneDepth(uv + float2(0, o.y)));
                dD = LinearDepthFromRaw(SampleSceneDepth(uv - float2(0, o.y)));

                float norm = max(dC, 0.25);

                float lap = abs((dR + dL + dU + dD) - (4.0 * dC)) / norm;

                float mn = min(min(dR, dL), min(dU, dD));
                float mx = max(max(dR, dL), max(dU, dD));
                float range = (mx - mn) / norm;

                float planeMask = smoothstep(_PlaneReject, _PlaneReject * 2.0, range);

                return lap * planeMask;
            }

            inline float NormalEdge_FromCenter(float2 uv, float2 texel, float thicknessPx, float3 nC)
            {
                float2 o = texel * thicknessPx;

                float3 nR = SampleSceneNormals(uv + float2(o.x, 0));
                float3 nL = SampleSceneNormals(uv - float2(o.x, 0));
                float3 nU = SampleSceneNormals(uv + float2(0, o.y));
                float3 nD = SampleSceneNormals(uv - float2(0, o.y));

                return max(max(length(nC - nR), length(nC - nL)), max(length(nC - nU), length(nC - nD)));
            }

            inline float DetailSignal_FromCenter(float2 uv, float2 texel, float stepPx, float3 nC)
            {
                float2 o = texel * stepPx;

                float3 nR = SampleSceneNormals(uv + float2(o.x, 0));
                float3 nU = SampleSceneNormals(uv + float2(0, o.y));

                return max(length(nC - nR), length(nC - nU));
            }

            inline float SampleMemoryMask(float3 wsPos)
            {
                float2 size = max(abs(_MemoryWorldSizeXZ), float2(1e-4, 1e-4));
                float2 uvM = (wsPos.xz - _MemoryWorldOriginXZ) / size;

                float inBounds =
                    step(0.0, uvM.x) * step(0.0, uvM.y) *
                    step(uvM.x, 1.0) * step(uvM.y, 1.0);

                if (inBounds < 0.5)
                    return 0.0;

                float m = SAMPLE_TEXTURE2D(_MemoryTex, sampler_MemoryTex, uvM).r;

                float cutoff = saturate(_MemoryCutoff);
                m = saturate((m - cutoff) / max(1.0 - cutoff, 1e-4));

                return m;
            }

            inline float LiveSenseMask_Egg(float3 wsPos)
            {
                float3 d = wsPos - _SenseCenterWS;

                float3 fwd = _SenseLightDirWS;
                float fLen = max(length(fwd), 1e-5);
                fwd /= fLen;

                float3 worldUp = float3(0, 1, 0);
                if (abs(dot(fwd, worldUp)) > 0.99)
                    worldUp = float3(0, 0, 1);

                float3 right = normalize(cross(worldUp, fwd));
                float3 up = normalize(cross(fwd, right));

                float x = dot(d, right);
                float y = dot(d, up);
                float z = dot(d, fwd);

                float sideR = max(_SenseSideRadius, 1e-4);
                float foreR = max(_SenseForwardRadius, 1e-4);
                float backR = max(_SenseBackRadius, 1e-4);
                float upR   = max(_SenseUpRadius, 1e-4);
                float downR = max(_SenseDownRadius, 1e-4);

                float ry = (y >= 0.0) ? upR : downR;
                float rz = (z >= 0.0) ? foreR : backR;

                float3 t = float3(x / sideR, y / ry, z / rz);
                float distN = length(t);

                float minAxis = min(sideR, min(ry, rz));
                float softN = clamp(_SenseSoftness / max(minAxis, 1e-4), 1e-5, 0.95);

                return saturate(1.0 - smoothstep(1.0 - softN, 1.0, distN));
            }

            inline float HeightGate_PlayerOnly(float wsY)
            {
                float halfRange = max(_MemoryHeightHalfRange, 0.0001);
                float soft = clamp(_MemoryHeightSoftness, 0.0, halfRange);

                float dy = abs(wsY - _SenseCenterWS.y);

                return 1.0 - smoothstep(halfRange - soft, halfRange, dy);
            }

            inline float SoundBlobMask(float3 wsPos)
            {
                int count = clamp((int)round(_SoundPulseCount), 0, SOUND_BLOB_MAX);

                float heightGate = saturate(HeightGate_PlayerOnly(wsPos.y));
                if (heightGate <= 0.0001)
                    return 0.0;

                float nowT = _Time.y;
                float fadeSec = max(_SoundFadeSeconds, 0.05);
                float expandSec = max(_SoundBlobExpandSeconds, 0.01);

                float best = 0.0;

                [loop]
                for (int i = 0; i < SOUND_BLOB_MAX; i++)
                {
                    if (i >= count)
                        break;

                    float3 c = _SoundPulseData[i].xyz;
                    float startTime = _SoundPulseData[i].w;

                    float radius = max(_SoundPulseParams[i].x, 0.01);
                    float intensity = saturate(_SoundPulseParams[i].y); // expected to be 1.0 from script
                    float lastHeardTime = _SoundPulseParams[i].z;
                    float edgeSoft = max(_SoundPulseParams[i].w, 0.01);

                    // Expand only once from the original start time.
                    float age = nowT - startTime;
                    if (age < 0.0)
                        continue;

                    float expand01 = saturate(age / expandSec);
                    float curR = radius * smoothstep(0.0, 1.0, expand01);

                    // Fade is driven by "time since last refresh".
                    float silentAge = nowT - lastHeardTime;
                    float fade01 = saturate(1.0 - (silentAge / fadeSec));
                    if (fade01 <= 0.0001)
                        continue;

                    float2 dXZ = wsPos.xz - c.xz;
                    float dist = length(dXZ);

                    float disk = 1.0 - smoothstep(max(curR - edgeSoft, 0.0), curR, dist);

                    float m = disk * intensity * fade01;
                    best = max(best, m);
                }

                // Master knobs (and THE important change):
                best *= saturate(_SoundPulseGlobal);

                // Make sound blob brightness match memory brightness.
                best *= saturate(_MemoryStrength);

                best *= heightGate;
                return saturate(best);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                const float4 kBlack = float4(0, 0, 0, 1);

                float2 uv = input.uv;
                float rawDepthC = SampleSceneDepth(uv);

                #if defined(UNITY_REVERSED_Z)
                    if (rawDepthC <= 0.000001)
                        return kBlack;
                #else
                    if (rawDepthC >= 0.999999)
                        return kBlack;
                #endif

                float dC = LinearDepthFromRaw(rawDepthC);
                float3 wsPos = WorldPosFromDepth(uv, rawDepthC);

                float liveMask = LiveSenseMask_Egg(wsPos);

                float memoryMask = 0.0;
                if (_HasMemory > 0.5 && _MemoryStrength > 0.0001)
                {
                    memoryMask = SampleMemoryMask(wsPos) * saturate(_MemoryStrength);
                    memoryMask *= saturate(HeightGate_PlayerOnly(wsPos.y));
                }

                float soundMask = SoundBlobMask(wsPos);

                float mask = max(liveMask, max(memoryMask, soundMask));

                float2 texel = rcp(_ScreenParams.xy);

                // ---- OUTLINE ----
                float dR, dL, dU, dD;
                float depthEdgeMag = DepthEdgeRobust_Neighbors(uv, texel, _OutlineThickness, dC, dR, dL, dU, dD);

                float edgeDepth = smoothstep(_DepthEdgeThreshold, _DepthEdgeThreshold + _DepthEdgeFeather, depthEdgeMag) * _DepthEdgeWeight;

                float minN = min(min(dR, dL), min(dU, dD));
                float occluded = step(minN, dC - _OcclusionBias);
                float foreGate = 1.0 - occluded;

                edgeDepth *= foreGate;

                float edge = edgeDepth;

                float3 nC = SampleSceneNormals(uv);

                if (_NormalEdgeWeight > 0.0001)
                {
                    float nEdgeMag = NormalEdge_FromCenter(uv, texel, _OutlineThickness, nC);
                    float edgeNorm = smoothstep(_NormalEdgeThreshold, _NormalEdgeThreshold + _NormalEdgeFeather, nEdgeMag) * _NormalEdgeWeight;
                    edge += edgeNorm * foreGate;
                }

                edge = saturate(edge);
                edge = min(edge, _OutlineClamp);

                // ---- DETAIL ----
                float depth01 = saturate((dC - _DetailNearDepth) / max(_DetailFarDepth - _DetailNearDepth, 1e-4));
                float closeScale = lerp(_DetailCloseStepScale, 1.0, depth01);
                float detailStepPx = _DetailThickness * closeScale;

                float dSig = DetailSignal_FromCenter(uv, texel, detailStepPx, nC);
                float detail = smoothstep(_DetailThreshold, _DetailThreshold + _DetailFeather, dSig);

                float anchor = max(saturate(edge * 1.25), _DetailFlatAllowance);
                detail *= anchor;

                float edgeKill = smoothstep(_DetailKillStart, _DetailKillEnd, edge);
                detail *= (1.0 - edgeKill);

                // ---- COLOR ----
                float3 c = _VisionColor.rgb;

                float3 outCol = 0.0;
                outCol += c * (edge * _OutlineIntensity);
                outCol += c * (detail * _DetailIntensity);

                outCol *= mask;

                return half4(outCol, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
