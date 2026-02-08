Shader "Hidden/PriestVisionFullscreen"
{
    Properties
    {
        _VisionColor ("Vision Color", Color) = (0.55, 0.60, 0.65, 1.0)

        // ---- OUTLINE ----
        _OutlineIntensity ("Outline Intensity", Range(0, 2)) = 1.0
        _OutlineThickness ("Outline Thickness (px)", Range(0.5, 3.0)) = 1.2

        // Depth edge (robust local test)
        _DepthEdgeWeight ("Depth Edge Weight", Range(0, 3)) = 1.6
        _DepthEdgeThreshold ("Depth Edge Threshold", Range(0.00001, 0.05)) = 0.0014
        _DepthEdgeFeather ("Depth Edge Feather", Range(0.00001, 0.05)) = 0.0010

        // Prevent smooth/tilted surfaces from becoming "edges"
        _PlaneReject ("Plane Reject", Range(0.00001, 0.02)) = 0.0018

        // Foreground-only gate bias (meters in linear eye depth space-ish).
        // Raise this to suppress more "look-through" glow.
        _OcclusionBias ("Occlusion Bias", Range(0.0001, 0.05)) = 0.006

        // Optional tiny normal edge for corners (keep low)
        _NormalEdgeWeight ("Normal Edge Weight", Range(0, 1)) = 0.08
        _NormalEdgeThreshold ("Normal Edge Threshold", Range(0.0001, 1.0)) = 0.18
        _NormalEdgeFeather ("Normal Edge Feather", Range(0.0001, 1.0)) = 0.10

        // Clamp so outlines never go white
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

        // Sense
        _SenseCenterWS ("Sense Center WS", Vector) = (0,0,0,1)
        _SenseRadius ("Sense Radius", Float) = 3.0
        _SenseSoftness ("Sense Softness", Float) = 2.6
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
            float _SenseRadius;
            float _SenseSoftness;

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

                // Keep UV correct on platforms where UV origin flips.
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

            // Robust depth edge that:
            // - samples only the NEIGHBORS (center depth is supplied by caller to save a depth fetch)
            // - returns an edge magnitude
            // - outputs neighbor linear depths for occlusion gating
            inline float DepthEdgeRobust_Neighbors(
                float2 uv,
                float2 texel,
                float thicknessPx,
                float dC,
                out float dR, out float dL, out float dU, out float dD)
            {
                float2 o = texel * thicknessPx;

                // Neighbor depths (linear)
                dR = LinearDepthFromRaw(SampleSceneDepth(uv + float2(o.x, 0)));
                dL = LinearDepthFromRaw(SampleSceneDepth(uv - float2(o.x, 0)));
                dU = LinearDepthFromRaw(SampleSceneDepth(uv + float2(0, o.y)));
                dD = LinearDepthFromRaw(SampleSceneDepth(uv - float2(0, o.y)));

                float norm = max(dC, 0.25);

                // Laplacian (local second derivative) – robust for silhouettes/corners
                float lap = abs((dR + dL + dU + dD) - (4.0 * dC)) / norm;

                // Plane reject – suppress edges on smooth surfaces
                float mn = min(min(dR, dL), min(dU, dD));
                float mx = max(max(dR, dL), max(dU, dD));
                float range = (mx - mn) / norm;

                float planeMask = smoothstep(_PlaneReject, _PlaneReject * 2.0, range);

                return lap * planeMask;
            }

            // Optional normal-based edge to help corners read. Uses center normal from caller to avoid re-sampling it.
            inline float NormalEdge_FromCenter(float2 uv, float2 texel, float thicknessPx, float3 nC)
            {
                float2 o = texel * thicknessPx;

                float3 nR = SampleSceneNormals(uv + float2(o.x, 0));
                float3 nL = SampleSceneNormals(uv - float2(o.x, 0));
                float3 nU = SampleSceneNormals(uv + float2(0, o.y));
                float3 nD = SampleSceneNormals(uv - float2(0, o.y));

                // Max neighbor difference from center
                return max(max(length(nC - nR), length(nC - nL)), max(length(nC - nU), length(nC - nD)));
            }

            // Detail signal: small-scale normal variation (bump-like). Uses center normal from caller.
            inline float DetailSignal_FromCenter(float2 uv, float2 texel, float stepPx, float3 nC)
            {
                float2 o = texel * stepPx;

                float3 nR = SampleSceneNormals(uv + float2(o.x, 0));
                float3 nU = SampleSceneNormals(uv + float2(0, o.y));

                return max(length(nC - nR), length(nC - nU));
            }

            half4 Frag(Varyings input) : SV_Target
            {
                const float4 kBlack = float4(0, 0, 0, 1);

                float2 uv = input.uv;

                // Center depth – sample ONCE and reuse (saves a depth fetch).
                float rawDepthC = SampleSceneDepth(uv);

                // Background guard (normal Z + reversed Z)
                #if defined(UNITY_REVERSED_Z)
                    if (rawDepthC <= 0.000001)
                        return kBlack;
                #else
                    if (rawDepthC >= 0.999999)
                        return kBlack;
                #endif

                float dC = LinearDepthFromRaw(rawDepthC);

                // Sense mask uses WS distance → keep world-pos reconstruction (correct sphere in world space).
                float3 wsPos = WorldPosFromDepth(uv, rawDepthC);

                float r = max(_SenseRadius, 1e-4);
                float s = clamp(_SenseSoftness, 0.0001, r);

                float distWS = distance(wsPos, _SenseCenterWS);
                float mask = 1.0 - smoothstep(r - s, r, distWS);
                mask = saturate(mask);

                // Common texel size (avoid recomputing in each helper).
                float2 texel = rcp(_ScreenParams.xy);

                // ---- OUTLINE (depth edge + optional normal edge) ----
                float dR, dL, dU, dD;
                float depthEdgeMag = DepthEdgeRobust_Neighbors(uv, texel, _OutlineThickness, dC, dR, dL, dU, dD);

                float edgeDepth = smoothstep(_DepthEdgeThreshold, _DepthEdgeThreshold + _DepthEdgeFeather, depthEdgeMag) * _DepthEdgeWeight;

                // Foreground-only gate:
                // If any neighbor is meaningfully closer than the center, the center pixel is likely "behind" something.
                // This suppresses the look-through glow artifact.
                float minN = min(min(dR, dL), min(dU, dD));
                float occluded = step(minN, dC - _OcclusionBias); // 1 if occluded by closer neighbor
                float foreGate = 1.0 - occluded;

                edgeDepth *= foreGate;

                float edge = edgeDepth;

                // Sample center normal once; reuse for detail and optional normal edges.
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

                // Anchor detail to edges so flat areas don't get noisy.
                float anchor = max(saturate(edge * 1.25), _DetailFlatAllowance);
                detail *= anchor;

                // Kill micro-detail right on strong outlines to keep silhouettes clean.
                float edgeKill = smoothstep(_DetailKillStart, _DetailKillEnd, edge);
                detail *= (1.0 - edgeKill);

                // ---- COLOR ----
                float3 c = _VisionColor.rgb;

                float3 outCol = 0.0;
                outCol += c * (edge * _OutlineIntensity);
                outCol += c * (detail * _DetailIntensity);

                // Apply sense mask at the end to keep outside pitch black.
                outCol *= mask;

                return half4(outCol, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
