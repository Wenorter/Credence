Shader "Custom/BlindBuilding_URP"
{
    Properties
    {
        _FloorColor ("Floor Color", Color) = (0.06, 0.06, 0.06, 1)
        _GridColor  ("Grid Color",  Color) = (0.22, 0.22, 0.22, 1)
        _GridScale  ("Grid Scale",  Range(0.05, 2)) = 0.35
        _GridLineWidth ("Grid Line Width", Range(0.001, 0.08)) = 0.02
        _FloorIntensity ("Floor Intensity", Range(0, 5)) = 1.2

        _WallGlow ("Wall Glow", Range(0, 20)) = 8
        _FresnelPow ("Fresnel Power", Range(0.5, 8)) = 2.5

        _FloorDotThreshold ("Floor Dot Threshold", Range(0,1)) = 0.7
        _EdgeSoftness ("Edge Softness", Range(0.01, 0.5)) = 0.15
        _NearPow ("Near Falloff Power", Range(0.5, 4)) = 1.5

        // Set globally by script:
        _Blind_PlayerPos ("Blind Player Pos", Vector) = (0,0,0,1)
        _Blind_Radius    ("Blind Radius", Float) = 4
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Name "BlindBuilding"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _FloorColor;
                float4 _GridColor;
                float  _GridScale;
                float  _GridLineWidth;
                float  _FloorIntensity;

                float  _WallGlow;
                float  _FresnelPow;

                float  _FloorDotThreshold;
                float  _EdgeSoftness;
                float  _NearPow;

                float4 _Blind_PlayerPos;
                float  _Blind_Radius;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS  = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS    = TransformObjectToWorldNormal(v.normalOS);
                return o;
            }

            float GridLines(float2 worldXZ, float gridScale, float lineWidth)
            {
                // gridScale here is "cells per meter-ish"
                float2 p = worldXZ * gridScale;
                float2 f = abs(frac(p) - 0.5);
                float d = min(f.x, f.y);
                return 1.0 - smoothstep(lineWidth, lineWidth + 0.01, d);
            }

            half4 frag (Varyings i) : SV_Target
            {
                float3 N = normalize(i.normalWS);
                float3 V = normalize(GetWorldSpaceViewDir(i.positionWS));

                // Near mask (0 at radius edge, 1 at player)
                float3 playerPos = _Blind_PlayerPos.xyz;
                float dist = distance(i.positionWS, playerPos);
                float nearRaw = saturate(1.0 - dist / max(0.001, _Blind_Radius));
                float near = smoothstep(0.0, 1.0, nearRaw);
                near = pow(near, _NearPow);

                // Floor vs wall separation by normal
                float floorDot = dot(N, float3(0,1,0));
                float floorMask = smoothstep(_FloorDotThreshold, _FloorDotThreshold + 0.08, floorDot);
                float wallMask  = 1.0 - floorMask;

                // Fresnel for walls aura feel
                float fresnel = 1.0 - saturate(dot(N, V));
                fresnel = pow(fresnel, _FresnelPow);

                // Floor grid (world-space)
                float grid = GridLines(i.positionWS.xz, 1.0 / max(0.001, _GridScale), _GridLineWidth);
                float3 floorCol = lerp(_FloorColor.rgb, _GridColor.rgb, grid) * _FloorIntensity;

                // Both floor and walls only show near player
                float floorVis = floorMask * near;
                float wallAura = wallMask * near * fresnel;

                float3 wallCol = wallAura * _WallGlow * float3(1,1,1);

                float3 col = floorCol * floorVis + wallCol;

                // Soft alpha edge
                float alpha = saturate(floorVis + wallAura);
                alpha = smoothstep(0.0, _EdgeSoftness, alpha);

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
