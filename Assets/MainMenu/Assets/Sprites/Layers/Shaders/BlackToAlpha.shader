Shader "UI/BlackToAlpha" 
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {} 
        _Threshold ("Threshold", Range(0,1)) = 0.05 
        _Softness ("Softness", Range(0.001,1)) = 0.2 
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" } 
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off Cull Off 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Threshold, _Softness; 

            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 col : COLOR; };

            v2f vert (appdata_full v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.col = v.color; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                float luma = max(c.r, max(c.g, c.b)); 
                c.a = smoothstep(_Threshold, _Threshold + _Softness, luma) * i.col.a; 
                return c;
            }
            ENDCG
        }
    }
}