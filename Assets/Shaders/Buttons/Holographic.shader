Shader "Custom/Holographic"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Cyber Settings)]
        _GlitchIntensity ("Glitch Intensity", Range(0, 0.1)) = 0.005
        _GlitchFrequency ("Glitch Frequency", Range(0, 20)) = 5
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.2
        _ScanlineCount ("Scanline Count", Float) = 200
        _NoiseAmount ("Digital Noise", Range(0, 1)) = 0.05

        // UI Masking support
        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _ClipRect;
            
            float _GlitchIntensity;
            float _GlitchFrequency;
            float _ScanlineIntensity;
            float _ScanlineCount;
            float _NoiseAmount;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453123);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                float time = _Time.y;

                float glitchTime = time * _GlitchFrequency;
                float noiseBlock = floor(uv.y * 20.0 + glitchTime);
                float displacement = random(float2(noiseBlock, glitchTime));
                
                float activeGlitch = step(0.9, displacement) * _GlitchIntensity * sin(time * 100.0);
                
                float2 rUV = uv + float2(activeGlitch, 0);
                float2 gUV = uv;
                float2 bUV = uv - float2(activeGlitch, 0);

                fixed4 color;
                color.r = tex2D(_MainTex, rUV).r;
                color.g = tex2D(_MainTex, gUV).g;
                color.b = tex2D(_MainTex, bUV).b;
                color.a = tex2D(_MainTex, uv).a;

                float scanline = sin(uv.y * _ScanlineCount + time * 2.0);
                float scanlineEffect = 1.0 - (_ScanlineIntensity * (scanline * 0.5 + 0.5));
                color.rgb *= scanlineEffect;

                float noise = random(uv + time) * _NoiseAmount;
                color.rgb += noise;

                color *= IN.color;
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                clip (color.a - 0.01);

                return color;
            }
            ENDCG
        }
    }
}