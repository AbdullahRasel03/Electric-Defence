Shader "CustomShader/BgScroll"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Tiling("Tiling", Vector) = (1, 1, 0, 0)
        _ScrollSpeedX("Scroll Speed X", Float) = 0.1
        _ScrollSpeedY("Scroll Speed Y", Float) = 0.1
        _Alpha("Alpha", Range(0,1)) = 1

        // Required by UI system for stencil/masking
        [HideInInspector]_Stencil("Stencil ID", Float) = 0
        [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        LOD 100

        Pass
        {
            Name "ScrollingTextureUI"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _Tiling;
            float4 _Color;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            float _Alpha;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                float2 scrollUV =  (IN.uv * _Tiling.xy) + float2(_ScrollSpeedX, _ScrollSpeedY) * _Time.y;

                OUT.uv = scrollUV;
                

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                col *= _Color;
                col.a *= _Alpha;
                return col;
            }
            ENDHLSL
        }
    }
}
