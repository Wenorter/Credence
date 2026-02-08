Shader "FS_PureMagenta_URP_Helper"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Overlay" }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert (Attributes v)
            {
                Varyings o;

                // Fullscreen triangle from vertex ID (always correct).
                o.positionHCS = GetFullScreenTriangleVertexPosition(v.vertexID);
                o.uv = GetFullScreenTriangleTexCoord(v.vertexID);

                return o;
            }

            float4 Frag (Varyings i) : SV_Target
            {
                return float4(1, 0, 1, 1);
            }
            ENDHLSL
        }
    }
}
