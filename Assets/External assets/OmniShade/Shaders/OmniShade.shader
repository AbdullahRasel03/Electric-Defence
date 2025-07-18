//------------------------------------
//             OmniShade
//     Copyright© 2025 OmniShade     
//------------------------------------

Shader "OmniShade/Standard" { 
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Brightness ("Brightness", range(0, 25)) = 1
		_Contrast ("Contrast", range(0, 25)) = 1
		_Saturation ("Saturation", range(0, 2)) = 1
		[Toggle] _IgnoreMainTexAlpha ("Ignore Main Texture Alpha", Float) = 0

		[HeaderGroup(Diffuse)]
		[Toggle(DIFFUSE)] _Diffuse ("Enable Diffuse", Float) = 1
		_DiffuseWrap ("Diffuse Softness", range(-1, 1)) = 0
		_DiffuseBrightness ("Diffuse Brightness", range(0, 25)) = 1
		_DiffuseContrast ("Diffuse Contrast", range(0.01, 25)) = 1
		[Toggle(DIFFUSE_PER_PIXEL)] _DiffusePerPixel ("Per-Pixel Point Lights", Float) = 0

		[HeaderGroup(Specular)]
		[Toggle(SPECULAR)] _Specular ("Enable Specular", Float) = 0
		_SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
		_SpecularBrightness ("Specular Brightness", range(0, 25)) = 1
		[PowerSlider(10)] _SpecularSmoothness ("Specular Smoothness", range(1, 500)) = 20
		_SpecularTex ("Specular Map", 2D) = "white" {}
		[KeywordEnum(UV1, UV2)] _SpecularUV ("Specular UV", Float) = 0
		[Toggle(SPECULAR_HAIR)] _SpecularHair ("Specular Hair", Float) = 0

		[HeaderGroup(Normal Map)]
		[Normal] _NormalTex ("Normal Map", 2D) = "bump" {}
		[KeywordEnum(UV1, UV2)] _NormalUV ("Normal Map UV", Float) = 0
		[Normal] _NormalTex2 ("Normal Map 2", 2D) = "bump" {}
		[KeywordEnum(UV1, UV2)] _NormalUV2 ("Normal Map 2 UV", Float) = 0
		_NormalStrength ("Normal Strength", range(0, 5)) = 1

		[HeaderGroup(Occlusion Map)]
		_LightmapTex ("Occlusion Map", 2D) = "white" {}
		_LightmapColor ("Occlusion Color", Color) = (1, 1, 1, 1)
		_LightmapBrightness ("Occlusion Brightness", range(0, 25)) = 1
		[KeywordEnum(UV1, UV2)] _LightmapUV ("Occlusion UV", Float) = 0

		[HeaderGroup(Rim Light)]
		[Toggle(RIM)] _Rim ("Enable Rim Light", Float) = 0
		[HDR] _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
		_RimAmount ("Rim Amount", range(0, 25)) = 1
		_RimContrast ("Rim Contrast", range(0, 50)) = 5
		[KeywordEnum(Alpha Blend, Additive, Multiply, Multiply Lighten, Transparency)] _RimBlend ("Rim Blend Mode", Float) = 1
		[Toggle] _RimInverse ("Rim Invert", Float) = 0
		_RimDirection ("Rim Direction", Vector) = (0, 0, 0, 0)

		[HeaderGroup(Reflection)]
		[Toggle(REFLECTION)] _Reflection ("Enable Reflection", Float) = 0
		[NoScaleOffset] _ReflectionTex ("Reflection Cubemap", Cube) = "" {}
		[HDR] _ReflectionColor ("Reflection Color", Color) = (1, 1, 1, 1)
		_ReflectionAmount ("Reflection Amount", range(0, 1)) = 1
		[Toggle(REFLECTION_RIM)] _ReflectionRim ("Mask With Rim", Float) = 0
		[Toggle(REFLECTION_SPECULAR)] _ReflectionSpecular ("Mask With Specular Map", Float) = 0

		[HeaderGroup(Emissive)]
		[HDR] _Emissive ("Emissive Color", Color) = (0, 0, 0, 0)
		_EmissiveTex ("Emissive Map", 2D) = "white" {}

		[HeaderGroup(MatCap)]
		_MatCapTex ("MatCap Texture", 2D) = "black" {}
		_MatCapColor ("MatCap Color", Color) = (1, 1, 1, 1)
		_MatCapBrightness ("MatCap Brightness", range(0, 25)) = 1
		_MatCapContrast ("MatCap Contrast", range(0, 25)) = 1
		[KeywordEnum(Multiply, Multiply Lighten)] _MatCapBlend ("MatCap Blend Mode", Float) = 0
        [Toggle(MATCAP_PERSPECTIVE)] _MatCapPerspective ("Perspective Correction", Float) = 1
		[Toggle(MATCAP_STATIC)] _MatCapStatic ("Use Static Rotation", Float) = 0
		_MatCapRot ("MatCap Static Rotation", Vector) = (0, 0, 0, 0)

		[HeaderGroup(Vertex Colors)]
		[Toggle(VERTEX_COLORS)] _VertexColors ("Enable Vertex Colors", Float) = 0
        _VertexColorsAmount ("Vertex Colors Amount", range(0, 1)) = 1
		_VertexColorsContrast ("Vertex Colors Contrast", range(0, 25)) = 1

		[HeaderGroup(Detail Map)]
		_DetailTex ("Detail Map", 2D) = "white" {}
		_DetailColor ("Detail Color", Color) = (1, 1, 1, 1)
		_DetailBrightness ("Detail Brightness", range(0, 25)) = 1
		_DetailContrast ("Detail Contrast", range(0, 25)) = 1
		[KeywordEnum(Alpha Blend, Additive, Multiply, Multiply Lighten)] _DetailBlend ("Detail Blend Mode", Float) = 2
		[KeywordEnum(UV1, UV2)] _DetailUV ("Detail UV", Float) = 0
		[Toggle(DETAIL_LIGHTING)] _DetailLighting ("Apply To Lighting", Float) = 0
		[Toggle(DETAIL_VERTEX_COLORS)] _DetailVertexColors ("Mask With Vertex Color (A)", Float) = 0

		[HeaderGroup(Layer 1)]
		_Layer1Tex ("Layer Texture", 2D) = "white" {}
		_Layer1Color ("Layer Color", Color) = (1, 1, 1, 1)
		_Layer1Brightness ("Layer Brightness", range(0, 25)) = 1
		_Layer1Alpha ("Layer Alpha", range(0, 25)) = 1
		[KeywordEnum(Alpha Blend, Additive, Multiply, Multiply Lighten)] _Layer1Blend ("Layer Blend Mode", Float) = 0
		[KeywordEnum(UV1, UV2)] _Layer1UV ("Layer 1 UV", Float) = 0
        [Toggle] _Layer1VertexColor ("Mask With Vertex Color (R)", Float) = 0
		
		[HeaderGroup(Layer 2)]
		_Layer2Tex ("Layer Texture", 2D) = "white" {}
		_Layer2Color ("Layer Color", Color) = (1, 1, 1, 1)
		_Layer2Brightness ("Layer Brightness", range(0, 25)) = 1
		_Layer2Alpha ("Layer Alpha", range(0, 25)) = 1
		[KeywordEnum(Alpha Blend, Additive, Multiply, Multiply Lighten)] _Layer2Blend ("Layer Blend Mode", Float) = 0
		[KeywordEnum(UV1, UV2)] _Layer2UV ("Layer 2 UV", Float) = 0
		[Toggle] _Layer2VertexColor ("Mask With Vertex Color (G)", Float) = 0

		[HeaderGroup(Layer 3)]
		_Layer3Tex ("Layer Texture", 2D) = "white" {}
		_Layer3Color ("Layer Color", Color) = (1, 1, 1, 1)
		_Layer3Brightness ("Layer Brightness", range(0, 25)) = 1
		_Layer3Alpha ("Layer Alpha", range(0, 25)) = 1
		[KeywordEnum(Alpha Blend, Additive, Multiply, Multiply Lighten)] _Layer3Blend ("Layer Blend Mode", Float) = 0
        [KeywordEnum(UV1, UV2)] _Layer3UV ("Layer 3 UV", Float) = 0
        [Toggle] _Layer3VertexColor ("Mask With Vertex Color (B)", Float) = 0

		[HeaderGroup(Transparency Mask)]
		_TransparencyMaskTex ("Transparency Mask", 2D) = "white" {}
		_TransparencyMaskAmount ("Mask Amount", range(0, 25)) = 1
		_TransparencyMaskContrast ("Mask Contrast", range(0, 25)) = 1

		[HeaderGroup(Height Based Colors)]
		[Toggle(HEIGHT_COLORS)] _HeightColors ("Enable Height Based Colors", Float) = 0
		_HeightColorsColor ("Color", Color) = (1, 1, 1, 1)
		_HeightColorsAlpha ("Alpha", range(0, 25)) = 1
		_HeightColorsHeight ("Height", range(-100, 100)) = 0
		_HeightColorsEdgeThickness ("Edge Thickness", range(0.001, 100)) = 1
		_HeightColorsThickness ("Thickness", range(0, 100)) = 0
		[Enum(World, 0, Local, 1)] _HeightColorsSpace ("Coordinate Space", Float) = 0
		[KeywordEnum(Alpha Blend, Additive, Lit)] _HeightColorsBlend ("Height Colors Blend Mode", Float) = 0
		_HeightColorsTex ("Height Colors Texture", 2D) = "white" {}

		[HeaderGroup(Shadow Overlay)]
		_ShadowOverlayTex ("Shadow Overlay Tex", 2D) = "white" {}
		_ShadowOverlayBrightness ("Shadow Brightness", range(0, 2)) = 1
		_ShadowOverlaySpeedU ("Shadow Speed U", range(-5, 5)) = 0.1
		_ShadowOverlaySpeedV ("Shadow Speed V", range(-5, 5)) = 0.03
		_ShadowOverlaySwayAmount ("Shadow Sway Amount", range(0, 0.01)) = 0.01
		[KeywordEnum(Scroll, Sway)] _ShadowOverlayAnimation ("Animation Type", Float) = 0

		[HeaderGroup(Plant Sway)]
		[Toggle(PLANT_SWAY)] _Plant ("Enable Plant Sway", Float) = 0
		_PlantSwayAmount ("Sway Amount", range(0, 10)) = 0.15
		_PlantSwaySpeed ("Sway Speed", range(0, 10)) = 1
		_PlantBaseHeight ("Base Height", range(-100, 100)) = 0
		_PlantPhaseVariation ("Phase Variation", range(0, 100)) = 0.3
		[KeywordEnum(Plant, Leaf, Vertex Color Alpha)] _PlantType ("Plant Type", Float) = 0
		[Toggle(PLANT_SWAY_LOCAL)] _PlantLocal ("Use Local Space", Float) = 0

		[HeaderGroup(Outline)]
		[Toggle(OUTLINE)] _Outline ("Enable Outline", Float) = 0
		_OutlineWidth ("Outline Width", range(0, 0.1)) = 0.002
		[Toggle(OUTLINE_WIDTH_INDEPENDENT)] _OutlineWidthIndependent ("Outline Width Camera-Independent", Float) = 0
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineZPos ("Outline Z Offset", Range(-0.1, 1)) = 0   
        [Enum(Show, 8, Hide, 6)] _OutlineComp ("Interior Outlines", Float) = 8
		_OutlineGroup ("Outline Group", Float) = 0
		[HideInInspector] _OutlinePass ("Outline Pass", Float) = 0

		[HeaderGroup(Anime)]
		[Toggle(ANIME)] _Anime ("Enable Anime", Float) = 0
		[HDR] _AnimeColor1 ("Color 1", Color) = (1, 1, 1, 1)
		_AnimeThreshold1 ("Luminance Threshold 1", range(0, 3)) = 0.45
		[HDR] _AnimeColor2 ("Color 2", Color) = (1.35, 1.35, 1.35, 1)
		_AnimeThreshold2 ("Luminance Threshold 2", range(0, 3)) = 0.85
		[HDR] _AnimeColor3 ("Color 3", Color) = (2, 2, 2, 1)
		_AnimeSoftness ("Softness", range(0, 0.25)) = 0.01

		[HeaderGroup(Camera Fade)]
		_CameraFadeStart ("Fade Start Distance", range(0, 25)) = 0
		_CameraFadeEnd ("Fade End Distance", range(0, 25)) = 0

        [HeaderGroup(UV Tile Discard)]
		[Toggle(UVTILE)] _UVTile ("Enable UV Tile Discard", Float) = 0
		[Enum(UV0, 1, UV1, 2, UV2, 3, UV3, 4)] _UVTileDiscardUV ("Discard UV", Float) = 1
        [Toggle] _UVTileV3U0 ("v = 3", Float) = 0
        [Toggle] _UVTileV3U1 ("", Float) = 0
        [Toggle] _UVTileV3U2 ("", Float) = 0
        [Toggle] _UVTileV3U3 ("", Float) = 0
        [Toggle] _UVTileV2U0 ("v = 2", Float) = 0
        [Toggle] _UVTileV2U1 ("", Float) = 0
        [Toggle] _UVTileV2U2 ("", Float) = 0
        [Toggle] _UVTileV2U3 ("", Float) = 0
        [Toggle] _UVTileV1U0 ("v = 1", Float) = 0
        [Toggle] _UVTileV1U1 ("", Float) = 0
        [Toggle] _UVTileV1U2 ("", Float) = 0
        [Toggle] _UVTileV1U3 ("", Float) = 0
        [Toggle] _UVTileV0U0 ("v = 0", Float) = 0
        [Toggle] _UVTileV0U1 ("", Float) = 0
        [Toggle] _UVTileV0U2 ("", Float) = 0
        [Toggle] _UVTileV0U3 ("", Float) = 0

		[HeaderGroup(Environment And Shadows)]
		_AmbientBrightness ("Ambient Brightness", range(0, 25)) = 1
		[Toggle(FOG)] _Fog ("Enable Fog", Float) = 1

		[Header(Shadows)]
		[Toggle(SHADOWS_ENABLED)] _ShadowsEnabled ("Enable Shadows", Float) = 1
		[HDR] _ShadowColor ("Shadow Color", Color) = (0.3, 0.3, 0.3, 1)
		
		[HeaderGroup(Culling And Blending)]
		[Enum(Opaque, 0, Transparent, 1, Transparent Additive, 2, Transparent Additive Alpha, 3, Opaque Cutout, 4)] _Preset ("Culling And Blend Preset", Float) = 0

		[Header(Culling)]
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Culling", Float) = 2 				// Back
		[Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1.0								// On
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Z Test", Float) = 4 			// LessEqual
		_ZOffset ("Depth Offset", range(-5, 5)) = 0
		[Toggle(CUTOUT)] _Cutout ("Cutout Transparency", Float) = 0
		_CutoutCutoff ("Cutoff", range(0, 1)) = 0.5                              

		[Header(Blending)]
		[Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("Source Blend", Float) = 1 	// One
		[Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("Dest Blend", Float) = 0 		// Zero
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Mode", Float) = 0  			// Add

		[HeaderGroup(Rendering)]
		[Toggle(FLAT)] _Flat ("Enable Flat Shading", Float) = 0
	}
	
    Subshader {
        Name "Normal Shader"

        Pass {
            Name "ForwardBase"
            Tags { "LightMode" = "ForwardBase" }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SourceBlend][_DestBlend]
            BlendOp [_BlendOp]

            Stencil {
                Ref [_OutlineGroup]
                Pass [_OutlinePass]
            }

            CGPROGRAM
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
            #include "OmniShadeCore.cginc"
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile _ VERTEXLIGHT_ON
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma shader_feature BASE_CONTRAST
            #pragma shader_feature BASE_SATURATION
            #pragma shader_feature DIFFUSE
            #pragma shader_feature DIFFUSE_PER_PIXEL
            #pragma shader_feature MIXED_LIGHTING
            #pragma shader_feature SPECULAR
            #pragma shader_feature SPECULAR_MAP
            #pragma shader_feature _SPECULARUV_UV1 _SPECULARUV_UV2
            #pragma shader_feature SPECULAR_HAIR
            #pragma shader_feature RIM
            #pragma shader_feature _RIMBLEND_ALPHA_BLEND _RIMBLEND_ADDITIVE _RIMBLEND_MULTIPLY _RIMBLEND_MULTIPLY_LIGHTEN _RIMBLEND_TRANSPARENCY
            #pragma shader_feature RIM_DIRECTION
            #pragma shader_feature REFLECTION
            #pragma shader_feature REFLECTION_TEX
            #pragma shader_feature REFLECTION_RIM
            #pragma shader_feature REFLECTION_SPECULAR
            #pragma shader_feature NORMAL_MAP
            #pragma shader_feature NORMAL_MAP2
            #pragma shader_feature _NORMALUV_UV1 _NORMALUV_UV2
            #pragma shader_feature _NORMALUV2_UV1 _NORMALUV2_UV2
            #pragma shader_feature LIGHT_MAP
            #pragma shader_feature _LIGHTMAPUV_UV1 _LIGHTMAPUV_UV2
            #pragma shader_feature EMISSIVE_MAP
            #pragma shader_feature MATCAP
            #pragma shader_feature MATCAP_CONTRAST
            #pragma shader_feature _MATCAPBLEND_MULTIPLY _MATCAPBLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature MATCAP_PERSPECTIVE
            #pragma shader_feature MATCAP_STATIC
            #pragma shader_feature VERTEX_COLORS
            #pragma shader_feature VERTEX_COLORS_CONTRAST
            #pragma shader_feature DETAIL
            #pragma shader_feature DETAIL_CONTRAST
            #pragma shader_feature _DETAILBLEND_ALPHA_BLEND _DETAILBLEND_ADDITIVE _DETAILBLEND_MULTIPLY _DETAILBLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _DETAILUV_UV1 _DETAILUV_UV2
            #pragma shader_feature DETAIL_LIGHTING
            #pragma shader_feature DETAIL_VERTEX_COLORS
            #pragma shader_feature LAYER1
            #pragma shader_feature _LAYER1BLEND_ALPHA_BLEND _LAYER1BLEND_ADDITIVE _LAYER1BLEND_MULTIPLY _LAYER1BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER1UV_UV1 _LAYER1UV_UV2
            #pragma shader_feature LAYER2
            #pragma shader_feature _LAYER2BLEND_ALPHA_BLEND _LAYER2BLEND_ADDITIVE _LAYER2BLEND_MULTIPLY _LAYER2BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER2UV_UV1 _LAYER2UV_UV2
            #pragma shader_feature LAYER3
            #pragma shader_feature _LAYER3BLEND_ALPHA_BLEND _LAYER3BLEND_ADDITIVE _LAYER3BLEND_MULTIPLY _LAYER3BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER3UV_UV1 _LAYER3UV_UV2
            #pragma shader_feature TRANSPARENCY_MASK
            #pragma shader_feature TRANSPARENCY_MASK_CONTRAST
            #pragma shader_feature HEIGHT_COLORS
            #pragma shader_feature _HEIGHTCOLORSBLEND_ALPHA_BLEND _HEIGHTCOLORSBLEND_ADDITIVE _HEIGHTCOLORSBLEND_LIT
            #pragma shader_feature HEIGHT_COLORS_TEX
            #pragma shader_feature SHADOW_OVERLAY
            #pragma shader_feature _SHADOWOVERLAYANIMATION_SCROLL _SHADOWOVERLAYANIMATION_SWAY
            #pragma shader_feature PLANT_SWAY
            #pragma shader_feature PLANT_SWAY_LOCAL
            #pragma shader_feature _PLANTTYPE_PLANT _PLANTTYPE_LEAF _PLANTTYPE_VERTEX_COLOR_ALPHA
            #pragma shader_feature ANIME
            #pragma shader_feature ANIME_SOFT
            #pragma shader_feature CAMERA_FADE
            #pragma shader_feature UVTILE
            #pragma shader_feature AMBIENT
            #pragma shader_feature FOG
            #pragma shader_feature SHADOWS_ENABLED
            #pragma shader_feature ZOFFSET
            #pragma shader_feature CUTOUT
            #pragma shader_feature FLAT
            ENDCG
        }

        Pass {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            Cull Front
            Blend One Zero

            Stencil {
                Ref [_OutlineGroup]
                Comp [_OutlineComp]
            }

            CGPROGRAM
            #define OUTLINE_PASS 1
            #include "UnityCG.cginc"
            #include "OmniShadeCore.cginc"
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature TRANSPARENCY_MASK
            #pragma shader_feature TRANSPARENCY_MASK_CONTRAST
            #pragma shader_feature PLANT_SWAY
            #pragma shader_feature PLANT_SWAY_LOCAL
            #pragma shader_feature _PLANTTYPE_PLANT _PLANTTYPE_LEAF _PLANTTYPE_VERTEX_COLOR_ALPHA
            #pragma shader_feature OUTLINE
            #pragma shader_feature OUTLINE_WIDTH_INDEPENDENT
            #pragma shader_feature OUTLINE_PASS_DISABLED
            #pragma shader_feature CAMERA_FADE
            #pragma shader_feature UVTILE
            #pragma shader_feature ZOFFSET
            #pragma shader_feature CUTOUT
            ENDCG
        }

        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SourceBlend][_DestBlend]
            BlendOp [_BlendOp]

            CGPROGRAM
            #define SHADOW_CASTER 1
            #include "UnityCG.cginc"
            #include "OmniShadeCore.cginc"
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature TRANSPARENCY_MASK
            #pragma shader_feature TRANSPARENCY_MASK_CONTRAST
            #pragma shader_feature PLANT_SWAY
            #pragma shader_feature PLANT_SWAY_LOCAL
            #pragma shader_feature _PLANTTYPE_PLANT _PLANTTYPE_LEAF _PLANTTYPE_VERTEX_COLOR_ALPHA
            #pragma shader_feature UVTILE
            #pragma shader_feature CUTOUT
            ENDCG
        }

        Pass {
            Name "Meta"
            Tags { "LightMode" = "Meta" }
            Cull Off
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SourceBlend][_DestBlend]
            BlendOp [_BlendOp]

            CGPROGRAM
            #define META 1
            #include "UnityCG.cginc"
            #include "UnityMetaPass.cginc"
            #include "OmniShadeCore.cginc"
            #pragma target 3.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma shader_feature BASE_CONTRAST
            #pragma shader_feature EMISSIVE_MAP
            #pragma shader_feature MATCAP
            #pragma shader_feature MATCAP_CONTRAST
            #pragma shader_feature _MATCAPBLEND_MULTIPLY _MATCAPBLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature VERTEX_COLORS
            #pragma shader_feature VERTEX_COLORS_CONTRAST
            #pragma shader_feature DETAIL
            #pragma shader_feature DETAIL_CONTRAST
            #pragma shader_feature _DETAILBLEND_ALPHA_BLEND _DETAILBLEND_ADDITIVE _DETAILBLEND_MULTIPLY _DETAILBLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _DETAILUV_UV1 _DETAILUV_UV2
            #pragma shader_feature DETAIL_LIGHTING
            #pragma shader_feature DETAIL_VERTEX_COLORS
            #pragma shader_feature LAYER1
            #pragma shader_feature _LAYER1BLEND_ALPHA_BLEND _LAYER1BLEND_ADDITIVE _LAYER1BLEND_MULTIPLY _LAYER1BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER1UV_UV1 _LAYER1UV_UV2
            #pragma shader_feature LAYER2
            #pragma shader_feature _LAYER2BLEND_ALPHA_BLEND _LAYER2BLEND_ADDITIVE _LAYER2BLEND_MULTIPLY _LAYER2BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER2UV_UV1 _LAYER2UV_UV2
            #pragma shader_feature LAYER3
            #pragma shader_feature _LAYER3BLEND_ALPHA_BLEND _LAYER3BLEND_ADDITIVE _LAYER3BLEND_MULTIPLY _LAYER3BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER3UV_UV1 _LAYER3UV_UV2
            #pragma shader_feature TRANSPARENCY_MASK
            #pragma shader_feature TRANSPARENCY_MASK_CONTRAST
            #pragma shader_feature HEIGHT_COLORS
            #pragma shader_feature _HEIGHTCOLORSBLEND_ALPHA_BLEND _HEIGHTCOLORSBLEND_ADDITIVE _HEIGHTCOLORSBLEND_LIT
            #pragma shader_feature HEIGHT_COLORS_TEX
            #pragma shader_feature UVTILE
            #pragma shader_feature CUTOUT
            ENDCG
        }
    }

    Subshader {
        Name "Fallback Shader"

        Pass {
            Name "Fallback"
            Tags { "LightMode" = "ForwardBase" }
            Cull [_Cull]
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Blend [_SourceBlend][_DestBlend]
            BlendOp [_BlendOp]

            CGPROGRAM
            #define FALLBACK_PASS 1
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"
            #include "OmniShadeCore.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma shader_feature BASE_SATURATION
            #pragma shader_feature DIFFUSE
            #pragma shader_feature MIXED_LIGHTING
            #pragma shader_feature SPECULAR
            #pragma shader_feature RIM
            #pragma shader_feature _RIMBLEND_ALPHA_BLEND _RIMBLEND_ADDITIVE _RIMBLEND_MULTIPLY _RIMBLEND_MULTIPLY_LIGHTEN _RIMBLEND_TRANSPARENCY
            #pragma shader_feature LIGHT_MAP
            #pragma shader_feature _LIGHTMAPUV_UV1 _LIGHTMAPUV_UV2
            #pragma shader_feature MATCAP
            #pragma shader_feature _MATCAPBLEND_MULTIPLY _MATCAPBLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature MATCAP_STATIC
            #pragma shader_feature VERTEX_COLORS
            #pragma shader_feature LAYER1
            #pragma shader_feature _LAYER1BLEND_ALPHA_BLEND _LAYER1BLEND_ADDITIVE _LAYER1BLEND_MULTIPLY _LAYER1BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER1UV_UV1 _LAYER1UV_UV2
            #pragma shader_feature LAYER2
            #pragma shader_feature _LAYER2BLEND_ALPHA_BLEND _LAYER2BLEND_ADDITIVE _LAYER2BLEND_MULTIPLY _LAYER2BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER2UV_UV1 _LAYER2UV_UV2
            #pragma shader_feature LAYER3
            #pragma shader_feature _LAYER3BLEND_ALPHA_BLEND _LAYER3BLEND_ADDITIVE _LAYER3BLEND_MULTIPLY _LAYER3BLEND_MULTIPLY_LIGHTEN
            #pragma shader_feature _LAYER3UV_UV1 _LAYER3UV_UV2
            #pragma shader_feature TRANSPARENCY_MASK
            #pragma shader_feature ANIME
            #pragma shader_feature UVTILE
            #pragma shader_feature AMBIENT
            #pragma shader_feature FOG
            #pragma shader_feature ZOFFSET
            #pragma shader_feature CUTOUT
            #pragma shader_feature FLAT
            ENDCG
        }
    }

	Fallback "Diffuse"
	CustomEditor "OmniShadeGUI"
    // float4 color : COLOR;  This line is for Polybrush support
}
