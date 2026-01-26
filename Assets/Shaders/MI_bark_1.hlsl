#define NUM_TEX_COORD_INTERPOLATORS 2
#define NUM_MATERIAL_TEXCOORDS_VERTEX 2
#define NUM_CUSTOM_VERTEX_INTERPOLATORS 0

struct Input
{
	//float3 Normal;
	float2 uv_MainTex : TEXCOORD0;
	float2 uv2_Material_Texture2D_0 : TEXCOORD1;
	float4 color : COLOR;
	float4 tangent;
	//float4 normal;
	float3 viewDir;
	float4 screenPos;
	float3 worldPos;
	//float3 worldNormal;
	float3 normal2;
};
struct SurfaceOutputStandard
{
	float3 Albedo;		// base (diffuse or specular) color
	float3 Normal;		// tangent space normal, if written
	half3 Emission;
	half Metallic;		// 0=non-metal, 1=metal
	// Smoothness is the user facing name, it should be perceptual smoothness but user should not have to deal with it.
	// Everywhere in the code you meet smoothness it is perceptual smoothness
	half Smoothness;	// 0=rough, 1=smooth
	half Occlusion;		// occlusion (default 1)
	float Alpha;		// alpha for transparencies
};

//#define HDRP 1
#define URP 1
#define UE5
//#define HAS_CUSTOMIZED_UVS 1
#define MATERIAL_TANGENTSPACENORMAL 1
//struct Material
//{
	//samplers start
SAMPLER( SamplerState_Linear_Repeat );
SAMPLER( SamplerState_Linear_Clamp );
TEXTURE2D(       Material_Texture2D_0 );
SAMPLER(  samplerMaterial_Texture2D_0 );
float4 Material_Texture2D_0_TexelSize;
float4 Material_Texture2D_0_ST;
TEXTURE2D(       Material_Texture2D_1 );
SAMPLER(  samplerMaterial_Texture2D_1 );
float4 Material_Texture2D_1_TexelSize;
float4 Material_Texture2D_1_ST;
TEXTURE2D(       Material_Texture2D_2 );
SAMPLER(  samplerMaterial_Texture2D_2 );
float4 Material_Texture2D_2_TexelSize;
float4 Material_Texture2D_2_ST;
TEXTURE2D(       Material_Texture2D_3 );
SAMPLER(  samplerMaterial_Texture2D_3 );
float4 Material_Texture2D_3_TexelSize;
float4 Material_Texture2D_3_ST;
TEXTURE2D(       Material_Texture2D_4 );
SAMPLER(  samplerMaterial_Texture2D_4 );
float4 Material_Texture2D_4_TexelSize;
float4 Material_Texture2D_4_ST;
TEXTURE2D(       Material_Texture2D_5 );
SAMPLER(  samplerMaterial_Texture2D_5 );
float4 Material_Texture2D_5_TexelSize;
float4 Material_Texture2D_5_ST;
TEXTURE2D(       Material_Texture2D_6 );
SAMPLER(  samplerMaterial_Texture2D_6 );
float4 Material_Texture2D_6_TexelSize;
float4 Material_Texture2D_6_ST;
TEXTURE2D(       Material_Texture2D_7 );
SAMPLER(  samplerMaterial_Texture2D_7 );
float4 Material_Texture2D_7_TexelSize;
float4 Material_Texture2D_7_ST;
TEXTURE2D(       Material_Texture2D_8 );
SAMPLER(  samplerMaterial_Texture2D_8 );
float4 Material_Texture2D_8_TexelSize;
float4 Material_Texture2D_8_ST;
TEXTURE2D(       Material_Texture2D_9 );
SAMPLER(  samplerMaterial_Texture2D_9 );
float4 Material_Texture2D_9_TexelSize;
float4 Material_Texture2D_9_ST;
TEXTURE2D(       Material_Texture2D_10 );
SAMPLER(  samplerMaterial_Texture2D_10 );
float4 Material_Texture2D_10_TexelSize;
float4 Material_Texture2D_10_ST;
TEXTURE2D(       Material_Texture2D_11 );
SAMPLER(  samplerMaterial_Texture2D_11 );
float4 Material_Texture2D_11_TexelSize;
float4 Material_Texture2D_11_ST;
TEXTURE2D(       Material_Texture2D_12 );
SAMPLER(  samplerMaterial_Texture2D_12 );
float4 Material_Texture2D_12_TexelSize;
float4 Material_Texture2D_12_ST;
TEXTURE2D(       Material_Texture2D_13 );
SAMPLER(  samplerMaterial_Texture2D_13 );
float4 Material_Texture2D_13_TexelSize;
float4 Material_Texture2D_13_ST;
TEXTURE2D(       Material_Texture2D_14 );
SAMPLER(  samplerMaterial_Texture2D_14 );
float4 Material_Texture2D_14_TexelSize;
float4 Material_Texture2D_14_ST;
uniform float4 SelectionColor;
uniform float Rotation_custom;
uniform float U_tile;
uniform float V_tile;
uniform float Tiling;
uniform float Base_UV___U_Tiling;
uniform float Base_UV___V_Tiling;
uniform float UTG_3_Normal_Strengh;
uniform float Rotation_custom_Mask_RGBA;
uniform float U_tile_Mask_RGBA;
uniform float V_tile_Mask_RGBA;
uniform float Tiling_Mask_RGBA;
uniform float Base_UV___U_Tiling_Mask_RGBA;
uniform float Base_UV___V_Tiling_Mask_RGBA;
uniform float Tiling_texture_R;
uniform float Normal_Strengh_maskR;
uniform float UVmask_R_opacity_bloor;
uniform float UVmask_R_opacity;
uniform float Grunge_tiling;
uniform float Grunge_channel_RGBA_power;
uniform float Tiling_texture_G;
uniform float Normal_Strengh_maskG;
uniform float UVmask_G_opacity_bloor;
uniform float UVmask_G_opacity;
uniform float Tiling_texture_B;
uniform float Normal_Strengh_maskB;
uniform float UVmask_B_opacity_bloor;
uniform float UVmask_B_opacity;
uniform float UVmask_A_opacity_bloor;
uniform float UVmask_A_opacity;
uniform float Normal_RGBA_;
uniform float Brightness__opacity_bloor;
uniform float Brightness__opacity;
uniform float4 emissive_color;
uniform float emissive_power;
uniform float4 UTG_1_Tint_base_texture;
uniform float UTG_1_color_tone;
uniform float UTG_1_Brightness;
uniform float UTG_1_contrast;
uniform float4 maskR_Tint;
uniform float color_tone_maskR;
uniform float Brightness_Base_maskR;
uniform float contrast_maskR;
uniform float4 Tint_maskG;
uniform float color_tone_maskG;
uniform float Brightness_maskG;
uniform float contrast_maskG;
uniform float4 Tint_maskB;
uniform float color_tone_maskB;
uniform float Brightness_maskB;
uniform float contrast_maskB;
uniform float4 tint_Brightness_;
uniform float Brightness__Rchannel;
uniform float Metallic_Contrast_main_layer;
uniform float Metallic_Intensity_main_layer;
uniform float Metallic_Contrast_maskR;
uniform float Metallic_Intensity_maskR;
uniform float Metallic_Contrast_maskG;
uniform float Metallic_Intensity_maskG;
uniform float Metallic_Contrast_maskB;
uniform float Metallic_Intensity_maskB;
uniform float Specular_6;
uniform float UTG_2_Roughness_min;
uniform float UTG_2_Roughness_max;
uniform float Roughness_min_maskR;
uniform float Roughness_max_maskR;
uniform float Roughness_min_maskG;
uniform float Roughness_max_maskG;
uniform float Roughness_min_maskB;
uniform float Roughness_max_maskB;
uniform float Roughness_max_RGBA_;
uniform float UTG_4_Opacity_Mask_power;
uniform float UTG_1_SSS;
uniform float __SubsurfaceProfile;
uniform float UTG_2_AO_power;
uniform float AO_power_maskR;
uniform float AO_power_maskG;
uniform float AO_power_maskB;

//};

#ifdef UE5
	#define UE_LWC_RENDER_TILE_SIZE			2097152.0
	#define UE_LWC_RENDER_TILE_SIZE_SQRT	1448.15466
	#define UE_LWC_RENDER_TILE_SIZE_RSQRT	0.000690533954
	#define UE_LWC_RENDER_TILE_SIZE_RCP		4.76837158e-07
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_PI		0.673652053
	#define UE_LWC_RENDER_TILE_SIZE_FMOD_2PI	0.673652053
	#define INVARIANT(X) X
	#define PI 					(3.1415926535897932)

	#include "LargeWorldCoordinates.hlsl"
#endif
struct MaterialStruct
{
	float4 PreshaderBuffer[28];
	float4 ScalarExpressions[1];
	float VTPackedPageTableUniform[2];
	float VTPackedUniform[1];
};
#define SVTPackedUniform VTPackedUniform
static SamplerState View_MaterialTextureBilinearWrapedSampler;
static SamplerState View_MaterialTextureBilinearClampedSampler;
struct ViewStruct
{
	float GameTime;
	float RealTime;
	float DeltaTime;
	float PrevFrameGameTime;
	float PrevFrameRealTime;
	float MaterialTextureMipBias;	
	float4 PrimitiveSceneData[ 40 ];
	float4 TemporalAAParams;
	float2 ViewRectMin;
	float4 ViewSizeAndInvSize;
	float2 ResolutionFractionAndInv;
	float MaterialTextureDerivativeMultiply;
	uint StateFrameIndexMod8;
	uint StateFrameIndex;
	float FrameNumber;
	float2 FieldOfViewWideAngles;
	float4 RuntimeVirtualTextureMipLevel;
	float PreExposure;
	float4 BufferBilinearUVMinMax;
};
struct ResolvedViewStruct
{
	#ifdef UE5
		FLWCVector3 WorldCameraOrigin;
		FLWCVector3 PrevWorldCameraOrigin;
		FLWCVector3 PreViewTranslation;
		FLWCVector3 WorldViewOrigin;
	#else
		float3 WorldCameraOrigin;
		float3 PrevWorldCameraOrigin;
		float3 PreViewTranslation;
		float3 WorldViewOrigin;
	#endif
	float4 ScreenPositionScaleBias;
	float4x4 TranslatedWorldToView;
	float4x4 TranslatedWorldToCameraView;
	float4x4 TranslatedWorldToClip;
	float4x4 ViewToTranslatedWorld;
	float4x4 PrevViewToTranslatedWorld;
	float4x4 CameraViewToTranslatedWorld;
	float4 BufferBilinearUVMinMax;
	float4 XRPassthroughCameraUVs[ 2 ];
};
struct PrimitiveStruct
{
	float4x4 WorldToLocal;
	float4x4 LocalToWorld;
};

static ViewStruct View;
static ResolvedViewStruct ResolvedView;
static PrimitiveStruct Primitive;
uniform float4 View_BufferSizeAndInvSize;
uniform float4 LocalObjectBoundsMin;
uniform float4 LocalObjectBoundsMax;
static SamplerState Material_Wrap_WorldGroupSettings;
static SamplerState Material_Clamp_WorldGroupSettings;

#include "UnrealCommon.cginc"

static MaterialStruct Material;
void InitializeExpressions()
{
	Material.PreshaderBuffer[0] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[1] = float4(0.000000,0.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[2] = float4(1.000000,1.000000,-0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[4] = float4(1.000000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[5] = float4(1.000000,2.000000,27.741325,2.000000);//(Unknown)
	Material.PreshaderBuffer[6] = float4(-1.000000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[7] = float4(2.000000,-1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[8] = float4(1.000000,2.000000,-1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[9] = float4(2.000000,-1.000000,1.000000,2.000000);//(Unknown)
	Material.PreshaderBuffer[10] = float4(-1.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[11] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[12] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[13] = float4(0.706667,0.058667,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[14] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[15] = float4(0.757265,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[16] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[17] = float4(0.347928,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[18] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[19] = float4(0.420000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[20] = float4(1.000000,1.000000,1.000000,0.630805);//(Unknown)
	Material.PreshaderBuffer[21] = float4(1.000000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[22] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[23] = float4(0.000000,1.000000,0.000000,0.150000);//(Unknown)
	Material.PreshaderBuffer[24] = float4(1.000000,0.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[25] = float4(1.000000,0.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[26] = float4(1.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[27] = float4(1.000000,1.000000,1.000000,1.000000);//(Unknown)

	Material.PreshaderBuffer[0].xy = Append(Cos( Rotation_custom * 6.283 ), Sin( Rotation_custom * 6.283 ) * -1);
	Material.PreshaderBuffer[0].zw = Append(Sin( Rotation_custom * 6.283 ), Cos( Rotation_custom * 6.283 ));
	Material.PreshaderBuffer[1].xy = Append(U_tile, V_tile);
	Material.PreshaderBuffer[1].zw = Tiling * Append(Base_UV___U_Tiling, Base_UV___V_Tiling);
	Material.PreshaderBuffer[2].x = UTG_3_Normal_Strengh;
	Material.PreshaderBuffer[2].yz = Append(Cos( Rotation_custom_Mask_RGBA * 6.283 ), Sin( Rotation_custom_Mask_RGBA * 6.283 ) * -1);
	Material.PreshaderBuffer[3].xy = Append(Sin( Rotation_custom_Mask_RGBA * 6.283 ), Cos( Rotation_custom_Mask_RGBA * 6.283 ));
	Material.PreshaderBuffer[3].zw = Append(U_tile_Mask_RGBA, V_tile_Mask_RGBA);
	Material.PreshaderBuffer[4].xy = Tiling_Mask_RGBA * Append(Base_UV___U_Tiling_Mask_RGBA, Base_UV___V_Tiling_Mask_RGBA);
	Material.PreshaderBuffer[4].z = Tiling_texture_R;
	Material.PreshaderBuffer[4].w = Normal_Strengh_maskR;
	Material.PreshaderBuffer[5].x = UVmask_R_opacity;
	Material.PreshaderBuffer[5].y = Grunge_tiling;
	Material.PreshaderBuffer[5].z = Grunge_channel_RGBA_power;
	Material.PreshaderBuffer[5].w = UVmask_R_opacity_bloor + 1;
	Material.PreshaderBuffer[6].x = 0 - UVmask_R_opacity_bloor;
	Material.PreshaderBuffer[6].y = Tiling_texture_G;
	Material.PreshaderBuffer[6].z = Normal_Strengh_maskG;
	Material.PreshaderBuffer[6].w = UVmask_G_opacity;
	Material.PreshaderBuffer[7].x = UVmask_G_opacity_bloor + 1;
	Material.PreshaderBuffer[7].y = 0 - UVmask_G_opacity_bloor;
	Material.PreshaderBuffer[7].z = Tiling_texture_B;
	Material.PreshaderBuffer[7].w = Normal_Strengh_maskB;
	Material.PreshaderBuffer[8].x = UVmask_B_opacity;
	Material.PreshaderBuffer[8].y = UVmask_B_opacity_bloor + 1;
	Material.PreshaderBuffer[8].z = 0 - UVmask_B_opacity_bloor;
	Material.PreshaderBuffer[8].w = UVmask_A_opacity;
	Material.PreshaderBuffer[9].x = UVmask_A_opacity_bloor + 1;
	Material.PreshaderBuffer[9].y = 0 - UVmask_A_opacity_bloor;
	Material.PreshaderBuffer[9].z = Normal_RGBA_;
	Material.PreshaderBuffer[9].w = Brightness__opacity_bloor + 1;
	Material.PreshaderBuffer[10].x = 0 - Brightness__opacity_bloor;
	Material.PreshaderBuffer[10].y = Brightness__opacity;
	Material.PreshaderBuffer[11].xyz = emissive_color.xyz * emissive_power;
	Material.PreshaderBuffer[11].w = SelectionColor.w;
	Material.PreshaderBuffer[12].xyz = SelectionColor.xyz;
	Material.PreshaderBuffer[12].w = UTG_1_color_tone * 6.283;
	Material.PreshaderBuffer[13].x = UTG_1_Brightness;
	Material.PreshaderBuffer[13].y = UTG_1_contrast;
	Material.PreshaderBuffer[14].xyz = UTG_1_Tint_base_texture.xyz;
	Material.PreshaderBuffer[14].w = color_tone_maskR * 6.283;
	Material.PreshaderBuffer[15].x = Brightness_Base_maskR;
	Material.PreshaderBuffer[15].y = contrast_maskR;
	Material.PreshaderBuffer[16].xyz = maskR_Tint.xyz;
	Material.PreshaderBuffer[16].w = color_tone_maskG * 6.283;
	Material.PreshaderBuffer[17].x = Brightness_maskG;
	Material.PreshaderBuffer[17].y = contrast_maskG;
	Material.PreshaderBuffer[18].xyz = Tint_maskG.xyz;
	Material.PreshaderBuffer[18].w = color_tone_maskB * 6.283;
	Material.PreshaderBuffer[19].x = Brightness_maskB;
	Material.PreshaderBuffer[19].y = contrast_maskB;
	Material.PreshaderBuffer[20].xyz = Tint_maskB.xyz;
	Material.PreshaderBuffer[20].w = Brightness__Rchannel;
	Material.PreshaderBuffer[21].xyz = tint_Brightness_.xyz;
	Material.PreshaderBuffer[21].w = Metallic_Contrast_main_layer;
	Material.PreshaderBuffer[22].x = 1 - Metallic_Intensity_main_layer;
	Material.PreshaderBuffer[22].y = Metallic_Contrast_maskR;
	Material.PreshaderBuffer[22].z = 1 - Metallic_Intensity_maskR;
	Material.PreshaderBuffer[22].w = Metallic_Contrast_maskG;
	Material.PreshaderBuffer[23].x = 1 - Metallic_Intensity_maskG;
	Material.PreshaderBuffer[23].y = Metallic_Contrast_maskB;
	Material.PreshaderBuffer[23].z = 1 - Metallic_Intensity_maskB;
	Material.PreshaderBuffer[23].w = Specular_6;
	Material.PreshaderBuffer[24].x = UTG_2_Roughness_max;
	Material.PreshaderBuffer[24].y = UTG_2_Roughness_min;
	Material.PreshaderBuffer[24].z = Roughness_max_maskR;
	Material.PreshaderBuffer[24].w = Roughness_min_maskR;
	Material.PreshaderBuffer[25].x = Roughness_max_maskG;
	Material.PreshaderBuffer[25].y = Roughness_min_maskG;
	Material.PreshaderBuffer[25].z = Roughness_max_maskB;
	Material.PreshaderBuffer[25].w = Roughness_min_maskB;
	Material.PreshaderBuffer[26].x = Roughness_max_RGBA_;
	Material.PreshaderBuffer[26].y = UTG_4_Opacity_Mask_power;
	Material.PreshaderBuffer[26].z = UTG_1_SSS;
	Material.PreshaderBuffer[26].w = __SubsurfaceProfile;
	Material.PreshaderBuffer[27].x = UTG_2_AO_power;
	Material.PreshaderBuffer[27].y = AO_power_maskR;
	Material.PreshaderBuffer[27].z = AO_power_maskG;
	Material.PreshaderBuffer[27].w = AO_power_maskB;
}
float3 GetMaterialWorldPositionOffset(FMaterialVertexParameters Parameters)
{
SHADER_PUSH_WARNINGS_STATE
SHADER_DISABLE_WARNINGS
	return 0.00000000.rrr;;
SHADER_POP_WARNINGS_STATE
}
void CalcPixelMaterialInputs(in out FMaterialPixelParameters Parameters, in out FPixelMaterialInputs PixelMaterialInputs)
{
	//WorldAligned texturing & others use normals & stuff that think Z is up
	Parameters.TangentToWorld[0] = Parameters.TangentToWorld[0].xzy;
	Parameters.TangentToWorld[1] = Parameters.TangentToWorld[1].xzy;
	Parameters.TangentToWorld[2] = Parameters.TangentToWorld[2].xzy;

	float3 WorldNormalCopy = Parameters.WorldNormal;

SHADER_PUSH_WARNINGS_STATE
SHADER_DISABLE_WARNINGS
	// Initial calculations (required for Normal)
	MaterialFloat2 Local0 = Parameters.TexCoords[0].xy;
	MaterialFloat2 Local1 = (MaterialFloat2(-0.50000000,-0.50000000) + DERIV_BASE_VALUE(Local0));
	MaterialFloat Local2 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[0].xy);
	MaterialFloat Local3 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[0].zw);
	MaterialFloat2 Local4 = MaterialFloat2(DERIV_BASE_VALUE(Local2),DERIV_BASE_VALUE(Local3));
	MaterialFloat2 Local5 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local4));
	MaterialFloat2 Local6 = (DERIV_BASE_VALUE(Local5) + Material.PreshaderBuffer[1].xy);
	MaterialFloat2 Local7 = (DERIV_BASE_VALUE(Local6) * Material.PreshaderBuffer[1].zw);
	MaterialFloat2 Local8 = DDY((float2)DERIV_BASE_VALUE(Local7));
	MaterialFloat2 Local9 = DDX((float2)DERIV_BASE_VALUE(Local7));
	MaterialFloat2 Local10 = (DERIV_BASE_VALUE(Local9) * ((MaterialFloat2)View.MaterialTextureDerivativeMultiply));
	MaterialFloat2 Local11 = (DERIV_BASE_VALUE(Local8) * ((MaterialFloat2)View.MaterialTextureDerivativeMultiply));
	MaterialFloat Local12 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 2);
	MaterialFloat4 Local13 = UnpackNormalMap(Texture2DSampleGrad(Material_Texture2D_0,samplerMaterial_Texture2D_0,DERIV_BASE_VALUE(Local7),DERIV_BASE_VALUE(Local10),DERIV_BASE_VALUE(Local11)));
	MaterialFloat Local14 = MaterialStoreTexSample(Parameters, Local13, 2);
	MaterialFloat Local15 = (Local13.r * Material.PreshaderBuffer[2].x);
	MaterialFloat Local16 = (Local13.g * Material.PreshaderBuffer[2].x);
	MaterialFloat Local17 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[2].yz);
	MaterialFloat Local18 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[3].xy);
	MaterialFloat2 Local19 = MaterialFloat2(DERIV_BASE_VALUE(Local17),DERIV_BASE_VALUE(Local18));
	MaterialFloat2 Local20 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local19));
	MaterialFloat2 Local21 = (DERIV_BASE_VALUE(Local20) + Material.PreshaderBuffer[3].zw);
	MaterialFloat2 Local22 = (DERIV_BASE_VALUE(Local21) * Material.PreshaderBuffer[4].xy);
	MaterialFloat2 Local23 = DDY((float2)DERIV_BASE_VALUE(Local22));
	MaterialFloat2 Local24 = DDX((float2)DERIV_BASE_VALUE(Local22));
	MaterialFloat2 Local25 = (DERIV_BASE_VALUE(Local22) * ((MaterialFloat2)Material.PreshaderBuffer[4].z));
	MaterialFloat2 Local26 = (DERIV_BASE_VALUE(Local24) * ((MaterialFloat2)View.MaterialTextureDerivativeMultiply));
	MaterialFloat2 Local27 = (DERIV_BASE_VALUE(Local23) * ((MaterialFloat2)View.MaterialTextureDerivativeMultiply));
	MaterialFloat Local28 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local25), 2);
	MaterialFloat4 Local29 = UnpackNormalMap(Texture2DSampleGrad(Material_Texture2D_1,samplerMaterial_Texture2D_1,DERIV_BASE_VALUE(Local25),DERIV_BASE_VALUE(Local26),DERIV_BASE_VALUE(Local27)));
	MaterialFloat Local30 = MaterialStoreTexSample(Parameters, Local29, 2);
	MaterialFloat Local31 = (Local29.r * Material.PreshaderBuffer[4].w);
	MaterialFloat Local32 = (Local29.g * Material.PreshaderBuffer[4].w);
	MaterialFloat2 Local33 = Parameters.TexCoords[1].xy;
	MaterialFloat Local34 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local33), 1);
	MaterialFloat4 Local35 = Texture2DSampleBias(Material_Texture2D_2,samplerMaterial_Texture2D_2,DERIV_BASE_VALUE(Local33),View.MaterialTextureMipBias);
	MaterialFloat Local36 = MaterialStoreTexSample(Parameters, Local35, 1);
	MaterialFloat Local37 = (Local35.r * Material.PreshaderBuffer[5].x);
	MaterialFloat2 Local38 = (((MaterialFloat2)Material.PreshaderBuffer[5].y) * DERIV_BASE_VALUE(Local0));
	MaterialFloat Local39 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local38), 4);
	MaterialFloat4 Local40 = ProcessMaterialColorTextureLookup(Texture2DSampleBias(Material_Texture2D_3,samplerMaterial_Texture2D_3,DERIV_BASE_VALUE(Local38),View.MaterialTextureMipBias));
	MaterialFloat Local41 = MaterialStoreTexSample(Parameters, Local40, 4);
	MaterialFloat3 Local42 = (Local40.rgb * ((MaterialFloat3)Material.PreshaderBuffer[5].z));
	MaterialFloat3 Local43 = (((MaterialFloat3)Local35.r) + Local42);
	MaterialFloat3 Local44 = (((MaterialFloat3)Local37) * Local43);
	MaterialFloat Local45 = lerp(Material.PreshaderBuffer[6].x,Material.PreshaderBuffer[5].w,Local44.x);
	MaterialFloat Local46 = saturate(Local45);
	MaterialFloat Local47 = saturate(Local46.r);
	MaterialFloat3 Local48 = lerp(MaterialFloat3(MaterialFloat2(Local15,Local16),Local13.b),MaterialFloat3(MaterialFloat2(Local31,Local32),Local29.b),Local47);
	MaterialFloat2 Local49 = (DERIV_BASE_VALUE(Local22) * ((MaterialFloat2)Material.PreshaderBuffer[6].y));
	MaterialFloat Local50 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local49), 2);
	MaterialFloat4 Local51 = UnpackNormalMap(Texture2DSampleGrad(Material_Texture2D_4,samplerMaterial_Texture2D_4,DERIV_BASE_VALUE(Local49),DERIV_BASE_VALUE(Local26),DERIV_BASE_VALUE(Local27)));
	MaterialFloat Local52 = MaterialStoreTexSample(Parameters, Local51, 2);
	MaterialFloat Local53 = (Local51.r * Material.PreshaderBuffer[6].z);
	MaterialFloat Local54 = (Local51.g * Material.PreshaderBuffer[6].z);
	MaterialFloat Local55 = (Local35.g * Material.PreshaderBuffer[6].w);
	MaterialFloat3 Local56 = (((MaterialFloat3)Local35.g) + Local42);
	MaterialFloat3 Local57 = (((MaterialFloat3)Local55) * Local56);
	MaterialFloat Local58 = lerp(Material.PreshaderBuffer[7].y,Material.PreshaderBuffer[7].x,Local57.x);
	MaterialFloat Local59 = saturate(Local58);
	MaterialFloat Local60 = saturate(Local59.r);
	MaterialFloat3 Local61 = lerp(Local48,MaterialFloat3(MaterialFloat2(Local53,Local54),Local51.b),Local60);
	MaterialFloat2 Local62 = (DERIV_BASE_VALUE(Local22) * ((MaterialFloat2)Material.PreshaderBuffer[7].z));
	MaterialFloat Local63 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local62), 2);
	MaterialFloat4 Local64 = UnpackNormalMap(Texture2DSampleGrad(Material_Texture2D_5,samplerMaterial_Texture2D_5,DERIV_BASE_VALUE(Local62),DERIV_BASE_VALUE(Local26),DERIV_BASE_VALUE(Local27)));
	MaterialFloat Local65 = MaterialStoreTexSample(Parameters, Local64, 2);
	MaterialFloat Local66 = (Local64.r * Material.PreshaderBuffer[7].w);
	MaterialFloat Local67 = (Local64.g * Material.PreshaderBuffer[7].w);
	MaterialFloat Local68 = (Local35.b * Material.PreshaderBuffer[8].x);
	MaterialFloat3 Local69 = (((MaterialFloat3)Local35.b) + Local42);
	MaterialFloat3 Local70 = (((MaterialFloat3)Local68) * Local69);
	MaterialFloat Local71 = lerp(Material.PreshaderBuffer[8].z,Material.PreshaderBuffer[8].y,Local70.x);
	MaterialFloat Local72 = saturate(Local71);
	MaterialFloat Local73 = saturate(Local72.r);
	MaterialFloat3 Local74 = lerp(Local61,MaterialFloat3(MaterialFloat2(Local66,Local67),Local64.b),Local73);
	MaterialFloat Local75 = (Local35.a * Material.PreshaderBuffer[8].w);
	MaterialFloat3 Local76 = (((MaterialFloat3)Local35.a) + Local42);
	MaterialFloat3 Local77 = (((MaterialFloat3)Local75) * Local76);
	MaterialFloat Local78 = lerp(Material.PreshaderBuffer[9].y,Material.PreshaderBuffer[9].x,Local77.x);
	MaterialFloat Local79 = saturate(Local78);
	MaterialFloat Local80 = saturate(Local79.r);
	MaterialFloat3 Local81 = lerp(Local74,((MaterialFloat3)1.00000000),Local80);
	MaterialFloat Local82 = (Local81.r * Material.PreshaderBuffer[9].z);
	MaterialFloat Local83 = (Local81.g * Material.PreshaderBuffer[9].z);
	MaterialFloat3 Local84 = (Local81 * MaterialFloat3(MaterialFloat2(Local82,Local83),Local81.b));
	MaterialFloat4 Local85 = Texture2DSampleBias(Material_Texture2D_6,samplerMaterial_Texture2D_6,DERIV_BASE_VALUE(Local33),View.MaterialTextureMipBias);
	MaterialFloat Local86 = MaterialStoreTexSample(Parameters, Local85, 1);
	MaterialFloat Local87 = lerp(Material.PreshaderBuffer[10].x,Material.PreshaderBuffer[9].w,Local85.rgba.a);
	MaterialFloat Local88 = saturate(Local87);
	MaterialFloat Local89 = (Local88.r * Material.PreshaderBuffer[10].y);
	MaterialFloat Local90 = saturate(Local89);
	MaterialFloat3 Local91 = lerp(Local81,Local84,Local90);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local91;

SHADER_POP_WARNINGS_STATE

#if TEMPLATE_USES_SUBSTRATE
	Parameters.SubstratePixelFootprint = SubstrateGetPixelFootprint(Parameters.WorldPosition_CamRelative, GetRoughnessFromNormalCurvature(Parameters));
	Parameters.SharedLocalBases = SubstrateInitialiseSharedLocalBases();
	Parameters.SubstrateTree = GetInitialisedSubstrateTree();
#if SUBSTRATE_USE_FULLYSIMPLIFIED_MATERIAL == 1
	Parameters.SharedLocalBasesFullySimplified = SubstrateInitialiseSharedLocalBases();
	Parameters.SubstrateTreeFullySimplified = GetInitialisedSubstrateTree();
#endif
#endif

	// Note that here MaterialNormal can be in world space or tangent space
	float3 MaterialNormal = GetMaterialNormal(Parameters, PixelMaterialInputs);

#if MATERIAL_TANGENTSPACENORMAL

#if FEATURE_LEVEL >= FEATURE_LEVEL_SM4
	// Mobile will rely on only the final normalize for performance
	MaterialNormal = normalize(MaterialNormal);
#endif

	// normalizing after the tangent space to world space conversion improves quality with sheared bases (UV layout to WS causes shrearing)
	// use full precision normalize to avoid overflows
	Parameters.WorldNormal = TransformTangentNormalToWorld(Parameters.TangentToWorld, MaterialNormal);

#else //MATERIAL_TANGENTSPACENORMAL

	Parameters.WorldNormal = normalize(MaterialNormal);

#endif //MATERIAL_TANGENTSPACENORMAL

#if MATERIAL_TANGENTSPACENORMAL || TWO_SIDED_WORLD_SPACE_SINGLELAYERWATER_NORMAL
	// flip the normal for backfaces being rendered with a two-sided material
	Parameters.WorldNormal *= Parameters.TwoSidedSign;
#endif

	Parameters.ReflectionVector = ReflectionAboutCustomWorldNormal(Parameters, Parameters.WorldNormal, false);

#if !PARTICLE_SPRITE_FACTORY
	Parameters.Particle.MotionBlurFade = 1.0f;
#endif // !PARTICLE_SPRITE_FACTORY

SHADER_PUSH_WARNINGS_STATE
SHADER_DISABLE_WARNINGS
	// Now the rest of the inputs
	MaterialFloat3 Local92 = lerp(Material.PreshaderBuffer[11].xyz,Material.PreshaderBuffer[12].xyz,Material.PreshaderBuffer[11].w);
	MaterialFloat Local93 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 0);
	MaterialFloat4 Local94 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local95 = MaterialStoreTexSample(Parameters, Local94, 0);
	MaterialFloat3 Local96 = (Local94.rgb * ((MaterialFloat3)Material.PreshaderBuffer[13].x));
	MaterialFloat Local97 = dot(Local96,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local98 = lerp(Local96,((MaterialFloat3)Local97),Material.PreshaderBuffer[13].y);
	MaterialFloat3 Local99 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[12].w),((MaterialFloat3)0.00000000),Local98);
	MaterialFloat3 Local100 = (Local99 + Local98);
	MaterialFloat3 Local101 = (Material.PreshaderBuffer[14].xyz * Local100);
	MaterialFloat Local102 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local25), 0);
	MaterialFloat4 Local103 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_8,GetMaterialSharedSampler(samplerMaterial_Texture2D_8,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local25)));
	MaterialFloat Local104 = MaterialStoreTexSample(Parameters, Local103, 0);
	MaterialFloat3 Local105 = (Local103.rgb * ((MaterialFloat3)Material.PreshaderBuffer[15].x));
	MaterialFloat Local106 = dot(Local105,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local107 = lerp(Local105,((MaterialFloat3)Local106),Material.PreshaderBuffer[15].y);
	MaterialFloat3 Local108 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[14].w),((MaterialFloat3)0.00000000),Local107);
	MaterialFloat3 Local109 = (Local108 + Local107);
	MaterialFloat3 Local110 = (Material.PreshaderBuffer[16].xyz * Local109);
	MaterialFloat3 Local111 = lerp(Local101,Local110,Local47);
	MaterialFloat Local112 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local49), 0);
	MaterialFloat4 Local113 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_9,GetMaterialSharedSampler(samplerMaterial_Texture2D_9,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local49)));
	MaterialFloat Local114 = MaterialStoreTexSample(Parameters, Local113, 0);
	MaterialFloat3 Local115 = (Local113.rgb * ((MaterialFloat3)Material.PreshaderBuffer[17].x));
	MaterialFloat Local116 = dot(Local115,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local117 = lerp(Local115,((MaterialFloat3)Local116),Material.PreshaderBuffer[17].y);
	MaterialFloat3 Local118 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[16].w),((MaterialFloat3)0.00000000),Local117);
	MaterialFloat3 Local119 = (Local118 + Local117);
	MaterialFloat3 Local120 = (Material.PreshaderBuffer[18].xyz * Local119);
	MaterialFloat3 Local121 = lerp(Local111,Local120,Local60);
	MaterialFloat Local122 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local62), 0);
	MaterialFloat4 Local123 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_10,GetMaterialSharedSampler(samplerMaterial_Texture2D_10,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local62)));
	MaterialFloat Local124 = MaterialStoreTexSample(Parameters, Local123, 0);
	MaterialFloat3 Local125 = (Local123.rgb * ((MaterialFloat3)Material.PreshaderBuffer[19].x));
	MaterialFloat Local126 = dot(Local125,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local127 = lerp(Local125,((MaterialFloat3)Local126),Material.PreshaderBuffer[19].y);
	MaterialFloat3 Local128 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[18].w),((MaterialFloat3)0.00000000),Local127);
	MaterialFloat3 Local129 = (Local128 + Local127);
	MaterialFloat3 Local130 = (Material.PreshaderBuffer[20].xyz * Local129);
	MaterialFloat3 Local131 = lerp(Local121,Local130,Local73);
	MaterialFloat3 Local132 = lerp(Local131,((MaterialFloat3)1.00000000),Local80);
	MaterialFloat3 Local133 = (Local132 * ((MaterialFloat3)Material.PreshaderBuffer[20].w));
	MaterialFloat3 Local134 = (Material.PreshaderBuffer[21].xyz * Local133);
	MaterialFloat3 Local135 = lerp(Local132,Local134,Local90);
	MaterialFloat Local136 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 1);
	MaterialFloat4 Local137 = Texture2DSample(Material_Texture2D_11,GetMaterialSharedSampler(samplerMaterial_Texture2D_11,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7));
	MaterialFloat Local138 = MaterialStoreTexSample(Parameters, Local137, 1);
	MaterialFloat Local139 = PositiveClampedPow(Local137.b,Material.PreshaderBuffer[21].w);
	MaterialFloat Local140 = (Local139 + Material.PreshaderBuffer[22].x);
	MaterialFloat Local141 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local25), 1);
	MaterialFloat4 Local142 = Texture2DSample(Material_Texture2D_12,GetMaterialSharedSampler(samplerMaterial_Texture2D_12,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local25));
	MaterialFloat Local143 = MaterialStoreTexSample(Parameters, Local142, 1);
	MaterialFloat Local144 = PositiveClampedPow(Local142.b,Material.PreshaderBuffer[22].y);
	MaterialFloat Local145 = (Local144 + Material.PreshaderBuffer[22].z);
	MaterialFloat Local146 = lerp(Local140,Local145,Local47);
	MaterialFloat Local147 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local49), 1);
	MaterialFloat4 Local148 = Texture2DSample(Material_Texture2D_13,GetMaterialSharedSampler(samplerMaterial_Texture2D_13,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local49));
	MaterialFloat Local149 = MaterialStoreTexSample(Parameters, Local148, 1);
	MaterialFloat Local150 = PositiveClampedPow(Local148.b,Material.PreshaderBuffer[22].w);
	MaterialFloat Local151 = (Local150 + Material.PreshaderBuffer[23].x);
	MaterialFloat Local152 = lerp(Local146,Local151,Local60);
	MaterialFloat Local153 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local62), 1);
	MaterialFloat4 Local154 = Texture2DSample(Material_Texture2D_14,GetMaterialSharedSampler(samplerMaterial_Texture2D_14,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local62));
	MaterialFloat Local155 = MaterialStoreTexSample(Parameters, Local154, 1);
	MaterialFloat Local156 = PositiveClampedPow(Local154.b,Material.PreshaderBuffer[23].y);
	MaterialFloat Local157 = (Local156 + Material.PreshaderBuffer[23].z);
	MaterialFloat Local158 = lerp(Local152,Local157,Local73);
	MaterialFloat Local159 = lerp(Local158,1.00000000,Local80);
	MaterialFloat Local160 = lerp(Material.PreshaderBuffer[24].y,Material.PreshaderBuffer[24].x,Local137.g);
	MaterialFloat Local161 = lerp(Material.PreshaderBuffer[24].w,Material.PreshaderBuffer[24].z,Local142.g);
	MaterialFloat Local162 = lerp(Local160,Local161,Local47);
	MaterialFloat Local163 = lerp(Material.PreshaderBuffer[25].y,Material.PreshaderBuffer[25].x,Local148.g);
	MaterialFloat Local164 = lerp(Local162,Local163,Local60);
	MaterialFloat Local165 = lerp(Material.PreshaderBuffer[25].w,Material.PreshaderBuffer[25].z,Local154.g);
	MaterialFloat Local166 = lerp(Local164,Local165,Local73);
	MaterialFloat Local167 = lerp(Local166,1.00000000,Local80);
	MaterialFloat Local168 = lerp(Local167,Material.PreshaderBuffer[26].x,Local90);
	MaterialFloat Local169 = lerp(Local94.a,1.00000000,0.50000000);
	MaterialFloat Local170 = (Local169 * Material.PreshaderBuffer[26].y);
	MaterialFloat3 Local171 = (Local135 * ((MaterialFloat3)Material.PreshaderBuffer[26].z));
	MaterialFloat Local172 = PositiveClampedPow(Local137.r,Material.PreshaderBuffer[27].x);
	MaterialFloat Local173 = PositiveClampedPow(Local142.r,Material.PreshaderBuffer[27].y);
	MaterialFloat Local174 = lerp(Local172,Local173,Local47);
	MaterialFloat Local175 = PositiveClampedPow(Local148.r,Material.PreshaderBuffer[27].z);
	MaterialFloat Local176 = lerp(Local174,Local175,Local60);
	MaterialFloat Local177 = PositiveClampedPow(Local154.r,Material.PreshaderBuffer[27].w);
	MaterialFloat Local178 = lerp(Local176,Local177,Local73);
	MaterialFloat Local179 = lerp(Local178,1.00000000,Local80);

	PixelMaterialInputs.EmissiveColor = Local92;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = Local170;
	PixelMaterialInputs.BaseColor = Local135;
	PixelMaterialInputs.Metallic = Local159;
	PixelMaterialInputs.Specular = Material.PreshaderBuffer[23].w;
	PixelMaterialInputs.Roughness = Local168;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local91;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = MaterialFloat4(Local171,Material.PreshaderBuffer[26].w);
	PixelMaterialInputs.AmbientOcclusion = Local179;
	PixelMaterialInputs.Refraction = 0;
	PixelMaterialInputs.PixelDepthOffset = 0.00000000;
	PixelMaterialInputs.ShadingModel = 6;
	PixelMaterialInputs.FrontMaterial = GetInitialisedSubstrateData();
	PixelMaterialInputs.SurfaceThickness = 0.01000000;
	PixelMaterialInputs.Displacement = -1.00000000;

SHADER_POP_WARNINGS_STATE

#if MATERIAL_USES_ANISOTROPY
	Parameters.WorldTangent = CalculateAnisotropyTangent(Parameters, PixelMaterialInputs);
#else
	Parameters.WorldTangent = 0;
#endif
}

#define UnityObjectToWorldDir TransformObjectToWorld

void SetupCommonData( int Parameters_PrimitiveId )
{
	View_MaterialTextureBilinearWrapedSampler = SamplerState_Linear_Repeat;
	View_MaterialTextureBilinearClampedSampler = SamplerState_Linear_Clamp;

	Material_Wrap_WorldGroupSettings = SamplerState_Linear_Repeat;
	Material_Clamp_WorldGroupSettings = SamplerState_Linear_Clamp;

	View.GameTime = View.RealTime = _Time.y;// _Time is (t/20, t, t*2, t*3)
	View.PrevFrameGameTime = View.GameTime - unity_DeltaTime.x;//(dt, 1/dt, smoothDt, 1/smoothDt)
	View.PrevFrameRealTime = View.RealTime;
	View.DeltaTime = unity_DeltaTime.x;
	View.MaterialTextureMipBias = 0.0;
	View.TemporalAAParams = float4( 0, 0, 0, 0 );
	View.ViewRectMin = float2( 0, 0 );
	View.ViewSizeAndInvSize = View_BufferSizeAndInvSize;
	View.ResolutionFractionAndInv = float2( View_BufferSizeAndInvSize.x / View_BufferSizeAndInvSize.y, 1.0 / ( View_BufferSizeAndInvSize.x / View_BufferSizeAndInvSize.y ));
	View.MaterialTextureDerivativeMultiply = 1.0f;
	View.StateFrameIndexMod8 = 0;
	View.FrameNumber = (int)_Time.y;
	View.FieldOfViewWideAngles = float2( PI * 0.42f, PI * 0.42f );//75degrees, default unity
	View.RuntimeVirtualTextureMipLevel = float4( 0, 0, 0, 0 );
	View.PreExposure = 0;
	View.BufferBilinearUVMinMax = float4(
		View_BufferSizeAndInvSize.z * ( 0 + 0.5 ),//EffectiveViewRect.Min.X
		View_BufferSizeAndInvSize.w * ( 0 + 0.5 ),//EffectiveViewRect.Min.Y
		View_BufferSizeAndInvSize.z * ( View_BufferSizeAndInvSize.x - 0.5 ),//EffectiveViewRect.Max.X
		View_BufferSizeAndInvSize.w * ( View_BufferSizeAndInvSize.y - 0.5 ) );//EffectiveViewRect.Max.Y

	for( int i2 = 0; i2 < 40; i2++ )
		View.PrimitiveSceneData[ i2 ] = float4( 0, 0, 0, 0 );

	float4x4 LocalToWorld = transpose( UNITY_MATRIX_M );
    LocalToWorld[3] = float4(ToUnrealPos(LocalToWorld[3]), LocalToWorld[3].w);
	float4x4 WorldToLocal = transpose( UNITY_MATRIX_I_M );
	float4x4 ViewMatrix = transpose( UNITY_MATRIX_V );
	float4x4 InverseViewMatrix = transpose( UNITY_MATRIX_I_V );
	float4x4 ViewProjectionMatrix = transpose( UNITY_MATRIX_VP );
	uint PrimitiveBaseOffset = Parameters_PrimitiveId * PRIMITIVE_SCENE_DATA_STRIDE;
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 0 ] = LocalToWorld[ 0 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 1 ] = LocalToWorld[ 1 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 2 ] = LocalToWorld[ 2 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 3 ] = LocalToWorld[ 3 ];//LocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 5 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 100.0 );//ObjectWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 6 ] = WorldToLocal[ 0 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 7 ] = WorldToLocal[ 1 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 8 ] = WorldToLocal[ 2 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 9 ] = WorldToLocal[ 3 ];//WorldToLocal
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 10 ] = LocalToWorld[ 0 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 11 ] = LocalToWorld[ 1 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 12 ] = LocalToWorld[ 2 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 13 ] = LocalToWorld[ 3 ];//PreviousLocalToWorld
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 18 ] = float4( ToUnrealPos( SHADERGRAPH_OBJECT_POSITION ), 0 );//ActorWorldPosition
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 19 ] = LocalObjectBoundsMax - LocalObjectBoundsMin;//ObjectBounds
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 21 ] = mul( LocalToWorld, float3( 1, 0, 0 ) );
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 23 ] = LocalObjectBoundsMin;//LocalObjectBoundsMin 
	View.PrimitiveSceneData[ PrimitiveBaseOffset + 24 ] = LocalObjectBoundsMax;//LocalObjectBoundsMax

#ifdef UE5
	ResolvedView.WorldCameraOrigin = LWCPromote( ToUnrealPos( _WorldSpaceCameraPos.xyz ) );
	ResolvedView.PreViewTranslation = LWCPromote( float3( 0, 0, 0 ) );
	ResolvedView.WorldViewOrigin = LWCPromote( float3( 0, 0, 0 ) );
#else
	ResolvedView.WorldCameraOrigin = ToUnrealPos( _WorldSpaceCameraPos.xyz );
	ResolvedView.PreViewTranslation = float3( 0, 0, 0 );
	ResolvedView.WorldViewOrigin = float3( 0, 0, 0 );
#endif
	ResolvedView.PrevWorldCameraOrigin = ResolvedView.WorldCameraOrigin;
	ResolvedView.ScreenPositionScaleBias = float4( 1, 1, 0, 0 );
	ResolvedView.TranslatedWorldToView		 = ViewMatrix;
	ResolvedView.TranslatedWorldToCameraView = ViewMatrix;
	ResolvedView.TranslatedWorldToClip		 = ViewProjectionMatrix;
	ResolvedView.ViewToTranslatedWorld		 = InverseViewMatrix;
	ResolvedView.PrevViewToTranslatedWorld = ResolvedView.ViewToTranslatedWorld;
	ResolvedView.CameraViewToTranslatedWorld = InverseViewMatrix;
	ResolvedView.BufferBilinearUVMinMax = View.BufferBilinearUVMinMax;
	Primitive.WorldToLocal = WorldToLocal;
	Primitive.LocalToWorld = LocalToWorld;
}
#define VS_USES_UNREAL_SPACE 1
float3 PrepareAndGetWPO( float4 VertexColor, float3 UnrealWorldPos, float3 UnrealNormal, float4 InTangent,
						 float4 UV0, float4 UV1 )
{
	InitializeExpressions();
	FMaterialVertexParameters Parameters = (FMaterialVertexParameters)0;

	float3 InWorldNormal = UnrealNormal;
	float4 tangentWorld = InTangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	
	#ifdef VS_USES_UNREAL_SPACE
		UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	#endif
	Parameters.WorldPosition = UnrealWorldPos;
	#ifdef VS_USES_UNREAL_SPACE
		Parameters.TangentToWorld[ 0 ] = Parameters.TangentToWorld[ 0 ].xzy;
		Parameters.TangentToWorld[ 1 ] = Parameters.TangentToWorld[ 1 ].xzy;
		Parameters.TangentToWorld[ 2 ] = Parameters.TangentToWorld[ 2 ].xzy;//WorldAligned texturing uses normals that think Z is up
	#endif

	Parameters.VertexColor = VertexColor;

#if NUM_MATERIAL_TEXCOORDS_VERTEX > 0			
	Parameters.TexCoords[ 0 ] = float2( UV0.x, UV0.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 1
	Parameters.TexCoords[ 1 ] = float2( UV1.x, UV1.y );
#endif
#if NUM_MATERIAL_TEXCOORDS_VERTEX > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( UV0.x, UV0.y );
	}
#endif

	Parameters.PrimitiveId = 0;

	SetupCommonData( Parameters.PrimitiveId );

#ifdef UE5
	Parameters.PrevFrameLocalToWorld = MakeLWCMatrix( float3( 0, 0, 0 ), Primitive.LocalToWorld );
#else
	Parameters.PrevFrameLocalToWorld = Primitive.LocalToWorld;
#endif
	
	float3 Offset = float3( 0, 0, 0 );
	Offset = GetMaterialWorldPositionOffset( Parameters );
	#ifdef VS_USES_UNREAL_SPACE
		//Convert from unreal units to unity
		Offset /= float3( 100, 100, 100 );
		Offset = Offset.xzy;
	#endif
	return Offset;
}

void SurfaceReplacement( Input In, out SurfaceOutputStandard o )
{
	InitializeExpressions();

	float3 Z3 = float3( 0, 0, 0 );
	float4 Z4 = float4( 0, 0, 0, 0 );

	float3 UnrealWorldPos = float3( In.worldPos.x, In.worldPos.y, In.worldPos.z );

	float3 UnrealNormal = In.normal2;	

	FMaterialPixelParameters Parameters = (FMaterialPixelParameters)0;
#if NUM_TEX_COORD_INTERPOLATORS > 0			
	Parameters.TexCoords[ 0 ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 1
	Parameters.TexCoords[ 1 ] = float2( In.uv2_Material_Texture2D_0.x, 1.0 - In.uv2_Material_Texture2D_0.y );
#endif
#if NUM_TEX_COORD_INTERPOLATORS > 2
	for( int i = 2; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
	{
		Parameters.TexCoords[ i ] = float2( In.uv_MainTex.x, 1.0 - In.uv_MainTex.y );
	}
#endif
	Parameters.PostProcessUV = In.uv_MainTex;
	Parameters.VertexColor = In.color;
	Parameters.WorldNormal = UnrealNormal;
	Parameters.ReflectionVector = half3( 0, 0, 1 );
	//Parameters.CameraVector = normalize( _WorldSpaceCameraPos.xyz - UnrealWorldPos.xyz );
	//Parameters.CameraVector = mul( ( float3x3 )unity_CameraToWorld, float3( 0, 0, 1 ) ) * -1;	
	float3 CameraDirection = (-1 * mul((float3x3)UNITY_MATRIX_M, transpose(mul(UNITY_MATRIX_I_M, UNITY_MATRIX_I_V)) [2].xyz));//From ShaderGraph
	Parameters.CameraVector = CameraDirection;
	Parameters.LightVector = half3( 0, 0, 0 );
	float4 screenpos = In.screenPos;
	screenpos /= screenpos.w;
	Parameters.SvPosition = screenpos;
	Parameters.ScreenPosition = Parameters.SvPosition;

	Parameters.UnMirrored = 1;

	Parameters.TwoSidedSign = 1;


	float3 InWorldNormal = UnrealNormal;	
	float4 tangentWorld = In.tangent;
	tangentWorld.xyz = normalize( tangentWorld.xyz );
	//float3x3 tangentToWorld = CreateTangentToWorldPerVertex( InWorldNormal, tangentWorld.xyz, tangentWorld.w );
	Parameters.TangentToWorld = float3x3( normalize( cross( InWorldNormal, tangentWorld.xyz ) * tangentWorld.w ), tangentWorld.xyz, InWorldNormal );

	//WorldAlignedTexturing in UE relies on the fact that coords there are 100x larger, prepare values for that
	//but watch out for any computation that might get skewed as a side effect
	UnrealWorldPos = ToUnrealPos( UnrealWorldPos );
	
	Parameters.AbsoluteWorldPosition = UnrealWorldPos;
	Parameters.WorldPosition_CamRelative = UnrealWorldPos;
	Parameters.WorldPosition_NoOffsets = UnrealWorldPos;

	Parameters.WorldPosition_NoOffsets_CamRelative = Parameters.WorldPosition_CamRelative;
	Parameters.LightingPositionOffset = float3( 0, 0, 0 );

	Parameters.AOMaterialMask = 0;

	Parameters.Particle.RelativeTime = 0;
	Parameters.Particle.MotionBlurFade;
	Parameters.Particle.Random = 0;
	Parameters.Particle.Velocity = half4( 1, 1, 1, 1 );
	Parameters.Particle.Color = half4( 1, 1, 1, 1 );
	Parameters.Particle.TranslatedWorldPositionAndSize = float4( UnrealWorldPos, 0 );
	Parameters.Particle.MacroUV = half4( 0, 0, 1, 1 );
	Parameters.Particle.DynamicParameter = half4( 0, 0, 0, 0 );
	Parameters.Particle.LocalToWorld = float4x4( Z4, Z4, Z4, Z4 );
	Parameters.Particle.Size = float2( 1, 1 );
	Parameters.Particle.SubUVCoords[ 0 ] = Parameters.Particle.SubUVCoords[ 1 ] = float2( 0, 0 );
	Parameters.Particle.SubUVLerp = 0.0;
	Parameters.TexCoordScalesParams = float2( 0, 0 );
	Parameters.PrimitiveId = 0;
	Parameters.VirtualTextureFeedback = 0;

	FPixelMaterialInputs PixelMaterialInputs = (FPixelMaterialInputs)0;
	PixelMaterialInputs.Normal = float3( 0, 0, 1 );
	PixelMaterialInputs.ShadingModel = 0;
	//PixelMaterialInputs.FrontMaterial = GetStrataUnlitBSDF( float3( 0, 0, 0 ), float3( 0, 0, 0 ) );

	SetupCommonData( Parameters.PrimitiveId );
	//CustomizedUVs
	#if NUM_TEX_COORD_INTERPOLATORS > 0 && HAS_CUSTOMIZED_UVS
		float2 OutTexCoords[ NUM_TEX_COORD_INTERPOLATORS ];
		//Prevent uninitialized reads
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			OutTexCoords[ i ] = float2( 0, 0 );
		}
		GetMaterialCustomizedUVs( Parameters, OutTexCoords );
		for( int i = 0; i < NUM_TEX_COORD_INTERPOLATORS; i++ )
		{
			Parameters.TexCoords[ i ] = OutTexCoords[ i ];
		}
	#endif
	//<-
	CalcPixelMaterialInputs( Parameters, PixelMaterialInputs );

	#define HAS_WORLDSPACE_NORMAL 0
	#if HAS_WORLDSPACE_NORMAL
		PixelMaterialInputs.Normal = mul( PixelMaterialInputs.Normal, (MaterialFloat3x3)( transpose( Parameters.TangentToWorld ) ) );
	#endif

	o.Albedo = PixelMaterialInputs.BaseColor.rgb;
	o.Alpha = PixelMaterialInputs.Opacity;
	if( PixelMaterialInputs.OpacityMask < 0.333 ) discard;
	//o.Alpha = PixelMaterialInputs.OpacityMask;

	o.Metallic = PixelMaterialInputs.Metallic;
	o.Smoothness = 1.0 - PixelMaterialInputs.Roughness;
	o.Normal = normalize( PixelMaterialInputs.Normal );
	o.Emission = PixelMaterialInputs.EmissiveColor.rgb;
	o.Occlusion = PixelMaterialInputs.AmbientOcclusion;
}