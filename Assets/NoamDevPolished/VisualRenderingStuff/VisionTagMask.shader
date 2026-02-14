// Assets/Shaders/VisionTagMask.shader
Shader "Hidden/Vision/TagMask"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        Pass
        {
            Name "VisionTagMask"
            ZWrite Off
            ZTest LEqual
            Cull Back
            Blend Off

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                // R=1 means "this pixel belongs to the category"
                return half4(1, 0, 0, 1);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
