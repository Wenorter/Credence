Shader "UI/CinematicFadeIn"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (0,0,0,1)
        _Progress ("Progress", Range(0,1)) = 0
        _Softness ("Iris Softness", Range(0.05,1.5)) = 0.6
        _Letterbox ("Letterbox Height", Range(0,0.2)) = 0.1
        _Grain ("Film Grain", Range(0,1)) = 0.35
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off Lighting Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Tint;
            float _Progress, _Softness, _Letterbox, _Grain;

            struct appdata { float4 vertex : POSITION; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float4 screen : TEXCOORD0; fixed4 col : COLOR; };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screen = ComputeScreenPos(o.pos);
                o.col = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screen.xy / i.screen.w;
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 p = (uv - 0.5) * float2(aspect, 1);
                float d = length(p);

                float e = _Progress * _Progress * (3 - 2 * _Progress);
                float maxR = length(float2(aspect, 1)) * 0.5 + _Softness;
                float r = e * maxR;
                float reveal = 1 - smoothstep(r - _Softness, r, d);

                float dim = 1 - smoothstep(0.0, 0.55, _Progress);
                float a = max(1 - reveal, dim);

                float bh = _Letterbox * (1 - smoothstep(0.35, 1.0, _Progress));
                float bar = step(uv.y, bh) + step(1 - bh, uv.y);
                a = max(a, saturate(bar));

                float n = frac(sin(dot(uv * _ScreenParams.xy + _Time.y * 60.0, float2(12.9898, 78.233))) * 43758.5453);
                a = saturate(a + (n - 0.5) * _Grain * a * (1 - a) * 4);

                return fixed4(_Tint.rgb, a * _Tint.a * i.col.a);
            }
            ENDCG
        }
    }
}