#define NUM_TEX_COORD_INTERPOLATORS 1
#define NUM_MATERIAL_TEXCOORDS_VERTEX 1
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
TEXTURE2D(       Material_Texture2D_15 );
SAMPLER(  samplerMaterial_Texture2D_15 );
float4 Material_Texture2D_15_TexelSize;
float4 Material_Texture2D_15_ST;
TEXTURE2D(       Material_Texture2D_16 );
SAMPLER(  samplerMaterial_Texture2D_16 );
float4 Material_Texture2D_16_TexelSize;
float4 Material_Texture2D_16_ST;
TEXTURE2D(       Material_Texture2D_17 );
SAMPLER(  samplerMaterial_Texture2D_17 );
float4 Material_Texture2D_17_TexelSize;
float4 Material_Texture2D_17_ST;
TEXTURE2D(       Material_Texture2D_18 );
SAMPLER(  samplerMaterial_Texture2D_18 );
float4 Material_Texture2D_18_TexelSize;
float4 Material_Texture2D_18_ST;
TEXTURE2D(       Material_Texture2D_19 );
SAMPLER(  samplerMaterial_Texture2D_19 );
float4 Material_Texture2D_19_TexelSize;
float4 Material_Texture2D_19_ST;
TEXTURE2D(       Material_Texture2D_20 );
SAMPLER(  samplerMaterial_Texture2D_20 );
float4 Material_Texture2D_20_TexelSize;
float4 Material_Texture2D_20_ST;
TEXTURE2D(       Material_Texture2D_21 );
SAMPLER(  samplerMaterial_Texture2D_21 );
float4 Material_Texture2D_21_TexelSize;
float4 Material_Texture2D_21_ST;
uniform float4 SelectionColor;
uniform float Rotation_custom_main_layer;
uniform float U_tile_main_layer;
uniform float V_tile_main_layer;
uniform float UV_Tiling_main_layer;
uniform float Base_UV___U_Tiling_main_layer;
uniform float Base_UV___V_Tiling_main_layer;
uniform float Normal_intensity_main_layer;
uniform float threshold_BCmask_layer_1;
uniform float4 color_mask_BCmask_layer_1;
uniform float threshold_BCmask_layer_2;
uniform float4 color_mask_BCmask_layer_2;
uniform float threshold_BCmask_layer_3;
uniform float4 color_mask_BCmask_layer_3;
uniform float Rotation_custom_R_channel;
uniform float U_tile_R_channel;
uniform float V_tile_R_channel;
uniform float UV_Tiling_R_channel;
uniform float Base_UV___U_Tiling_R_channel;
uniform float Base_UV___V_Tiling_R_channel;
uniform float Normal_intensity_R_channel;
uniform float R_channel_contrast;
uniform float Disp_Contrast_main_layer;
uniform float Disp_Intensity_main_layer;
uniform float Disp_Contrast_R_channel;
uniform float Disp_Intensity_R_channel;
uniform float R_channel_blend;
uniform float R_channel_extend;
uniform float UVmask_R_opacity_bloor;
uniform float UVmask_R_opacity;
uniform float Grunge_tiling_R_channel;
uniform float Grunge_channel_R_power;
uniform float Rotation_custom_G_channel;
uniform float U_tile_G_channel;
uniform float V_tile_G_channel;
uniform float UV_Tiling_G_channel;
uniform float Base_UV___U_Tiling_G_channel;
uniform float Base_UV___V_Tiling_G_channel;
uniform float Normal_intensity_G_channel;
uniform float G_channel_contrast;
uniform float Disp_Contrast_G_channel;
uniform float Disp_Intensity_G_channel;
uniform float G_channel_blend;
uniform float G_channel_extend;
uniform float UVmask_G_opacity_bloor;
uniform float UVmask_G_opacity;
uniform float Grunge_tiling_G_channel;
uniform float Grunge_channel_G_power;
uniform float Rotation_custom_B_channel;
uniform float U_tile_B_channel;
uniform float V_tile_B_channel;
uniform float UV_Tiling_B_channel;
uniform float Base_UV___U_Tiling_B_channel;
uniform float Base_UV___V_Tiling_B_channel;
uniform float Normal_intensity_B_channel;
uniform float B_channel_contrast;
uniform float Disp_Contrast_B_channel;
uniform float Disp_Intensity_B_channel;
uniform float B_channel_blend;
uniform float B_channel_extend;
uniform float UVmask_B_opacity_bloor;
uniform float UVmask_B_opacity;
uniform float Grunge_tiling_B_channel;
uniform float Grunge_channel_B_power;
uniform float UVmask_Ab_opacity_bloor;
uniform float UVmask_Ab_opacity;
uniform float Grunge_tiling_Ab_channel;
uniform float Grunge_channel_Ab_power;
uniform float color_tone_main_layer;
uniform float Brightness_main_layer;
uniform float contrast_main_layer;
uniform float4 Tint_base_color_main_layer;
uniform float color_tone_BCmask_layer_1;
uniform float Rotation_custom_BCmask_layer_1;
uniform float U_tile_BCmask_layer_1;
uniform float V_tile_BCmask_layer_1;
uniform float UV_Tiling_BCmask_layer_1;
uniform float Base_UV___U_Tiling_BCmask_layer_1;
uniform float Base_UV___V_Tiling_BCmask_layer_1;
uniform float Brightness_BCmask_layer_1;
uniform float contrast_BCmask_layer_1;
uniform float4 Tint_base_color_BCmask_layer_1;
uniform float color_tone_BCmask_layer_2;
uniform float Rotation_custom_BCmask_layer_2;
uniform float U_tile_BCmask_layer_2;
uniform float V_tile_BCmask_layer_2;
uniform float UV_Tiling_BCmask_layer_2;
uniform float Base_UV___U_Tiling_BCmask_layer_2;
uniform float Base_UV___V_Tiling_BCmask_layer_2;
uniform float Brightness_BCmask_layer_2;
uniform float contrast_BCmask_layer_2;
uniform float4 Tint_base_color_BCmask_layer_2;
uniform float color_tone_BCmask_layer_3;
uniform float Rotation_custom_BCmask_layer_3;
uniform float U_tile_BCmask_layer_3;
uniform float V_tile_BCmask_layer_3;
uniform float UV_Tiling_BCmask_layer_3;
uniform float Base_UV___U_Tiling_BCmask_layer_3;
uniform float Base_UV___V_Tiling_BCmask_layer_3;
uniform float Brightness_BCmask_layer_3;
uniform float contrast_BCmask_layer_3;
uniform float4 Tint_base_color_BCmask_layer_3;
uniform float color_tone_R_channel;
uniform float Brightness_R_channel;
uniform float contrast_R_channel;
uniform float4 Tint_base_color_R_channel;
uniform float color_tone_G_channel;
uniform float Brightness_G_channel;
uniform float contrast_G_channel;
uniform float4 Tint_base_color_G_channel;
uniform float color_tone_B_channel;
uniform float Brightness_B_channel;
uniform float contrast_B_channel;
uniform float4 Tint_base_color_B_channel;
uniform float color_tone_B_all_mask;
uniform float Brightness_B_all_mask;
uniform float contrast_B_all_mask;
uniform float4 Tint_base_color_B_all_mask;
uniform float Metallic_main_layer;
uniform float Metallic_R_channel;
uniform float Metallic_G_channel;
uniform float Metallic_B_channel;
uniform float Specular_main_layer;
uniform float Specular_R_channel;
uniform float Specular_G_channel;
uniform float Specular_B_channel;
uniform float Roughness_min_main_layer;
uniform float Roughness_max_main_layer;
uniform float Roughness_min_BCmask_layer_1;
uniform float Roughness_max_BCmask_layer_1;
uniform float Roughness_min_BCmask_layer_2;
uniform float Roughness_max_BCmask_layer_2;
uniform float Roughness_min_BCmask_layer_3;
uniform float Roughness_max_BCmask_layer_3;
uniform float Roughness_min_R_channel;
uniform float Roughness_max_R_channel;
uniform float Roughness_min_G_channel;
uniform float Roughness_max_G_channel;
uniform float Roughness_min_B_channel;
uniform float Roughness_max_B_channel;
uniform float Roughness_min_B_all_mask;
uniform float Roughness_max_B_all_mask;
uniform float AO_Contrast_main_layer;
uniform float AO_Intensity_main_layer;
uniform float AO_Contrast_R_channel;
uniform float AO_Intensity_R_channel;
uniform float AO_Contrast_G_channel;
uniform float AO_Intensity_G_channel;
uniform float AO_Contrast_B_channel;
uniform float AO_Intensity_B_channel;

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
	float4 PreshaderBuffer[56];
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
	Material.PreshaderBuffer[1] = float4(0.000000,0.000000,10.000000,10.000000);//(Unknown)
	Material.PreshaderBuffer[2] = float4(1.000000,0.891387,0.000000,0.984000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(2.000000,0.967448,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[4] = float4(1.000000,0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[5] = float4(0.300000,1.000000,-0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[6] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[7] = float4(10.000000,10.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[8] = float4(0.000000,1.000000,0.420000,0.580000);//(Unknown)
	Material.PreshaderBuffer[9] = float4(1.000000,1.000000,0.300000,1.000000);//(Unknown)
	Material.PreshaderBuffer[10] = float4(150.000000,4.000000,2.000000,-1.000000);//(Unknown)
	Material.PreshaderBuffer[11] = float4(2.000000,-1.000000,1.000000,-0.000000);//(Unknown)
	Material.PreshaderBuffer[12] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[13] = float4(5.000000,5.000000,1.000000,0.756000);//(Unknown)
	Material.PreshaderBuffer[14] = float4(0.244000,1.000000,1.000000,0.300000);//(Unknown)
	Material.PreshaderBuffer[15] = float4(1.000000,150.000000,4.000000,2.000000);//(Unknown)
	Material.PreshaderBuffer[16] = float4(-1.000000,2.000000,-1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[17] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[18] = float4(0.000000,0.000000,19.000000,19.000000);//(Unknown)
	Material.PreshaderBuffer[19] = float4(1.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[20] = float4(3.318073,-0.658752,1.000000,150.000000);//(Unknown)
	Material.PreshaderBuffer[21] = float4(4.000000,2.000000,-1.000000,11.000000);//(Unknown)
	Material.PreshaderBuffer[22] = float4(-10.000000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[23] = float4(2.000000,-1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[24] = float4(0.000000,0.000000,0.000000,6.283185);//(Unknown)
	Material.PreshaderBuffer[25] = float4(0.616000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[26] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[27] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[28] = float4(0.000000,0.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[29] = float4(0.455472,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[30] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[31] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[32] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[33] = float4(0.000000,0.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[34] = float4(0.536000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[35] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[36] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[37] = float4(0.000000,0.000000,10.000000,10.000000);//(Unknown)
	Material.PreshaderBuffer[38] = float4(0.488000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[39] = float4(0.588163,0.794290,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[40] = float4(0.700000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[41] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[42] = float4(0.750000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[43] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[44] = float4(1.000000,0.264000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[45] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[46] = float4(0.720000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[47] = float4(1.000000,0.957842,0.933594,0.000000);//(Unknown)
	Material.PreshaderBuffer[48] = float4(0.000000,0.000000,0.000000,0.150000);//(Unknown)
	Material.PreshaderBuffer[49] = float4(0.150000,0.150000,0.150000,1.000000);//(Unknown)
	Material.PreshaderBuffer[50] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[51] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[52] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[53] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[54] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[55] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)

	Material.PreshaderBuffer[0].xy = Append(Cos( Rotation_custom_main_layer * 6.283 ), Sin( Rotation_custom_main_layer * 6.283 ) * -1);
	Material.PreshaderBuffer[0].zw = Append(Sin( Rotation_custom_main_layer * 6.283 ), Cos( Rotation_custom_main_layer * 6.283 ));
	Material.PreshaderBuffer[1].xy = Append(U_tile_main_layer, V_tile_main_layer);
	Material.PreshaderBuffer[1].zw = UV_Tiling_main_layer * Append(Base_UV___U_Tiling_main_layer, Base_UV___V_Tiling_main_layer);
	Material.PreshaderBuffer[2].x = Normal_intensity_main_layer;
	Material.PreshaderBuffer[2].yzw = color_mask_BCmask_layer_1.xyz;
	Material.PreshaderBuffer[3].x = threshold_BCmask_layer_1;
	Material.PreshaderBuffer[3].yzw = color_mask_BCmask_layer_2.xyz;
	Material.PreshaderBuffer[4].x = threshold_BCmask_layer_2;
	Material.PreshaderBuffer[4].yzw = color_mask_BCmask_layer_3.xyz;
	Material.PreshaderBuffer[5].x = threshold_BCmask_layer_3;
	Material.PreshaderBuffer[5].yz = Append(Cos( Rotation_custom_R_channel * 6.283 ), Sin( Rotation_custom_R_channel * 6.283 ) * -1);
	Material.PreshaderBuffer[6].xy = Append(Sin( Rotation_custom_R_channel * 6.283 ), Cos( Rotation_custom_R_channel * 6.283 ));
	Material.PreshaderBuffer[6].zw = Append(U_tile_R_channel, V_tile_R_channel);
	Material.PreshaderBuffer[7].xy = UV_Tiling_R_channel * Append(Base_UV___U_Tiling_R_channel, Base_UV___V_Tiling_R_channel);
	Material.PreshaderBuffer[7].z = Normal_intensity_R_channel;
	Material.PreshaderBuffer[7].w = Disp_Contrast_main_layer + 1;
	Material.PreshaderBuffer[8].x = 0 - Disp_Contrast_main_layer;
	Material.PreshaderBuffer[8].y = Disp_Intensity_main_layer;
	Material.PreshaderBuffer[8].z = Disp_Contrast_R_channel + 1;
	Material.PreshaderBuffer[8].w = 0 - Disp_Contrast_R_channel;
	Material.PreshaderBuffer[9].x = Disp_Intensity_R_channel;
	Material.PreshaderBuffer[9].y = R_channel_blend;
	Material.PreshaderBuffer[9].z = R_channel_extend;
	Material.PreshaderBuffer[9].w = UVmask_R_opacity;
	Material.PreshaderBuffer[10].x = Grunge_tiling_R_channel;
	Material.PreshaderBuffer[10].y = Grunge_channel_R_power;
	Material.PreshaderBuffer[10].z = UVmask_R_opacity_bloor + 1;
	Material.PreshaderBuffer[10].w = 0 - UVmask_R_opacity_bloor;
	Material.PreshaderBuffer[11].x = R_channel_contrast + 1;
	Material.PreshaderBuffer[11].y = 0 - R_channel_contrast;
	Material.PreshaderBuffer[11].zw = Append(Cos( Rotation_custom_G_channel * 6.283 ), Sin( Rotation_custom_G_channel * 6.283 ) * -1);
	Material.PreshaderBuffer[12].xy = Append(Sin( Rotation_custom_G_channel * 6.283 ), Cos( Rotation_custom_G_channel * 6.283 ));
	Material.PreshaderBuffer[12].zw = Append(U_tile_G_channel, V_tile_G_channel);
	Material.PreshaderBuffer[13].xy = UV_Tiling_G_channel * Append(Base_UV___U_Tiling_G_channel, Base_UV___V_Tiling_G_channel);
	Material.PreshaderBuffer[13].z = Normal_intensity_G_channel;
	Material.PreshaderBuffer[13].w = Disp_Contrast_G_channel + 1;
	Material.PreshaderBuffer[14].x = 0 - Disp_Contrast_G_channel;
	Material.PreshaderBuffer[14].y = Disp_Intensity_G_channel;
	Material.PreshaderBuffer[14].z = G_channel_blend;
	Material.PreshaderBuffer[14].w = G_channel_extend;
	Material.PreshaderBuffer[15].x = UVmask_G_opacity;
	Material.PreshaderBuffer[15].y = Grunge_tiling_G_channel;
	Material.PreshaderBuffer[15].z = Grunge_channel_G_power;
	Material.PreshaderBuffer[15].w = UVmask_G_opacity_bloor + 1;
	Material.PreshaderBuffer[16].x = 0 - UVmask_G_opacity_bloor;
	Material.PreshaderBuffer[16].y = G_channel_contrast + 1;
	Material.PreshaderBuffer[16].z = 0 - G_channel_contrast;
	Material.PreshaderBuffer[17].xy = Append(Cos( Rotation_custom_B_channel * 6.283 ), Sin( Rotation_custom_B_channel * 6.283 ) * -1);
	Material.PreshaderBuffer[17].zw = Append(Sin( Rotation_custom_B_channel * 6.283 ), Cos( Rotation_custom_B_channel * 6.283 ));
	Material.PreshaderBuffer[18].xy = Append(U_tile_B_channel, V_tile_B_channel);
	Material.PreshaderBuffer[18].zw = UV_Tiling_B_channel * Append(Base_UV___U_Tiling_B_channel, Base_UV___V_Tiling_B_channel);
	Material.PreshaderBuffer[19].x = Normal_intensity_B_channel;
	Material.PreshaderBuffer[19].y = Disp_Contrast_B_channel + 1;
	Material.PreshaderBuffer[19].z = 0 - Disp_Contrast_B_channel;
	Material.PreshaderBuffer[19].w = Disp_Intensity_B_channel;
	Material.PreshaderBuffer[20].x = B_channel_blend;
	Material.PreshaderBuffer[20].y = B_channel_extend;
	Material.PreshaderBuffer[20].z = UVmask_B_opacity;
	Material.PreshaderBuffer[20].w = Grunge_tiling_B_channel;
	Material.PreshaderBuffer[21].x = Grunge_channel_B_power;
	Material.PreshaderBuffer[21].y = UVmask_B_opacity_bloor + 1;
	Material.PreshaderBuffer[21].z = 0 - UVmask_B_opacity_bloor;
	Material.PreshaderBuffer[21].w = B_channel_contrast + 1;
	Material.PreshaderBuffer[22].x = 0 - B_channel_contrast;
	Material.PreshaderBuffer[22].y = UVmask_Ab_opacity;
	Material.PreshaderBuffer[22].z = Grunge_tiling_Ab_channel;
	Material.PreshaderBuffer[22].w = Grunge_channel_Ab_power;
	Material.PreshaderBuffer[23].x = UVmask_Ab_opacity_bloor + 1;
	Material.PreshaderBuffer[23].y = 0 - UVmask_Ab_opacity_bloor;
	Material.PreshaderBuffer[23].z = SelectionColor.w;
	Material.PreshaderBuffer[24].xyz = SelectionColor.xyz;
	Material.PreshaderBuffer[24].w = color_tone_main_layer * 6.283;
	Material.PreshaderBuffer[25].x = Brightness_main_layer;
	Material.PreshaderBuffer[25].y = contrast_main_layer;
	Material.PreshaderBuffer[26].xyz = Tint_base_color_main_layer.xyz;
	Material.PreshaderBuffer[26].w = color_tone_BCmask_layer_1 * 6.283;
	Material.PreshaderBuffer[27].xy = Append(Cos( Rotation_custom_BCmask_layer_1 * 6.283 ), Sin( Rotation_custom_BCmask_layer_1 * 6.283 ) * -1);
	Material.PreshaderBuffer[27].zw = Append(Sin( Rotation_custom_BCmask_layer_1 * 6.283 ), Cos( Rotation_custom_BCmask_layer_1 * 6.283 ));
	Material.PreshaderBuffer[28].xy = Append(U_tile_BCmask_layer_1, V_tile_BCmask_layer_1);
	Material.PreshaderBuffer[28].zw = UV_Tiling_BCmask_layer_1 * Append(Base_UV___U_Tiling_BCmask_layer_1, Base_UV___V_Tiling_BCmask_layer_1);
	Material.PreshaderBuffer[29].x = Brightness_BCmask_layer_1;
	Material.PreshaderBuffer[29].y = contrast_BCmask_layer_1;
	Material.PreshaderBuffer[30].xyz = Tint_base_color_BCmask_layer_1.xyz;
	Material.PreshaderBuffer[30].w = color_tone_BCmask_layer_2 * 6.283;
	Material.PreshaderBuffer[32].xy = Append(Cos( Rotation_custom_BCmask_layer_2 * 6.283 ), Sin( Rotation_custom_BCmask_layer_2 * 6.283 ) * -1);
	Material.PreshaderBuffer[32].zw = Append(Sin( Rotation_custom_BCmask_layer_2 * 6.283 ), Cos( Rotation_custom_BCmask_layer_2 * 6.283 ));
	Material.PreshaderBuffer[33].xy = Append(U_tile_BCmask_layer_2, V_tile_BCmask_layer_2);
	Material.PreshaderBuffer[33].zw = UV_Tiling_BCmask_layer_2 * Append(Base_UV___U_Tiling_BCmask_layer_2, Base_UV___V_Tiling_BCmask_layer_2);
	Material.PreshaderBuffer[34].x = Brightness_BCmask_layer_2;
	Material.PreshaderBuffer[34].y = contrast_BCmask_layer_2;
	Material.PreshaderBuffer[35].xyz = Tint_base_color_BCmask_layer_2.xyz;
	Material.PreshaderBuffer[35].w = color_tone_BCmask_layer_3 * 6.283;
	Material.PreshaderBuffer[36].xy = Append(Cos( Rotation_custom_BCmask_layer_3 * 6.283 ), Sin( Rotation_custom_BCmask_layer_3 * 6.283 ) * -1);
	Material.PreshaderBuffer[36].zw = Append(Sin( Rotation_custom_BCmask_layer_3 * 6.283 ), Cos( Rotation_custom_BCmask_layer_3 * 6.283 ));
	Material.PreshaderBuffer[37].xy = Append(U_tile_BCmask_layer_3, V_tile_BCmask_layer_3);
	Material.PreshaderBuffer[37].zw = UV_Tiling_BCmask_layer_3 * Append(Base_UV___U_Tiling_BCmask_layer_3, Base_UV___V_Tiling_BCmask_layer_3);
	Material.PreshaderBuffer[38].x = Brightness_BCmask_layer_3;
	Material.PreshaderBuffer[38].y = contrast_BCmask_layer_3;
	Material.PreshaderBuffer[39].xyz = Tint_base_color_BCmask_layer_3.xyz;
	Material.PreshaderBuffer[39].w = color_tone_R_channel * 6.283;
	Material.PreshaderBuffer[40].x = Brightness_R_channel;
	Material.PreshaderBuffer[40].y = contrast_R_channel;
	Material.PreshaderBuffer[41].xyz = Tint_base_color_R_channel.xyz;
	Material.PreshaderBuffer[41].w = color_tone_G_channel * 6.283;
	Material.PreshaderBuffer[42].x = Brightness_G_channel;
	Material.PreshaderBuffer[42].y = contrast_G_channel;
	Material.PreshaderBuffer[43].xyz = Tint_base_color_G_channel.xyz;
	Material.PreshaderBuffer[43].w = color_tone_B_channel * 6.283;
	Material.PreshaderBuffer[44].x = Brightness_B_channel;
	Material.PreshaderBuffer[44].y = contrast_B_channel;
	Material.PreshaderBuffer[45].xyz = Tint_base_color_B_channel.xyz;
	Material.PreshaderBuffer[45].w = color_tone_B_all_mask * 6.283;
	Material.PreshaderBuffer[46].x = Brightness_B_all_mask;
	Material.PreshaderBuffer[46].y = contrast_B_all_mask;
	Material.PreshaderBuffer[47].xyz = Tint_base_color_B_all_mask.xyz;
	Material.PreshaderBuffer[47].w = Metallic_main_layer;
	Material.PreshaderBuffer[48].x = Metallic_R_channel;
	Material.PreshaderBuffer[48].y = Metallic_G_channel;
	Material.PreshaderBuffer[48].z = Metallic_B_channel;
	Material.PreshaderBuffer[48].w = Specular_main_layer;
	Material.PreshaderBuffer[49].x = Specular_R_channel;
	Material.PreshaderBuffer[49].y = Specular_G_channel;
	Material.PreshaderBuffer[49].z = Specular_B_channel;
	Material.PreshaderBuffer[49].w = Roughness_max_main_layer;
	Material.PreshaderBuffer[50].x = Roughness_min_main_layer;
	Material.PreshaderBuffer[50].y = Roughness_max_BCmask_layer_1;
	Material.PreshaderBuffer[50].z = Roughness_min_BCmask_layer_1;
	Material.PreshaderBuffer[50].w = Roughness_max_BCmask_layer_2;
	Material.PreshaderBuffer[51].x = Roughness_min_BCmask_layer_2;
	Material.PreshaderBuffer[51].y = Roughness_max_BCmask_layer_3;
	Material.PreshaderBuffer[51].z = Roughness_min_BCmask_layer_3;
	Material.PreshaderBuffer[51].w = Roughness_max_R_channel;
	Material.PreshaderBuffer[52].x = Roughness_min_R_channel;
	Material.PreshaderBuffer[52].y = Roughness_max_G_channel;
	Material.PreshaderBuffer[52].z = Roughness_min_G_channel;
	Material.PreshaderBuffer[52].w = Roughness_max_B_channel;
	Material.PreshaderBuffer[53].x = Roughness_min_B_channel;
	Material.PreshaderBuffer[53].y = Roughness_max_B_all_mask;
	Material.PreshaderBuffer[53].z = Roughness_min_B_all_mask;
	Material.PreshaderBuffer[53].w = AO_Contrast_main_layer;
	Material.PreshaderBuffer[54].x = 1 - AO_Intensity_main_layer;
	Material.PreshaderBuffer[54].y = AO_Contrast_R_channel;
	Material.PreshaderBuffer[54].z = 1 - AO_Intensity_R_channel;
	Material.PreshaderBuffer[54].w = AO_Contrast_G_channel;
	Material.PreshaderBuffer[55].x = 1 - AO_Intensity_G_channel;
	Material.PreshaderBuffer[55].y = AO_Contrast_B_channel;
	Material.PreshaderBuffer[55].z = 1 - AO_Intensity_B_channel;
}
float3 GetMaterialWorldPositionOffset(FMaterialVertexParameters Parameters)
{
	return MaterialFloat3(0.00000000,0.00000000,0.00000000);;
}
void CalcPixelMaterialInputs(in out FMaterialPixelParameters Parameters, in out FPixelMaterialInputs PixelMaterialInputs)
{
	//WorldAligned texturing & others use normals & stuff that think Z is up
	Parameters.TangentToWorld[0] = Parameters.TangentToWorld[0].xzy;
	Parameters.TangentToWorld[1] = Parameters.TangentToWorld[1].xzy;
	Parameters.TangentToWorld[2] = Parameters.TangentToWorld[2].xzy;

	float3 WorldNormalCopy = Parameters.WorldNormal;

	// Initial calculations (required for Normal)
	MaterialFloat2 Local0 = Parameters.TexCoords[0].xy;
	MaterialFloat2 Local1 = (MaterialFloat2(-0.50000000,-0.50000000) + DERIV_BASE_VALUE(Local0));
	MaterialFloat Local2 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[0].xy);
	MaterialFloat Local3 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[0].zw);
	MaterialFloat2 Local4 = MaterialFloat2(DERIV_BASE_VALUE(Local2),DERIV_BASE_VALUE(Local3));
	MaterialFloat2 Local5 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local4));
	MaterialFloat2 Local6 = (DERIV_BASE_VALUE(Local5) + Material.PreshaderBuffer[1].xy);
	MaterialFloat2 Local7 = (DERIV_BASE_VALUE(Local6) * Material.PreshaderBuffer[1].zw);
	MaterialFloat Local8 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 5);
	MaterialFloat4 Local9 = UnpackNormalMap(Texture2DSample(Material_Texture2D_0,GetMaterialSharedSampler(samplerMaterial_Texture2D_0,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local10 = MaterialStoreTexSample(Parameters, Local9, 5);
	MaterialFloat Local11 = (Local9.rgb.r * Material.PreshaderBuffer[2].x);
	MaterialFloat Local12 = (Local9.rgb.g * Material.PreshaderBuffer[2].x);
	MaterialFloat Local13 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local0), 10);
	MaterialFloat4 Local14 = Texture2DSample(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local0));
	MaterialFloat Local15 = MaterialStoreTexSample(Parameters, Local14, 10);
	MaterialFloat3 Local16 = (Local14.rgb - Material.PreshaderBuffer[2].yzw);
	MaterialFloat Local17 = length(Local16);
	MaterialFloat Local18 = (Local17.r + Local17.r);
	MaterialFloat Local19 = (Local18 + Local17.r);
	MaterialFloat Local20 = (Material.PreshaderBuffer[3].x * Local19);
	MaterialFloat Local21 = saturate(Local20);
	MaterialFloat Local22 = (1.00000000 - Local21);
	MaterialFloat3 Local23 = (Local14.rgb - Material.PreshaderBuffer[3].yzw);
	MaterialFloat Local24 = length(Local23);
	MaterialFloat Local25 = (Local24.r + Local24.r);
	MaterialFloat Local26 = (Local25 + Local24.r);
	MaterialFloat Local27 = (Material.PreshaderBuffer[4].x * Local26);
	MaterialFloat Local28 = saturate(Local27);
	MaterialFloat Local29 = (1.00000000 - Local28);
	MaterialFloat3 Local30 = (Local14.rgb - Material.PreshaderBuffer[4].yzw);
	MaterialFloat Local31 = length(Local30);
	MaterialFloat Local32 = (Local31.r + Local31.r);
	MaterialFloat Local33 = (Local32 + Local31.r);
	MaterialFloat Local34 = (Material.PreshaderBuffer[5].x * Local33);
	MaterialFloat Local35 = saturate(Local34);
	MaterialFloat Local36 = (1.00000000 - Local35);
	MaterialFloat Local37 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[5].yz);
	MaterialFloat Local38 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[6].xy);
	MaterialFloat2 Local39 = MaterialFloat2(DERIV_BASE_VALUE(Local37),DERIV_BASE_VALUE(Local38));
	MaterialFloat2 Local40 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local39));
	MaterialFloat2 Local41 = (DERIV_BASE_VALUE(Local40) + Material.PreshaderBuffer[6].zw);
	MaterialFloat2 Local42 = (DERIV_BASE_VALUE(Local41) * Material.PreshaderBuffer[7].xy);
	MaterialFloat Local43 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local42), 5);
	MaterialFloat4 Local44 = UnpackNormalMap(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local42)));
	MaterialFloat Local45 = MaterialStoreTexSample(Parameters, Local44, 5);
	MaterialFloat Local46 = (Local44.rgb.r * Material.PreshaderBuffer[7].z);
	MaterialFloat Local47 = (Local44.rgb.g * Material.PreshaderBuffer[7].z);
	MaterialFloat Local48 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 7);
	MaterialFloat4 Local49 = Texture2DSample(Material_Texture2D_3,GetMaterialSharedSampler(samplerMaterial_Texture2D_3,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7));
	MaterialFloat Local50 = MaterialStoreTexSample(Parameters, Local49, 7);
	MaterialFloat Local51 = lerp(Material.PreshaderBuffer[8].x,Material.PreshaderBuffer[7].w,Local49.b);
	MaterialFloat Local52 = saturate(Local51);
	MaterialFloat Local53 = (Local52.r * Material.PreshaderBuffer[8].y);
	MaterialFloat Local54 = saturate(Local53);
	MaterialFloat Local55 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local42), 7);
	MaterialFloat4 Local56 = Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(samplerMaterial_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local42));
	MaterialFloat Local57 = MaterialStoreTexSample(Parameters, Local56, 7);
	MaterialFloat Local58 = lerp(Material.PreshaderBuffer[8].w,Material.PreshaderBuffer[8].z,Local56.b);
	MaterialFloat Local59 = saturate(Local58);
	MaterialFloat Local60 = (Local59.r * Material.PreshaderBuffer[9].x);
	MaterialFloat Local61 = lerp(Local54,Local60,Material.PreshaderBuffer[9].y);
	MaterialFloat Local62 = (1.00000000 - Local61);
	MaterialFloat Local63 = (Local62 * Material.PreshaderBuffer[9].z);
	MaterialFloat Local64 = (Local63 - 1.00000000);
	MaterialFloat Local65 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local0), 8);
	MaterialFloat4 Local66 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local0)));
	MaterialFloat Local67 = MaterialStoreTexSample(Parameters, Local66, 8);
	MaterialFloat Local68 = (Local66.r * Material.PreshaderBuffer[9].w);
	MaterialFloat2 Local69 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[10].x));
	MaterialFloat Local70 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local69), 9);
	MaterialFloat4 Local71 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_6,GetMaterialSharedSampler(samplerMaterial_Texture2D_6,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local69)));
	MaterialFloat Local72 = MaterialStoreTexSample(Parameters, Local71, 9);
	MaterialFloat3 Local73 = (Local71.rgb * ((MaterialFloat3)Material.PreshaderBuffer[10].y));
	MaterialFloat3 Local74 = (((MaterialFloat3)Local66.r) + Local73);
	MaterialFloat3 Local75 = (((MaterialFloat3)Local68) * Local74);
	MaterialFloat Local76 = lerp(Material.PreshaderBuffer[10].w,Material.PreshaderBuffer[10].z,Local75.x);
	MaterialFloat Local77 = saturate(Local76);
	MaterialFloat Local78 = saturate(Local77.r);
	MaterialFloat Local79 = (Local78 * 2.00000000);
	MaterialFloat Local80 = (Local64 + Local79);
	MaterialFloat Local81 = saturate(Local80);
	MaterialFloat Local82 = lerp(Material.PreshaderBuffer[11].y,Material.PreshaderBuffer[11].x,Local81);
	MaterialFloat Local83 = saturate(Local82);
	MaterialFloat3 Local84 = lerp(MaterialFloat3(MaterialFloat2(Local11,Local12),Local9.rgb.b),MaterialFloat3(MaterialFloat2(Local46,Local47),Local44.rgb.b),Local83.r);
	MaterialFloat Local85 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[11].zw);
	MaterialFloat Local86 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[12].xy);
	MaterialFloat2 Local87 = MaterialFloat2(DERIV_BASE_VALUE(Local85),DERIV_BASE_VALUE(Local86));
	MaterialFloat2 Local88 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local87));
	MaterialFloat2 Local89 = (DERIV_BASE_VALUE(Local88) + Material.PreshaderBuffer[12].zw);
	MaterialFloat2 Local90 = (DERIV_BASE_VALUE(Local89) * Material.PreshaderBuffer[13].xy);
	MaterialFloat Local91 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local90), 5);
	MaterialFloat4 Local92 = UnpackNormalMap(Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local90)));
	MaterialFloat Local93 = MaterialStoreTexSample(Parameters, Local92, 5);
	MaterialFloat Local94 = (Local92.rgb.r * Material.PreshaderBuffer[13].z);
	MaterialFloat Local95 = (Local92.rgb.g * Material.PreshaderBuffer[13].z);
	MaterialFloat Local96 = lerp(Local53,Local60,Local83.r);
	MaterialFloat Local97 = saturate(Local96);
	MaterialFloat Local98 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local90), 7);
	MaterialFloat4 Local99 = Texture2DSample(Material_Texture2D_8,GetMaterialSharedSampler(samplerMaterial_Texture2D_8,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local90));
	MaterialFloat Local100 = MaterialStoreTexSample(Parameters, Local99, 7);
	MaterialFloat Local101 = lerp(Material.PreshaderBuffer[14].x,Material.PreshaderBuffer[13].w,Local99.b);
	MaterialFloat Local102 = saturate(Local101);
	MaterialFloat Local103 = (Local102.r * Material.PreshaderBuffer[14].y);
	MaterialFloat Local104 = lerp(Local97,Local103,Material.PreshaderBuffer[14].z);
	MaterialFloat Local105 = (1.00000000 - Local104);
	MaterialFloat Local106 = (Local105 * Material.PreshaderBuffer[14].w);
	MaterialFloat Local107 = (Local106 - 1.00000000);
	MaterialFloat Local108 = (Local66.g * Material.PreshaderBuffer[15].x);
	MaterialFloat2 Local109 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[15].y));
	MaterialFloat Local110 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local109), 9);
	MaterialFloat4 Local111 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_9,GetMaterialSharedSampler(samplerMaterial_Texture2D_9,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local109)));
	MaterialFloat Local112 = MaterialStoreTexSample(Parameters, Local111, 9);
	MaterialFloat3 Local113 = (Local111.rgb * ((MaterialFloat3)Material.PreshaderBuffer[15].z));
	MaterialFloat3 Local114 = (((MaterialFloat3)Local66.g) + Local113);
	MaterialFloat3 Local115 = (((MaterialFloat3)Local108) * Local114);
	MaterialFloat Local116 = lerp(Material.PreshaderBuffer[16].x,Material.PreshaderBuffer[15].w,Local115.x);
	MaterialFloat Local117 = saturate(Local116);
	MaterialFloat Local118 = saturate(Local117.r);
	MaterialFloat Local119 = (Local118 * 2.00000000);
	MaterialFloat Local120 = (Local107 + Local119);
	MaterialFloat Local121 = saturate(Local120);
	MaterialFloat Local122 = lerp(Material.PreshaderBuffer[16].z,Material.PreshaderBuffer[16].y,Local121);
	MaterialFloat Local123 = saturate(Local122);
	MaterialFloat3 Local124 = lerp(Local84,MaterialFloat3(MaterialFloat2(Local94,Local95),Local92.rgb.b),Local123.r);
	MaterialFloat Local125 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[17].xy);
	MaterialFloat Local126 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[17].zw);
	MaterialFloat2 Local127 = MaterialFloat2(DERIV_BASE_VALUE(Local125),DERIV_BASE_VALUE(Local126));
	MaterialFloat2 Local128 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local127));
	MaterialFloat2 Local129 = (DERIV_BASE_VALUE(Local128) + Material.PreshaderBuffer[18].xy);
	MaterialFloat2 Local130 = (DERIV_BASE_VALUE(Local129) * Material.PreshaderBuffer[18].zw);
	MaterialFloat Local131 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local130), 5);
	MaterialFloat4 Local132 = UnpackNormalMap(Texture2DSample(Material_Texture2D_10,GetMaterialSharedSampler(samplerMaterial_Texture2D_10,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local130)));
	MaterialFloat Local133 = MaterialStoreTexSample(Parameters, Local132, 5);
	MaterialFloat Local134 = (Local132.rgb.r * Material.PreshaderBuffer[19].x);
	MaterialFloat Local135 = (Local132.rgb.g * Material.PreshaderBuffer[19].x);
	MaterialFloat Local136 = lerp(Local96,Local103,Local123.r);
	MaterialFloat Local137 = saturate(Local136);
	MaterialFloat Local138 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local130), 7);
	MaterialFloat4 Local139 = Texture2DSample(Material_Texture2D_11,GetMaterialSharedSampler(samplerMaterial_Texture2D_11,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local130));
	MaterialFloat Local140 = MaterialStoreTexSample(Parameters, Local139, 7);
	MaterialFloat Local141 = lerp(Material.PreshaderBuffer[19].z,Material.PreshaderBuffer[19].y,Local139.b);
	MaterialFloat Local142 = saturate(Local141);
	MaterialFloat Local143 = (Local142.r * Material.PreshaderBuffer[19].w);
	MaterialFloat Local144 = lerp(Local137,Local143,Material.PreshaderBuffer[20].x);
	MaterialFloat Local145 = (1.00000000 - Local144);
	MaterialFloat Local146 = (Local145 * Material.PreshaderBuffer[20].y);
	MaterialFloat Local147 = (Local146 - 1.00000000);
	MaterialFloat Local148 = (Local66.b * Material.PreshaderBuffer[20].z);
	MaterialFloat2 Local149 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[20].w));
	MaterialFloat Local150 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local149), 9);
	MaterialFloat4 Local151 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_12,GetMaterialSharedSampler(samplerMaterial_Texture2D_12,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local149)));
	MaterialFloat Local152 = MaterialStoreTexSample(Parameters, Local151, 9);
	MaterialFloat3 Local153 = (Local151.rgb * ((MaterialFloat3)Material.PreshaderBuffer[21].x));
	MaterialFloat3 Local154 = (((MaterialFloat3)Local66.b) + Local153);
	MaterialFloat3 Local155 = (((MaterialFloat3)Local148) * Local154);
	MaterialFloat Local156 = lerp(Material.PreshaderBuffer[21].z,Material.PreshaderBuffer[21].y,Local155.x);
	MaterialFloat Local157 = saturate(Local156);
	MaterialFloat Local158 = saturate(Local157.r);
	MaterialFloat Local159 = (Local158 * 2.00000000);
	MaterialFloat Local160 = (Local147 + Local159);
	MaterialFloat Local161 = saturate(Local160);
	MaterialFloat Local162 = lerp(Material.PreshaderBuffer[22].x,Material.PreshaderBuffer[21].w,Local161);
	MaterialFloat Local163 = saturate(Local162);
	MaterialFloat3 Local164 = lerp(Local124,MaterialFloat3(MaterialFloat2(Local134,Local135),Local132.rgb.b),Local163.r);
	MaterialFloat Local165 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local0), 11);
	MaterialFloat4 Local166 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_13,GetMaterialSharedSampler(samplerMaterial_Texture2D_13,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local0)));
	MaterialFloat Local167 = MaterialStoreTexSample(Parameters, Local166, 11);
	MaterialFloat Local168 = (Local166.a * Material.PreshaderBuffer[22].y);
	MaterialFloat2 Local169 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[22].z));
	MaterialFloat Local170 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local169), 9);
	MaterialFloat4 Local171 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_14,GetMaterialSharedSampler(samplerMaterial_Texture2D_14,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local169)));
	MaterialFloat Local172 = MaterialStoreTexSample(Parameters, Local171, 9);
	MaterialFloat3 Local173 = (Local171.rgb * ((MaterialFloat3)Material.PreshaderBuffer[22].w));
	MaterialFloat3 Local174 = (((MaterialFloat3)Local166.a) + Local173);
	MaterialFloat3 Local175 = (((MaterialFloat3)Local168) * Local174);
	MaterialFloat Local176 = lerp(Material.PreshaderBuffer[23].y,Material.PreshaderBuffer[23].x,Local175.x);
	MaterialFloat Local177 = saturate(Local176);
	MaterialFloat Local178 = saturate(Local177.r);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local164;


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

	// Now the rest of the inputs
	MaterialFloat3 Local179 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[24].xyz,Material.PreshaderBuffer[23].z);
	MaterialFloat Local180 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 6);
	MaterialFloat4 Local181 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_15,GetMaterialSharedSampler(samplerMaterial_Texture2D_15,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local182 = MaterialStoreTexSample(Parameters, Local181, 6);
	MaterialFloat3 Local183 = (Local181.rgb * ((MaterialFloat3)Material.PreshaderBuffer[25].x));
	MaterialFloat Local184 = dot(Local183,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local185 = lerp(Local183,((MaterialFloat3)Local184),Material.PreshaderBuffer[25].y);
	MaterialFloat3 Local186 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[24].w),((MaterialFloat3)0.00000000),Local185);
	MaterialFloat3 Local187 = (Local186 + Local185);
	MaterialFloat3 Local188 = (Local187 * Material.PreshaderBuffer[26].xyz);
	MaterialFloat Local189 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[27].xy);
	MaterialFloat Local190 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[27].zw);
	MaterialFloat2 Local191 = MaterialFloat2(DERIV_BASE_VALUE(Local189),DERIV_BASE_VALUE(Local190));
	MaterialFloat2 Local192 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local191));
	MaterialFloat2 Local193 = (DERIV_BASE_VALUE(Local192) + Material.PreshaderBuffer[28].xy);
	MaterialFloat2 Local194 = (DERIV_BASE_VALUE(Local193) * Material.PreshaderBuffer[28].zw);
	MaterialFloat Local195 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local194), 6);
	MaterialFloat4 Local196 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_16,GetMaterialSharedSampler(samplerMaterial_Texture2D_16,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local194)));
	MaterialFloat Local197 = MaterialStoreTexSample(Parameters, Local196, 6);
	MaterialFloat3 Local198 = (Local196.rgb * ((MaterialFloat3)Material.PreshaderBuffer[29].x));
	MaterialFloat Local199 = dot(Local198,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local200 = lerp(Local198,((MaterialFloat3)Local199),Material.PreshaderBuffer[29].y);
	MaterialFloat3 Local201 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[26].w),((MaterialFloat3)0.00000000),Local200);
	MaterialFloat3 Local202 = (Local201 + Local200);
	MaterialFloat3 Local203 = (Local202 * Material.PreshaderBuffer[30].xyz);
	MaterialFloat3 Local204 = lerp(Local188,Local203,Local22);
	MaterialFloat Local205 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[32].xy);
	MaterialFloat Local206 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[32].zw);
	MaterialFloat2 Local207 = MaterialFloat2(DERIV_BASE_VALUE(Local205),DERIV_BASE_VALUE(Local206));
	MaterialFloat2 Local208 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local207));
	MaterialFloat2 Local209 = (DERIV_BASE_VALUE(Local208) + Material.PreshaderBuffer[33].xy);
	MaterialFloat2 Local210 = (DERIV_BASE_VALUE(Local209) * Material.PreshaderBuffer[33].zw);
	MaterialFloat Local211 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local210), 6);
	MaterialFloat4 Local212 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_17,GetMaterialSharedSampler(samplerMaterial_Texture2D_17,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local210)));
	MaterialFloat Local213 = MaterialStoreTexSample(Parameters, Local212, 6);
	MaterialFloat3 Local214 = (Local212.rgb * ((MaterialFloat3)Material.PreshaderBuffer[34].x));
	MaterialFloat Local215 = dot(Local214,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local216 = lerp(Local214,((MaterialFloat3)Local215),Material.PreshaderBuffer[34].y);
	MaterialFloat3 Local217 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[30].w),((MaterialFloat3)0.00000000),Local216);
	MaterialFloat3 Local218 = (Local217 + Local216);
	MaterialFloat3 Local219 = (Local218 * Material.PreshaderBuffer[35].xyz);
	MaterialFloat3 Local220 = lerp(Local204,Local219,Local29);
	MaterialFloat Local221 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[36].xy);
	MaterialFloat Local222 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[36].zw);
	MaterialFloat2 Local223 = MaterialFloat2(DERIV_BASE_VALUE(Local221),DERIV_BASE_VALUE(Local222));
	MaterialFloat2 Local224 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local223));
	MaterialFloat2 Local225 = (DERIV_BASE_VALUE(Local224) + Material.PreshaderBuffer[37].xy);
	MaterialFloat2 Local226 = (DERIV_BASE_VALUE(Local225) * Material.PreshaderBuffer[37].zw);
	MaterialFloat Local227 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local226), 6);
	MaterialFloat4 Local228 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_18,GetMaterialSharedSampler(samplerMaterial_Texture2D_18,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local226)));
	MaterialFloat Local229 = MaterialStoreTexSample(Parameters, Local228, 6);
	MaterialFloat3 Local230 = (Local228.rgb * ((MaterialFloat3)Material.PreshaderBuffer[38].x));
	MaterialFloat Local231 = dot(Local230,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local232 = lerp(Local230,((MaterialFloat3)Local231),Material.PreshaderBuffer[38].y);
	MaterialFloat3 Local233 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[35].w),((MaterialFloat3)0.00000000),Local232);
	MaterialFloat3 Local234 = (Local233 + Local232);
	MaterialFloat3 Local235 = (Local234 * Material.PreshaderBuffer[39].xyz);
	MaterialFloat3 Local236 = lerp(Local220,Local235,Local36);
	MaterialFloat Local237 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local42), 6);
	MaterialFloat4 Local238 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_19,GetMaterialSharedSampler(samplerMaterial_Texture2D_19,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local42)));
	MaterialFloat Local239 = MaterialStoreTexSample(Parameters, Local238, 6);
	MaterialFloat3 Local240 = (Local238.rgb * ((MaterialFloat3)Material.PreshaderBuffer[40].x));
	MaterialFloat Local241 = dot(Local240,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local242 = lerp(Local240,((MaterialFloat3)Local241),Material.PreshaderBuffer[40].y);
	MaterialFloat3 Local243 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[39].w),((MaterialFloat3)0.00000000),Local242);
	MaterialFloat3 Local244 = (Local243 + Local242);
	MaterialFloat3 Local245 = (Local244 * Material.PreshaderBuffer[41].xyz);
	MaterialFloat3 Local246 = lerp(Local236,Local245,Local83.r);
	MaterialFloat Local247 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local90), 6);
	MaterialFloat4 Local248 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_20,GetMaterialSharedSampler(samplerMaterial_Texture2D_20,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local90)));
	MaterialFloat Local249 = MaterialStoreTexSample(Parameters, Local248, 6);
	MaterialFloat3 Local250 = (Local248.rgb * ((MaterialFloat3)Material.PreshaderBuffer[42].x));
	MaterialFloat Local251 = dot(Local250,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local252 = lerp(Local250,((MaterialFloat3)Local251),Material.PreshaderBuffer[42].y);
	MaterialFloat3 Local253 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[41].w),((MaterialFloat3)0.00000000),Local252);
	MaterialFloat3 Local254 = (Local253 + Local252);
	MaterialFloat3 Local255 = (Local254 * Material.PreshaderBuffer[43].xyz);
	MaterialFloat3 Local256 = lerp(Local246,Local255,Local123.r);
	MaterialFloat Local257 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local130), 6);
	MaterialFloat4 Local258 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_21,GetMaterialSharedSampler(samplerMaterial_Texture2D_21,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local130)));
	MaterialFloat Local259 = MaterialStoreTexSample(Parameters, Local258, 6);
	MaterialFloat3 Local260 = (Local258.rgb * ((MaterialFloat3)Material.PreshaderBuffer[44].x));
	MaterialFloat Local261 = dot(Local260,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local262 = lerp(Local260,((MaterialFloat3)Local261),Material.PreshaderBuffer[44].y);
	MaterialFloat3 Local263 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[43].w),((MaterialFloat3)0.00000000),Local262);
	MaterialFloat3 Local264 = (Local263 + Local262);
	MaterialFloat3 Local265 = (Local264 * Material.PreshaderBuffer[45].xyz);
	MaterialFloat3 Local266 = lerp(Local256,Local265,Local163.r);
	MaterialFloat3 Local267 = (Local266 * ((MaterialFloat3)Material.PreshaderBuffer[46].x));
	MaterialFloat Local268 = dot(Local267,MaterialFloat3(0.30000001,0.58999997,0.11000000));
	MaterialFloat3 Local269 = lerp(Local267,((MaterialFloat3)Local268),Material.PreshaderBuffer[46].y);
	MaterialFloat3 Local270 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[45].w),((MaterialFloat3)0.00000000),Local269);
	MaterialFloat3 Local271 = (Local270 + Local269);
	MaterialFloat3 Local272 = (Local271 * Material.PreshaderBuffer[47].xyz);
	MaterialFloat3 Local273 = lerp(Local266,Local272,Local178);
	MaterialFloat Local274 = lerp(Material.PreshaderBuffer[47].w,Material.PreshaderBuffer[48].x,Local83.r);
	MaterialFloat Local275 = lerp(Local274,Material.PreshaderBuffer[48].y,Local123.r);
	MaterialFloat Local276 = lerp(Local275,Material.PreshaderBuffer[48].z,Local163.r);
	MaterialFloat Local277 = lerp(Material.PreshaderBuffer[48].w,Material.PreshaderBuffer[49].x,Local83.r);
	MaterialFloat Local278 = lerp(Local277,Material.PreshaderBuffer[49].y,Local123.r);
	MaterialFloat Local279 = lerp(Local278,Material.PreshaderBuffer[49].z,Local163.r);
	MaterialFloat Local280 = lerp(Material.PreshaderBuffer[50].x,Material.PreshaderBuffer[49].w,Local49.g);
	MaterialFloat Local281 = lerp(Material.PreshaderBuffer[50].z,Material.PreshaderBuffer[50].y,Local280);
	MaterialFloat Local282 = lerp(Local280,Local281,Local22);
	MaterialFloat Local283 = lerp(Material.PreshaderBuffer[51].x,Material.PreshaderBuffer[50].w,Local280);
	MaterialFloat Local284 = lerp(Local282,Local283,Local29);
	MaterialFloat Local285 = lerp(Material.PreshaderBuffer[51].z,Material.PreshaderBuffer[51].y,Local280);
	MaterialFloat Local286 = lerp(Local284,Local285,Local36);
	MaterialFloat Local287 = lerp(Material.PreshaderBuffer[52].x,Material.PreshaderBuffer[51].w,Local56.g);
	MaterialFloat Local288 = lerp(Local286,Local287,Local83.r);
	MaterialFloat Local289 = lerp(Material.PreshaderBuffer[52].z,Material.PreshaderBuffer[52].y,Local99.g);
	MaterialFloat Local290 = lerp(Local288,Local289,Local123.r);
	MaterialFloat Local291 = lerp(Material.PreshaderBuffer[53].x,Material.PreshaderBuffer[52].w,Local139.g);
	MaterialFloat Local292 = lerp(Local290,Local291,Local163.r);
	MaterialFloat Local293 = lerp(Material.PreshaderBuffer[53].z,Material.PreshaderBuffer[53].y,Local292);
	MaterialFloat Local294 = lerp(Local292,Local293,Local178);
	MaterialFloat Local429 = PositiveClampedPow(Local49.r,Material.PreshaderBuffer[53].w);
	MaterialFloat Local430 = (Local429 + Material.PreshaderBuffer[54].x);
	MaterialFloat Local431 = PositiveClampedPow(Local56.r,Material.PreshaderBuffer[54].y);
	MaterialFloat Local432 = (Local431 + Material.PreshaderBuffer[54].z);
	MaterialFloat Local433 = lerp(Local430,Local432,Local83.r);
	MaterialFloat Local434 = PositiveClampedPow(Local99.r,Material.PreshaderBuffer[54].w);
	MaterialFloat Local435 = (Local434 + Material.PreshaderBuffer[55].x);
	MaterialFloat Local436 = lerp(Local433,Local435,Local123.r);
	MaterialFloat Local437 = PositiveClampedPow(Local139.r,Material.PreshaderBuffer[55].y);
	MaterialFloat Local438 = (Local437 + Material.PreshaderBuffer[55].z);
	MaterialFloat Local439 = lerp(Local436,Local438,Local163.r);

	PixelMaterialInputs.EmissiveColor = Local179;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local273;
	PixelMaterialInputs.Metallic = Local276;
	PixelMaterialInputs.Specular = Local279;
	PixelMaterialInputs.Roughness = Local294;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local164;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = Local439;
	PixelMaterialInputs.Refraction = 0;
	PixelMaterialInputs.PixelDepthOffset = 0.00000000;
	PixelMaterialInputs.ShadingModel = 1;
	PixelMaterialInputs.FrontMaterial = GetInitialisedSubstrateData();
	PixelMaterialInputs.SurfaceThickness = 0.01000000;
	PixelMaterialInputs.Displacement = 0.50000000;


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
	//if( PixelMaterialInputs.OpacityMask < 0.333 ) discard;
	//o.Alpha = PixelMaterialInputs.OpacityMask;

	o.Metallic = PixelMaterialInputs.Metallic;
	o.Smoothness = 1.0 - PixelMaterialInputs.Roughness;
	o.Normal = normalize( PixelMaterialInputs.Normal );
	o.Emission = PixelMaterialInputs.EmissiveColor.rgb;
	o.Occlusion = PixelMaterialInputs.AmbientOcclusion;
}