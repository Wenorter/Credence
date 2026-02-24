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
TEXTURE2D(       Material_Texture2D_22 );
SAMPLER(  samplerMaterial_Texture2D_22 );
float4 Material_Texture2D_22_TexelSize;
float4 Material_Texture2D_22_ST;
uniform float4 SelectionColor;
uniform float Rotation_custom_main_layer;
uniform float U_tile_main_layer;
uniform float V_tile_main_layer;
uniform float UV_Tiling_main_layer;
uniform float Base_UV___U_Tiling_main_layer;
uniform float Base_UV___V_Tiling_main_layer;
uniform float Normal_intensity_main_layer;
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
uniform float Shift_side_Y_TopTint_layer;
uniform float Tiling_coordinate_Y_TopTint_layer;
uniform float Tiling_TopTint_layer;
uniform float Shift_side_X_TopTint_layer;
uniform float Tiling_coordinate_X_TopTint_layer;
uniform float Seam_hardness_TopTint_layer;
uniform float Shift_side_Z_TopTint_layer;
uniform float Tiling_coordinate_Z_TopTint_layer;
uniform float Normal_intensity_TopTint_layer;
uniform float TopTint_layer_contrast;
uniform float Disp_Contrast_TopTint_layer;
uniform float Disp_Intensity_TopTint_layer;
uniform float TopTint_layer_blend;
uniform float TopTint_layer_extend;
uniform float BlendSharp_2;
uniform float BlendSharp_1;
uniform float Tiling_noise_AWP;
uniform float noise_power;
uniform float wave_speed;
uniform float wave_scale;
uniform float wave_Normal_int;
uniform float getting_wet_depth;
uniform float getting_wet_Height_setting;
uniform float layer_getting_wet_contrast;
uniform float layer_getting_wet_blend;
uniform float layer_getting_wet_extend;
uniform float color_tone_main_layer;
uniform float Brightness_main_layer;
uniform float contrast_main_layer;
uniform float4 Tint_base_color_main_layer;
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
uniform float color_tone_TopTint_layer;
uniform float Brightness_TopTint_layer;
uniform float contrast_TopTint_layer;
uniform float4 Tint_base_color_TopTint_layer;
uniform float4 getting_wet_color_depth;
uniform float wetness_limit;
uniform float wetness_limit_bloom;
uniform float4 getting_wet_color;
uniform float getting_wet_opacity;
uniform float getting_wet_contrast;
uniform float Metallic_main_layer;
uniform float Metallic_R_channel;
uniform float Metallic_G_channel;
uniform float Metallic_B_channel;
uniform float Metallic_TopTint_layer;
uniform float Specular_main_layer;
uniform float Specular_R_channel;
uniform float Specular_G_channel;
uniform float Specular_B_channel;
uniform float Specular_TopTint_layer;
uniform float Roughness_min_main_layer;
uniform float Roughness_max_main_layer;
uniform float Roughness_min_R_channel;
uniform float Roughness_max_R_channel;
uniform float Roughness_min_G_channel;
uniform float Roughness_max_G_channel;
uniform float Roughness_min_B_channel;
uniform float Roughness_max_B_channel;
uniform float Roughness_min_B_all_mask;
uniform float Roughness_max_B_all_mask;
uniform float Roughness_min_TopTint_layer;
uniform float Roughness_max_TopTint_layer;
uniform float getting_wet_Roughness_bloom;
uniform float getting_wet_Roughness;
uniform float AO_Contrast_main_layer;
uniform float AO_Intensity_main_layer;
uniform float AO_Contrast_R_channel;
uniform float AO_Intensity_R_channel;
uniform float AO_Contrast_G_channel;
uniform float AO_Intensity_G_channel;
uniform float AO_Contrast_B_channel;
uniform float AO_Intensity_B_channel;
uniform float AO_Contrast_TopTint_layer;
uniform float AO_Intensity_TopTint_layer;

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
	float4 PreshaderBuffer[55];
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
	Material.PreshaderBuffer[2] = float4(1.000000,1.000000,-0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[4] = float4(15.000000,15.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[5] = float4(0.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[6] = float4(1.000000,1.000000,0.300000,1.000000);//(Unknown)
	Material.PreshaderBuffer[7] = float4(150.000000,4.000000,2.000000,-1.000000);//(Unknown)
	Material.PreshaderBuffer[8] = float4(2.000000,-1.000000,1.000000,-0.000000);//(Unknown)
	Material.PreshaderBuffer[9] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[10] = float4(5.000000,5.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[11] = float4(0.000000,1.000000,1.000000,0.300000);//(Unknown)
	Material.PreshaderBuffer[12] = float4(1.000000,150.000000,4.000000,2.000000);//(Unknown)
	Material.PreshaderBuffer[13] = float4(-1.000000,2.000000,-1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[14] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[15] = float4(0.000000,0.000000,11.000000,11.000000);//(Unknown)
	Material.PreshaderBuffer[16] = float4(1.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[17] = float4(3.000000,1.685635,1.000000,150.000000);//(Unknown)
	Material.PreshaderBuffer[18] = float4(4.000000,2.000000,-1.000000,1.370000);//(Unknown)
	Material.PreshaderBuffer[19] = float4(-0.370000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[20] = float4(0.664000,0.336000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[21] = float4(2.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[22] = float4(2.000000,-1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[23] = float4(1.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[24] = float4(1.000000,0.300000,0.005000,1.000000);//(Unknown)
	Material.PreshaderBuffer[25] = float4(5.000000,3.221240,2.000000,-1.000000);//(Unknown)
	Material.PreshaderBuffer[26] = float4(0.001000,1.000000,0.100000,1.000000);//(Unknown)
	Material.PreshaderBuffer[27] = float4(1.000000,0.000000,4.000000,2.000000);//(Unknown)
	Material.PreshaderBuffer[28] = float4(-1.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[29] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[30] = float4(0.616000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[31] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[32] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[33] = float4(0.700000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[34] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[35] = float4(0.750000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[36] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[37] = float4(1.000000,0.264000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[38] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[39] = float4(0.500000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[40] = float4(0.611328,0.572602,0.527748,0.000000);//(Unknown)
	Material.PreshaderBuffer[41] = float4(1.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[42] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[43] = float4(0.000000,0.000000,0.000000,-0.800000);//(Unknown)
	Material.PreshaderBuffer[44] = float4(-0.950000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[45] = float4(0.009258,0.044271,0.040740,0.000000);//(Unknown)
	Material.PreshaderBuffer[46] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[47] = float4(0.150000,0.150000,0.150000,0.150000);//(Unknown)
	Material.PreshaderBuffer[48] = float4(0.150000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[49] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[50] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[51] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[52] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[53] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[54] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)

	Material.PreshaderBuffer[0].xy = Append(Cos( Rotation_custom_main_layer * 6.283 ), Sin( Rotation_custom_main_layer * 6.283 ) * -1);
	Material.PreshaderBuffer[0].zw = Append(Sin( Rotation_custom_main_layer * 6.283 ), Cos( Rotation_custom_main_layer * 6.283 ));
	Material.PreshaderBuffer[1].xy = Append(U_tile_main_layer, V_tile_main_layer);
	Material.PreshaderBuffer[1].zw = UV_Tiling_main_layer * Append(Base_UV___U_Tiling_main_layer, Base_UV___V_Tiling_main_layer);
	Material.PreshaderBuffer[2].x = Normal_intensity_main_layer;
	Material.PreshaderBuffer[2].yz = Append(Cos( Rotation_custom_R_channel * 6.283 ), Sin( Rotation_custom_R_channel * 6.283 ) * -1);
	Material.PreshaderBuffer[3].xy = Append(Sin( Rotation_custom_R_channel * 6.283 ), Cos( Rotation_custom_R_channel * 6.283 ));
	Material.PreshaderBuffer[3].zw = Append(U_tile_R_channel, V_tile_R_channel);
	Material.PreshaderBuffer[4].xy = UV_Tiling_R_channel * Append(Base_UV___U_Tiling_R_channel, Base_UV___V_Tiling_R_channel);
	Material.PreshaderBuffer[4].z = Normal_intensity_R_channel;
	Material.PreshaderBuffer[4].w = Disp_Contrast_main_layer + 1;
	Material.PreshaderBuffer[5].x = 0 - Disp_Contrast_main_layer;
	Material.PreshaderBuffer[5].y = Disp_Intensity_main_layer;
	Material.PreshaderBuffer[5].z = Disp_Contrast_R_channel + 1;
	Material.PreshaderBuffer[5].w = 0 - Disp_Contrast_R_channel;
	Material.PreshaderBuffer[6].x = Disp_Intensity_R_channel;
	Material.PreshaderBuffer[6].y = R_channel_blend;
	Material.PreshaderBuffer[6].z = R_channel_extend;
	Material.PreshaderBuffer[6].w = UVmask_R_opacity;
	Material.PreshaderBuffer[7].x = Grunge_tiling_R_channel;
	Material.PreshaderBuffer[7].y = Grunge_channel_R_power;
	Material.PreshaderBuffer[7].z = UVmask_R_opacity_bloor + 1;
	Material.PreshaderBuffer[7].w = 0 - UVmask_R_opacity_bloor;
	Material.PreshaderBuffer[8].x = R_channel_contrast + 1;
	Material.PreshaderBuffer[8].y = 0 - R_channel_contrast;
	Material.PreshaderBuffer[8].zw = Append(Cos( Rotation_custom_G_channel * 6.283 ), Sin( Rotation_custom_G_channel * 6.283 ) * -1);
	Material.PreshaderBuffer[9].xy = Append(Sin( Rotation_custom_G_channel * 6.283 ), Cos( Rotation_custom_G_channel * 6.283 ));
	Material.PreshaderBuffer[9].zw = Append(U_tile_G_channel, V_tile_G_channel);
	Material.PreshaderBuffer[10].xy = UV_Tiling_G_channel * Append(Base_UV___U_Tiling_G_channel, Base_UV___V_Tiling_G_channel);
	Material.PreshaderBuffer[10].z = Normal_intensity_G_channel;
	Material.PreshaderBuffer[10].w = Disp_Contrast_G_channel + 1;
	Material.PreshaderBuffer[11].x = 0 - Disp_Contrast_G_channel;
	Material.PreshaderBuffer[11].y = Disp_Intensity_G_channel;
	Material.PreshaderBuffer[11].z = G_channel_blend;
	Material.PreshaderBuffer[11].w = G_channel_extend;
	Material.PreshaderBuffer[12].x = UVmask_G_opacity;
	Material.PreshaderBuffer[12].y = Grunge_tiling_G_channel;
	Material.PreshaderBuffer[12].z = Grunge_channel_G_power;
	Material.PreshaderBuffer[12].w = UVmask_G_opacity_bloor + 1;
	Material.PreshaderBuffer[13].x = 0 - UVmask_G_opacity_bloor;
	Material.PreshaderBuffer[13].y = G_channel_contrast + 1;
	Material.PreshaderBuffer[13].z = 0 - G_channel_contrast;
	Material.PreshaderBuffer[14].xy = Append(Cos( Rotation_custom_B_channel * 6.283 ), Sin( Rotation_custom_B_channel * 6.283 ) * -1);
	Material.PreshaderBuffer[14].zw = Append(Sin( Rotation_custom_B_channel * 6.283 ), Cos( Rotation_custom_B_channel * 6.283 ));
	Material.PreshaderBuffer[15].xy = Append(U_tile_B_channel, V_tile_B_channel);
	Material.PreshaderBuffer[15].zw = UV_Tiling_B_channel * Append(Base_UV___U_Tiling_B_channel, Base_UV___V_Tiling_B_channel);
	Material.PreshaderBuffer[16].x = Normal_intensity_B_channel;
	Material.PreshaderBuffer[16].y = Disp_Contrast_B_channel + 1;
	Material.PreshaderBuffer[16].z = 0 - Disp_Contrast_B_channel;
	Material.PreshaderBuffer[16].w = Disp_Intensity_B_channel;
	Material.PreshaderBuffer[17].x = B_channel_blend;
	Material.PreshaderBuffer[17].y = B_channel_extend;
	Material.PreshaderBuffer[17].z = UVmask_B_opacity;
	Material.PreshaderBuffer[17].w = Grunge_tiling_B_channel;
	Material.PreshaderBuffer[18].x = Grunge_channel_B_power;
	Material.PreshaderBuffer[18].y = UVmask_B_opacity_bloor + 1;
	Material.PreshaderBuffer[18].z = 0 - UVmask_B_opacity_bloor;
	Material.PreshaderBuffer[18].w = B_channel_contrast + 1;
	Material.PreshaderBuffer[19].x = 0 - B_channel_contrast;
	Material.PreshaderBuffer[19].y = UVmask_Ab_opacity;
	Material.PreshaderBuffer[19].z = Grunge_tiling_Ab_channel;
	Material.PreshaderBuffer[19].w = Grunge_channel_Ab_power;
	Material.PreshaderBuffer[20].x = UVmask_Ab_opacity_bloor + 1;
	Material.PreshaderBuffer[20].y = 0 - UVmask_Ab_opacity_bloor;
	Material.PreshaderBuffer[20].z = Shift_side_Y_TopTint_layer;
	Material.PreshaderBuffer[20].w = rcp( Abs( Tiling_coordinate_Y_TopTint_layer ) );
	Material.PreshaderBuffer[21].x = rcp( Tiling_TopTint_layer );
	Material.PreshaderBuffer[21].y = 0.5 / Tiling_TopTint_layer;
	Material.PreshaderBuffer[21].z = Shift_side_X_TopTint_layer;
	Material.PreshaderBuffer[21].w = rcp( Abs( Tiling_coordinate_X_TopTint_layer ) );
	Material.PreshaderBuffer[22].x = Seam_hardness_TopTint_layer + 1;
	Material.PreshaderBuffer[22].y = 0 - Seam_hardness_TopTint_layer;
	Material.PreshaderBuffer[22].z = Shift_side_Z_TopTint_layer;
	Material.PreshaderBuffer[22].w = rcp( Abs( Tiling_coordinate_Z_TopTint_layer ) );
	Material.PreshaderBuffer[23].x = Normal_intensity_TopTint_layer;
	Material.PreshaderBuffer[23].y = Disp_Contrast_TopTint_layer + 1;
	Material.PreshaderBuffer[23].z = 0 - Disp_Contrast_TopTint_layer;
	Material.PreshaderBuffer[23].w = Disp_Intensity_TopTint_layer;
	Material.PreshaderBuffer[24].x = TopTint_layer_blend;
	Material.PreshaderBuffer[24].y = TopTint_layer_extend;
	Material.PreshaderBuffer[24].z = Tiling_noise_AWP / 1000;
	Material.PreshaderBuffer[24].w = noise_power;
	Material.PreshaderBuffer[25].x = BlendSharp_1;
	Material.PreshaderBuffer[25].y = BlendSharp_2;
	Material.PreshaderBuffer[25].z = TopTint_layer_contrast + 1;
	Material.PreshaderBuffer[25].w = 0 - TopTint_layer_contrast;
	Material.PreshaderBuffer[26].x = wave_speed;
	Material.PreshaderBuffer[26].y = wave_scale;
	Material.PreshaderBuffer[26].z = wave_Normal_int;
	Material.PreshaderBuffer[26].w = getting_wet_Height_setting;
	Material.PreshaderBuffer[27].x = getting_wet_depth;
	Material.PreshaderBuffer[27].y = layer_getting_wet_blend;
	Material.PreshaderBuffer[27].z = layer_getting_wet_extend;
	Material.PreshaderBuffer[27].w = layer_getting_wet_contrast + 1;
	Material.PreshaderBuffer[28].x = 0 - layer_getting_wet_contrast;
	Material.PreshaderBuffer[28].y = SelectionColor.w;
	Material.PreshaderBuffer[29].xyz = SelectionColor.xyz;
	Material.PreshaderBuffer[29].w = color_tone_main_layer * 6.283;
	Material.PreshaderBuffer[30].x = Brightness_main_layer;
	Material.PreshaderBuffer[30].y = contrast_main_layer;
	Material.PreshaderBuffer[32].xyz = Tint_base_color_main_layer.xyz;
	Material.PreshaderBuffer[32].w = color_tone_R_channel * 6.283;
	Material.PreshaderBuffer[33].x = Brightness_R_channel;
	Material.PreshaderBuffer[33].y = contrast_R_channel;
	Material.PreshaderBuffer[34].xyz = Tint_base_color_R_channel.xyz;
	Material.PreshaderBuffer[34].w = color_tone_G_channel * 6.283;
	Material.PreshaderBuffer[35].x = Brightness_G_channel;
	Material.PreshaderBuffer[35].y = contrast_G_channel;
	Material.PreshaderBuffer[36].xyz = Tint_base_color_G_channel.xyz;
	Material.PreshaderBuffer[36].w = color_tone_B_channel * 6.283;
	Material.PreshaderBuffer[37].x = Brightness_B_channel;
	Material.PreshaderBuffer[37].y = contrast_B_channel;
	Material.PreshaderBuffer[38].xyz = Tint_base_color_B_channel.xyz;
	Material.PreshaderBuffer[38].w = color_tone_B_all_mask * 6.283;
	Material.PreshaderBuffer[39].x = Brightness_B_all_mask;
	Material.PreshaderBuffer[39].y = contrast_B_all_mask;
	Material.PreshaderBuffer[40].xyz = Tint_base_color_B_all_mask.xyz;
	Material.PreshaderBuffer[40].w = color_tone_TopTint_layer * 6.283;
	Material.PreshaderBuffer[41].x = Brightness_TopTint_layer;
	Material.PreshaderBuffer[41].y = contrast_TopTint_layer;
	Material.PreshaderBuffer[42].xyz = Tint_base_color_TopTint_layer.xyz;
	Material.PreshaderBuffer[43].xyz = getting_wet_color_depth.xyz;
	Material.PreshaderBuffer[43].w = wetness_limit;
	Material.PreshaderBuffer[44].x = wetness_limit_bloom;
	Material.PreshaderBuffer[44].y = getting_wet_opacity;
	Material.PreshaderBuffer[44].z = getting_wet_contrast;
	Material.PreshaderBuffer[45].xyz = getting_wet_color.xyz;
	Material.PreshaderBuffer[45].w = Metallic_main_layer;
	Material.PreshaderBuffer[46].x = Metallic_R_channel;
	Material.PreshaderBuffer[46].y = Metallic_G_channel;
	Material.PreshaderBuffer[46].z = Metallic_B_channel;
	Material.PreshaderBuffer[46].w = Metallic_TopTint_layer;
	Material.PreshaderBuffer[47].x = Specular_main_layer;
	Material.PreshaderBuffer[47].y = Specular_R_channel;
	Material.PreshaderBuffer[47].z = Specular_G_channel;
	Material.PreshaderBuffer[47].w = Specular_B_channel;
	Material.PreshaderBuffer[48].x = Specular_TopTint_layer;
	Material.PreshaderBuffer[48].y = Roughness_max_main_layer;
	Material.PreshaderBuffer[48].z = Roughness_min_main_layer;
	Material.PreshaderBuffer[48].w = Roughness_max_R_channel;
	Material.PreshaderBuffer[49].x = Roughness_min_R_channel;
	Material.PreshaderBuffer[49].y = Roughness_max_G_channel;
	Material.PreshaderBuffer[49].z = Roughness_min_G_channel;
	Material.PreshaderBuffer[49].w = Roughness_max_B_channel;
	Material.PreshaderBuffer[50].x = Roughness_min_B_channel;
	Material.PreshaderBuffer[50].y = Roughness_max_B_all_mask;
	Material.PreshaderBuffer[50].z = Roughness_min_B_all_mask;
	Material.PreshaderBuffer[50].w = Roughness_max_TopTint_layer;
	Material.PreshaderBuffer[51].x = Roughness_min_TopTint_layer;
	Material.PreshaderBuffer[51].y = getting_wet_Roughness_bloom;
	Material.PreshaderBuffer[51].z = getting_wet_Roughness;
	Material.PreshaderBuffer[51].w = AO_Contrast_main_layer;
	Material.PreshaderBuffer[52].x = 1 - AO_Intensity_main_layer;
	Material.PreshaderBuffer[52].y = AO_Contrast_R_channel;
	Material.PreshaderBuffer[52].z = 1 - AO_Intensity_R_channel;
	Material.PreshaderBuffer[52].w = AO_Contrast_G_channel;
	Material.PreshaderBuffer[53].x = 1 - AO_Intensity_G_channel;
	Material.PreshaderBuffer[53].y = AO_Contrast_B_channel;
	Material.PreshaderBuffer[53].z = 1 - AO_Intensity_B_channel;
	Material.PreshaderBuffer[53].w = AO_Contrast_TopTint_layer;
	Material.PreshaderBuffer[54].x = 1 - AO_Intensity_TopTint_layer;
}
float3 GetMaterialWorldPositionOffset(FMaterialVertexParameters Parameters)
{
SHADER_PUSH_WARNINGS_STATE
SHADER_DISABLE_WARNINGS
	return MaterialFloat3(0.00000000,0.00000000,0.00000000);;
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
	MaterialFloat Local8 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 5);
	MaterialFloat4 Local9 = UnpackNormalMap(Texture2DSample(Material_Texture2D_0,GetMaterialSharedSampler(samplerMaterial_Texture2D_0,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local10 = MaterialStoreTexSample(Parameters, Local9, 5);
	MaterialFloat Local11 = (Local9.rgb.r * Material.PreshaderBuffer[2].x);
	MaterialFloat Local12 = (Local9.rgb.g * Material.PreshaderBuffer[2].x);
	MaterialFloat Local13 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[2].yz);
	MaterialFloat Local14 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[3].xy);
	MaterialFloat2 Local15 = MaterialFloat2(DERIV_BASE_VALUE(Local13),DERIV_BASE_VALUE(Local14));
	MaterialFloat2 Local16 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local15));
	MaterialFloat2 Local17 = (DERIV_BASE_VALUE(Local16) + Material.PreshaderBuffer[3].zw);
	MaterialFloat2 Local18 = (DERIV_BASE_VALUE(Local17) * Material.PreshaderBuffer[4].xy);
	MaterialFloat Local19 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local18), 5);
	MaterialFloat4 Local20 = UnpackNormalMap(Texture2DSample(Material_Texture2D_1,GetMaterialSharedSampler(samplerMaterial_Texture2D_1,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local18)));
	MaterialFloat Local21 = MaterialStoreTexSample(Parameters, Local20, 5);
	MaterialFloat Local22 = (Local20.rgb.r * Material.PreshaderBuffer[4].z);
	MaterialFloat Local23 = (Local20.rgb.g * Material.PreshaderBuffer[4].z);
	MaterialFloat Local24 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 7);
	MaterialFloat4 Local25 = Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7));
	MaterialFloat Local26 = MaterialStoreTexSample(Parameters, Local25, 7);
	MaterialFloat Local27 = lerp(Material.PreshaderBuffer[5].x,Material.PreshaderBuffer[4].w,Local25.b);
	MaterialFloat Local28 = saturate(Local27);
	MaterialFloat Local29 = (Local28.r * Material.PreshaderBuffer[5].y);
	MaterialFloat Local30 = saturate(Local29);
	MaterialFloat Local31 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local18), 7);
	MaterialFloat4 Local32 = Texture2DSample(Material_Texture2D_3,GetMaterialSharedSampler(samplerMaterial_Texture2D_3,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local18));
	MaterialFloat Local33 = MaterialStoreTexSample(Parameters, Local32, 7);
	MaterialFloat Local34 = lerp(Material.PreshaderBuffer[5].w,Material.PreshaderBuffer[5].z,Local32.b);
	MaterialFloat Local35 = saturate(Local34);
	MaterialFloat Local36 = (Local35.r * Material.PreshaderBuffer[6].x);
	MaterialFloat Local37 = lerp(Local30,Local36,Material.PreshaderBuffer[6].y);
	MaterialFloat Local38 = (1.00000000 - Local37);
	MaterialFloat Local39 = (Local38 * Material.PreshaderBuffer[6].z);
	MaterialFloat Local40 = (Local39 - 1.00000000);
	MaterialFloat Local41 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local0), 8);
	MaterialFloat4 Local42 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(samplerMaterial_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local0)));
	MaterialFloat Local43 = MaterialStoreTexSample(Parameters, Local42, 8);
	MaterialFloat Local44 = (Local42.r * Material.PreshaderBuffer[6].w);
	MaterialFloat2 Local45 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[7].x));
	MaterialFloat Local46 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local45), 9);
	MaterialFloat4 Local47 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local45)));
	MaterialFloat Local48 = MaterialStoreTexSample(Parameters, Local47, 9);
	MaterialFloat3 Local49 = (Local47.rgb * ((MaterialFloat3)Material.PreshaderBuffer[7].y));
	MaterialFloat3 Local50 = (((MaterialFloat3)Local42.r) + Local49);
	MaterialFloat3 Local51 = (((MaterialFloat3)Local44) * Local50);
	MaterialFloat Local52 = lerp(Material.PreshaderBuffer[7].w,Material.PreshaderBuffer[7].z,Local51.x);
	MaterialFloat Local53 = saturate(Local52);
	MaterialFloat Local54 = saturate(Local53.r);
	MaterialFloat Local55 = (Local54 * 2.00000000);
	MaterialFloat Local56 = (Local40 + Local55);
	MaterialFloat Local57 = saturate(Local56);
	MaterialFloat Local58 = lerp(Material.PreshaderBuffer[8].y,Material.PreshaderBuffer[8].x,Local57);
	MaterialFloat Local59 = saturate(Local58);
	MaterialFloat3 Local60 = lerp(MaterialFloat3(MaterialFloat2(Local11,Local12),Local9.rgb.b),MaterialFloat3(MaterialFloat2(Local22,Local23),Local20.rgb.b),Local59.r);
	MaterialFloat Local61 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[8].zw);
	MaterialFloat Local62 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[9].xy);
	MaterialFloat2 Local63 = MaterialFloat2(DERIV_BASE_VALUE(Local61),DERIV_BASE_VALUE(Local62));
	MaterialFloat2 Local64 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local63));
	MaterialFloat2 Local65 = (DERIV_BASE_VALUE(Local64) + Material.PreshaderBuffer[9].zw);
	MaterialFloat2 Local66 = (DERIV_BASE_VALUE(Local65) * Material.PreshaderBuffer[10].xy);
	MaterialFloat Local67 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local66), 5);
	MaterialFloat4 Local68 = UnpackNormalMap(Texture2DSample(Material_Texture2D_6,GetMaterialSharedSampler(samplerMaterial_Texture2D_6,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local66)));
	MaterialFloat Local69 = MaterialStoreTexSample(Parameters, Local68, 5);
	MaterialFloat Local70 = (Local68.rgb.r * Material.PreshaderBuffer[10].z);
	MaterialFloat Local71 = (Local68.rgb.g * Material.PreshaderBuffer[10].z);
	MaterialFloat Local72 = lerp(Local29,Local36,Local59.r);
	MaterialFloat Local73 = saturate(Local72);
	MaterialFloat Local74 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local66), 7);
	MaterialFloat4 Local75 = Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local66));
	MaterialFloat Local76 = MaterialStoreTexSample(Parameters, Local75, 7);
	MaterialFloat Local77 = lerp(Material.PreshaderBuffer[11].x,Material.PreshaderBuffer[10].w,Local75.b);
	MaterialFloat Local78 = saturate(Local77);
	MaterialFloat Local79 = (Local78.r * Material.PreshaderBuffer[11].y);
	MaterialFloat Local80 = lerp(Local73,Local79,Material.PreshaderBuffer[11].z);
	MaterialFloat Local81 = (1.00000000 - Local80);
	MaterialFloat Local82 = (Local81 * Material.PreshaderBuffer[11].w);
	MaterialFloat Local83 = (Local82 - 1.00000000);
	MaterialFloat Local84 = (Local42.g * Material.PreshaderBuffer[12].x);
	MaterialFloat2 Local85 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[12].y));
	MaterialFloat Local86 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local85), 9);
	MaterialFloat4 Local87 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_8,GetMaterialSharedSampler(samplerMaterial_Texture2D_8,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local85)));
	MaterialFloat Local88 = MaterialStoreTexSample(Parameters, Local87, 9);
	MaterialFloat3 Local89 = (Local87.rgb * ((MaterialFloat3)Material.PreshaderBuffer[12].z));
	MaterialFloat3 Local90 = (((MaterialFloat3)Local42.g) + Local89);
	MaterialFloat3 Local91 = (((MaterialFloat3)Local84) * Local90);
	MaterialFloat Local92 = lerp(Material.PreshaderBuffer[13].x,Material.PreshaderBuffer[12].w,Local91.x);
	MaterialFloat Local93 = saturate(Local92);
	MaterialFloat Local94 = saturate(Local93.r);
	MaterialFloat Local95 = (Local94 * 2.00000000);
	MaterialFloat Local96 = (Local83 + Local95);
	MaterialFloat Local97 = saturate(Local96);
	MaterialFloat Local98 = lerp(Material.PreshaderBuffer[13].z,Material.PreshaderBuffer[13].y,Local97);
	MaterialFloat Local99 = saturate(Local98);
	MaterialFloat3 Local100 = lerp(Local60,MaterialFloat3(MaterialFloat2(Local70,Local71),Local68.rgb.b),Local99.r);
	MaterialFloat Local101 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[14].xy);
	MaterialFloat Local102 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[14].zw);
	MaterialFloat2 Local103 = MaterialFloat2(DERIV_BASE_VALUE(Local101),DERIV_BASE_VALUE(Local102));
	MaterialFloat2 Local104 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local103));
	MaterialFloat2 Local105 = (DERIV_BASE_VALUE(Local104) + Material.PreshaderBuffer[15].xy);
	MaterialFloat2 Local106 = (DERIV_BASE_VALUE(Local105) * Material.PreshaderBuffer[15].zw);
	MaterialFloat Local107 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local106), 5);
	MaterialFloat4 Local108 = UnpackNormalMap(Texture2DSample(Material_Texture2D_9,GetMaterialSharedSampler(samplerMaterial_Texture2D_9,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local106)));
	MaterialFloat Local109 = MaterialStoreTexSample(Parameters, Local108, 5);
	MaterialFloat Local110 = (Local108.rgb.r * Material.PreshaderBuffer[16].x);
	MaterialFloat Local111 = (Local108.rgb.g * Material.PreshaderBuffer[16].x);
	MaterialFloat Local112 = lerp(Local72,Local79,Local99.r);
	MaterialFloat Local113 = saturate(Local112);
	MaterialFloat Local114 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local106), 7);
	MaterialFloat4 Local115 = Texture2DSample(Material_Texture2D_10,GetMaterialSharedSampler(samplerMaterial_Texture2D_10,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local106));
	MaterialFloat Local116 = MaterialStoreTexSample(Parameters, Local115, 7);
	MaterialFloat Local117 = lerp(Material.PreshaderBuffer[16].z,Material.PreshaderBuffer[16].y,Local115.b);
	MaterialFloat Local118 = saturate(Local117);
	MaterialFloat Local119 = (Local118.r * Material.PreshaderBuffer[16].w);
	MaterialFloat Local120 = lerp(Local113,Local119,Material.PreshaderBuffer[17].x);
	MaterialFloat Local121 = (1.00000000 - Local120);
	MaterialFloat Local122 = (Local121 * Material.PreshaderBuffer[17].y);
	MaterialFloat Local123 = (Local122 - 1.00000000);
	MaterialFloat Local124 = (Local42.b * Material.PreshaderBuffer[17].z);
	MaterialFloat2 Local125 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[17].w));
	MaterialFloat Local126 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local125), 9);
	MaterialFloat4 Local127 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_11,GetMaterialSharedSampler(samplerMaterial_Texture2D_11,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local125)));
	MaterialFloat Local128 = MaterialStoreTexSample(Parameters, Local127, 9);
	MaterialFloat3 Local129 = (Local127.rgb * ((MaterialFloat3)Material.PreshaderBuffer[18].x));
	MaterialFloat3 Local130 = (((MaterialFloat3)Local42.b) + Local129);
	MaterialFloat3 Local131 = (((MaterialFloat3)Local124) * Local130);
	MaterialFloat Local132 = lerp(Material.PreshaderBuffer[18].z,Material.PreshaderBuffer[18].y,Local131.x);
	MaterialFloat Local133 = saturate(Local132);
	MaterialFloat Local134 = saturate(Local133.r);
	MaterialFloat Local135 = (Local134 * 2.00000000);
	MaterialFloat Local136 = (Local123 + Local135);
	MaterialFloat Local137 = saturate(Local136);
	MaterialFloat Local138 = lerp(Material.PreshaderBuffer[19].x,Material.PreshaderBuffer[18].w,Local137);
	MaterialFloat Local139 = saturate(Local138);
	MaterialFloat3 Local140 = lerp(Local100,MaterialFloat3(MaterialFloat2(Local110,Local111),Local108.rgb.b),Local139.r);
	MaterialFloat Local141 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local0), 11);
	MaterialFloat4 Local142 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_12,GetMaterialSharedSampler(samplerMaterial_Texture2D_12,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local0)));
	MaterialFloat Local143 = MaterialStoreTexSample(Parameters, Local142, 11);
	MaterialFloat Local144 = (Local142.a * Material.PreshaderBuffer[19].y);
	MaterialFloat2 Local145 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[19].z));
	MaterialFloat Local146 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local145), 9);
	MaterialFloat4 Local147 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_13,GetMaterialSharedSampler(samplerMaterial_Texture2D_13,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local145)));
	MaterialFloat Local148 = MaterialStoreTexSample(Parameters, Local147, 9);
	MaterialFloat3 Local149 = (Local147.rgb * ((MaterialFloat3)Material.PreshaderBuffer[19].w));
	MaterialFloat3 Local150 = (((MaterialFloat3)Local142.a) + Local149);
	MaterialFloat3 Local151 = (((MaterialFloat3)Local144) * Local150);
	MaterialFloat Local152 = lerp(Material.PreshaderBuffer[20].y,Material.PreshaderBuffer[20].x,Local151.x);
	MaterialFloat Local153 = saturate(Local152);
	MaterialFloat Local154 = saturate(Local153.r);
	MaterialFloat3 Local155 = Parameters.TangentToWorld[2];
	MaterialFloat3 Local156 = normalize(DERIV_BASE_VALUE(Local155));
	MaterialFloat3 Local157 = cross(DERIV_BASE_VALUE(Local156),normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb));
	MaterialFloat Local158 = dot(DERIV_BASE_VALUE(Local157),DERIV_BASE_VALUE(Local157));
	MaterialFloat3 Local159 = normalize(DERIV_BASE_VALUE(Local157));
	MaterialFloat4 Local160 = MaterialFloat4(DERIV_BASE_VALUE(Local159),0.00000000);
	MaterialFloat4 Local161 = select((abs(DERIV_BASE_VALUE(Local158) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local158) >= 0.00000100), DERIV_BASE_VALUE(Local160), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local162 = DERIV_BASE_VALUE(Local161).rgb;
	MaterialFloat Local163 = dot(DERIV_BASE_VALUE(Local155),MaterialFloat3(0.00000000,1.00000000,0.00000000).rgb);
	MaterialFloat Local164 = select((DERIV_BASE_VALUE(Local163) >= 0.00000000), 1.00000000, -1.00000000);
	FWSVector3 Local165 = GetWorldPosition_NoMaterialOffsets(Parameters);
	FWSVector3 Local166 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local165)), WSGetY(DERIV_BASE_VALUE(Local165)), WSGetZ(DERIV_BASE_VALUE(Local165)));
	FWSVector3 Local167 = WSDivide(DERIV_BASE_VALUE(Local166), ((MaterialFloat3)-255.00000000));
	FWSVector2 Local168 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local167)), WSGetZ(DERIV_BASE_VALUE(Local167)));
	FWSVector2 Local169 = WSAdd(DERIV_BASE_VALUE(Local168), ((MaterialFloat2)Material.PreshaderBuffer[20].z));
	FWSVector2 Local170 = WSMultiply(DERIV_BASE_VALUE(Local169), ((MaterialFloat2)Material.PreshaderBuffer[20].w));
	FWSVector2 Local171 = WSMultiply(DERIV_BASE_VALUE(Local170), ((MaterialFloat2)Material.PreshaderBuffer[21].x));
	FWSVector2 Local172 = WSAdd(DERIV_BASE_VALUE(Local171), ((MaterialFloat2)0.50000000));
	FWSVector2 Local173 = WSSubtract(DERIV_BASE_VALUE(Local172), ((MaterialFloat2)Material.PreshaderBuffer[21].y));
	MaterialFloat2 Local174 = WSApplyAddressMode(DERIV_BASE_VALUE(Local173), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local175 = MaterialStoreTexCoordScale(Parameters, Local174, 5);
	MaterialFloat4 Local176 = UnpackNormalMap(Texture2DSample(Material_Texture2D_14,GetMaterialSharedSampler(samplerMaterial_Texture2D_14,View_MaterialTextureBilinearWrapedSampler),Local174));
	MaterialFloat Local177 = MaterialStoreTexSample(Parameters, Local176, 5);
	MaterialFloat3 Local178 = (MaterialFloat3(MaterialFloat2(Local164,-1.00000000),1.00000000) * Local176.rgb);
	MaterialFloat Local179 = dot(DERIV_BASE_VALUE(Local155),MaterialFloat3(1.00000000,0.00000000,0.00000000).rgb);
	MaterialFloat Local180 = select((DERIV_BASE_VALUE(Local179) >= 0.00000000), 1.00000000, -1.00000000);
	FWSVector2 Local181 = MakeWSVector(WSGetY(DERIV_BASE_VALUE(Local167)), WSGetZ(DERIV_BASE_VALUE(Local167)));
	FWSVector2 Local182 = WSAdd(DERIV_BASE_VALUE(Local181), ((MaterialFloat2)Material.PreshaderBuffer[21].z));
	FWSVector2 Local183 = WSMultiply(DERIV_BASE_VALUE(Local182), ((MaterialFloat2)Material.PreshaderBuffer[21].w));
	FWSVector2 Local184 = WSMultiply(DERIV_BASE_VALUE(Local183), ((MaterialFloat2)Material.PreshaderBuffer[21].x));
	FWSVector2 Local185 = WSAdd(DERIV_BASE_VALUE(Local184), ((MaterialFloat2)0.50000000));
	FWSVector2 Local186 = WSSubtract(DERIV_BASE_VALUE(Local185), ((MaterialFloat2)Material.PreshaderBuffer[21].y));
	MaterialFloat2 Local187 = WSApplyAddressMode(DERIV_BASE_VALUE(Local186), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local188 = MaterialStoreTexCoordScale(Parameters, Local187, 5);
	MaterialFloat4 Local189 = UnpackNormalMap(Texture2DSample(Material_Texture2D_14,GetMaterialSharedSampler(samplerMaterial_Texture2D_14,View_MaterialTextureBilinearWrapedSampler),Local187));
	MaterialFloat Local190 = MaterialStoreTexSample(Parameters, Local189, 5);
	MaterialFloat3 Local191 = (MaterialFloat3(MaterialFloat2(Local180,-1.00000000),1.00000000) * Local189.rgb);
	MaterialFloat3 Local192 = mul(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb, Parameters.TangentToWorld);
	MaterialFloat3 Local193 = abs(Local192);
	MaterialFloat Local194 = DERIV_BASE_VALUE(Local193).x;
	MaterialFloat Local195 = lerp(Material.PreshaderBuffer[22].y,Material.PreshaderBuffer[22].x,DERIV_BASE_VALUE(Local194));
	MaterialFloat Local196 = saturate(DERIV_BASE_VALUE(Local195));
	MaterialFloat Local197 = DERIV_BASE_VALUE(Local196).r;
	MaterialFloat3 Local198 = lerp(Local178,Local191,DERIV_BASE_VALUE(Local197));
	MaterialFloat3 Local199 = (DERIV_BASE_VALUE(Local162) * ((MaterialFloat3)Local198.r));
	MaterialFloat3 Local200 = cross(DERIV_BASE_VALUE(Local157),DERIV_BASE_VALUE(Local156));
	MaterialFloat Local201 = dot(DERIV_BASE_VALUE(Local200),DERIV_BASE_VALUE(Local200));
	MaterialFloat3 Local202 = normalize(DERIV_BASE_VALUE(Local200));
	MaterialFloat4 Local203 = MaterialFloat4(DERIV_BASE_VALUE(Local202),0.00000000);
	MaterialFloat4 Local204 = select((abs(DERIV_BASE_VALUE(Local201) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local201) >= 0.00000100), DERIV_BASE_VALUE(Local203), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local205 = DERIV_BASE_VALUE(Local204).rgb;
	MaterialFloat3 Local206 = (DERIV_BASE_VALUE(Local205) * ((MaterialFloat3)Local198.g));
	MaterialFloat3 Local207 = (Local199 + Local206);
	MaterialFloat3 Local208 = (DERIV_BASE_VALUE(Local156) * ((MaterialFloat3)Local198.b));
	MaterialFloat3 Local209 = (Local208 + MaterialFloat3(0.00000000,0.00000000,0.00000000));
	MaterialFloat3 Local210 = (Local207 + Local209);
	MaterialFloat3 Local211 = cross(DERIV_BASE_VALUE(Local156),normalize(MaterialFloat3(0.00000000,1.00000000,0.00000000).rgb));
	MaterialFloat Local212 = dot(DERIV_BASE_VALUE(Local211),DERIV_BASE_VALUE(Local211));
	MaterialFloat3 Local213 = normalize(DERIV_BASE_VALUE(Local211));
	MaterialFloat4 Local214 = MaterialFloat4(DERIV_BASE_VALUE(Local213),0.00000000);
	MaterialFloat4 Local215 = select((abs(DERIV_BASE_VALUE(Local212) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local212) >= 0.00000100), DERIV_BASE_VALUE(Local214), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local216 = DERIV_BASE_VALUE(Local215).rgb;
	MaterialFloat Local217 = dot(DERIV_BASE_VALUE(Local155),MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb);
	MaterialFloat Local218 = select((DERIV_BASE_VALUE(Local217) >= 0.00000000), 1.00000000, -1.00000000);
	FWSVector2 Local219 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local167)), WSGetY(DERIV_BASE_VALUE(Local167)));
	FWSVector2 Local220 = WSAdd(DERIV_BASE_VALUE(Local219), ((MaterialFloat2)Material.PreshaderBuffer[22].z));
	FWSVector2 Local221 = WSMultiply(DERIV_BASE_VALUE(Local220), ((MaterialFloat2)Material.PreshaderBuffer[22].w));
	FWSVector2 Local222 = WSMultiply(DERIV_BASE_VALUE(Local221), ((MaterialFloat2)Material.PreshaderBuffer[21].x));
	FWSVector2 Local223 = WSAdd(DERIV_BASE_VALUE(Local222), ((MaterialFloat2)0.50000000));
	FWSVector2 Local224 = WSSubtract(DERIV_BASE_VALUE(Local223), ((MaterialFloat2)Material.PreshaderBuffer[21].y));
	MaterialFloat2 Local225 = WSApplyAddressMode(DERIV_BASE_VALUE(Local224), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local226 = MaterialStoreTexCoordScale(Parameters, Local225, 5);
	MaterialFloat4 Local227 = UnpackNormalMap(Texture2DSample(Material_Texture2D_14,GetMaterialSharedSampler(samplerMaterial_Texture2D_14,View_MaterialTextureBilinearWrapedSampler),Local225));
	MaterialFloat Local228 = MaterialStoreTexSample(Parameters, Local227, 5);
	MaterialFloat3 Local229 = (MaterialFloat3(MaterialFloat2(Local218,-1.00000000),1.00000000) * Local227.rgb);
	MaterialFloat3 Local230 = (DERIV_BASE_VALUE(Local216) * ((MaterialFloat3)Local229.r));
	MaterialFloat3 Local231 = cross(DERIV_BASE_VALUE(Local211),DERIV_BASE_VALUE(Local156));
	MaterialFloat Local232 = dot(DERIV_BASE_VALUE(Local231),DERIV_BASE_VALUE(Local231));
	MaterialFloat3 Local233 = normalize(DERIV_BASE_VALUE(Local231));
	MaterialFloat4 Local234 = MaterialFloat4(DERIV_BASE_VALUE(Local233),0.00000000);
	MaterialFloat4 Local235 = select((abs(DERIV_BASE_VALUE(Local232) - 0.00000100) > 0.00001000), select((DERIV_BASE_VALUE(Local232) >= 0.00000100), DERIV_BASE_VALUE(Local234), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000)), MaterialFloat4(MaterialFloat3(0.00000000,0.00000000,0.00000000),1.00000000));
	MaterialFloat3 Local236 = DERIV_BASE_VALUE(Local235).rgb;
	MaterialFloat3 Local237 = (DERIV_BASE_VALUE(Local236) * ((MaterialFloat3)Local229.g));
	MaterialFloat3 Local238 = (Local230 + Local237);
	MaterialFloat3 Local239 = (DERIV_BASE_VALUE(Local156) * ((MaterialFloat3)Local229.b));
	MaterialFloat3 Local240 = (Local239 + MaterialFloat3(0.00000000,0.00000000,0.00000000));
	MaterialFloat3 Local241 = (Local238 + Local240);
	MaterialFloat Local242 = abs(Local192.b);
	MaterialFloat Local243 = lerp(Material.PreshaderBuffer[22].y,Material.PreshaderBuffer[22].x,DERIV_BASE_VALUE(Local242));
	MaterialFloat Local244 = saturate(DERIV_BASE_VALUE(Local243));
	MaterialFloat Local245 = DERIV_BASE_VALUE(Local244).r;
	MaterialFloat3 Local246 = lerp(Local210,Local241,DERIV_BASE_VALUE(Local245));
	MaterialFloat3 Local247 = mul((MaterialFloat3x3)(Parameters.TangentToWorld), Local246);
	MaterialFloat Local248 = (Local247.r * Material.PreshaderBuffer[23].x);
	MaterialFloat Local249 = (Local247.g * Material.PreshaderBuffer[23].x);
	MaterialFloat Local250 = lerp(Local112,Local119,Local139.r);
	MaterialFloat Local251 = saturate(Local250);
	MaterialFloat Local252 = MaterialStoreTexCoordScale(Parameters, Local174, 7);
	MaterialFloat4 Local253 = Texture2DSample(Material_Texture2D_15,GetMaterialSharedSampler(samplerMaterial_Texture2D_15,View_MaterialTextureBilinearWrapedSampler),Local174);
	MaterialFloat Local254 = MaterialStoreTexSample(Parameters, Local253, 7);
	MaterialFloat Local255 = MaterialStoreTexCoordScale(Parameters, Local187, 7);
	MaterialFloat4 Local256 = Texture2DSample(Material_Texture2D_15,GetMaterialSharedSampler(samplerMaterial_Texture2D_15,View_MaterialTextureBilinearWrapedSampler),Local187);
	MaterialFloat Local257 = MaterialStoreTexSample(Parameters, Local256, 7);
	MaterialFloat3 Local258 = abs(DERIV_BASE_VALUE(Local155));
	MaterialFloat Local259 = DERIV_BASE_VALUE(Local258).x;
	MaterialFloat Local260 = lerp(Material.PreshaderBuffer[22].y,Material.PreshaderBuffer[22].x,DERIV_BASE_VALUE(Local259));
	MaterialFloat Local261 = saturate(DERIV_BASE_VALUE(Local260));
	MaterialFloat Local262 = DERIV_BASE_VALUE(Local261).r;
	MaterialFloat3 Local263 = lerp(Local253.rgb,Local256.rgb,DERIV_BASE_VALUE(Local262));
	MaterialFloat Local264 = MaterialStoreTexCoordScale(Parameters, Local225, 7);
	MaterialFloat4 Local265 = Texture2DSample(Material_Texture2D_15,GetMaterialSharedSampler(samplerMaterial_Texture2D_15,View_MaterialTextureBilinearWrapedSampler),Local225);
	MaterialFloat Local266 = MaterialStoreTexSample(Parameters, Local265, 7);
	MaterialFloat Local267 = DERIV_BASE_VALUE(Local155).b;
	MaterialFloat Local268 = abs(DERIV_BASE_VALUE(Local267));
	MaterialFloat Local269 = lerp(Material.PreshaderBuffer[22].y,Material.PreshaderBuffer[22].x,DERIV_BASE_VALUE(Local268));
	MaterialFloat Local270 = saturate(DERIV_BASE_VALUE(Local269));
	MaterialFloat Local271 = DERIV_BASE_VALUE(Local270).r;
	MaterialFloat3 Local272 = lerp(Local263,Local265.rgb,DERIV_BASE_VALUE(Local271));
	MaterialFloat Local273 = lerp(Material.PreshaderBuffer[23].z,Material.PreshaderBuffer[23].y,Local272.b);
	MaterialFloat Local274 = saturate(Local273);
	MaterialFloat Local275 = (Local274.r * Material.PreshaderBuffer[23].w);
	MaterialFloat Local276 = lerp(Local251,Local275,Material.PreshaderBuffer[24].x);
	MaterialFloat Local277 = (1.00000000 - Local276);
	MaterialFloat Local278 = (Local277 * Material.PreshaderBuffer[24].y);
	MaterialFloat Local279 = (Local278 - 1.00000000);
	MaterialFloat Local280 = dot(DERIV_BASE_VALUE(Local155),normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb));
	MaterialFloat Local281 = (DERIV_BASE_VALUE(Local280) * 0.50000000);
	MaterialFloat Local282 = (DERIV_BASE_VALUE(Local281) + 0.50000000);
	FWSVector3 Local283 = GetWorldPosition(Parameters);
	FWSVector2 Local284 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local283)), WSGetY(DERIV_BASE_VALUE(Local283)));
	FWSVector2 Local285 = WSMultiply(DERIV_BASE_VALUE(Local284), ((MaterialFloat2)Material.PreshaderBuffer[24].z));
	MaterialFloat2 Local286 = WSApplyAddressMode(DERIV_BASE_VALUE(Local285), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local287 = MaterialStoreTexCoordScale(Parameters, Local286, 8);
	MaterialFloat4 Local288 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_16,GetMaterialSharedSampler(samplerMaterial_Texture2D_16,View_MaterialTextureBilinearWrapedSampler),Local286));
	MaterialFloat Local289 = MaterialStoreTexSample(Parameters, Local288, 8);
	MaterialFloat3 Local290 = (Local288.rgb * ((MaterialFloat3)Material.PreshaderBuffer[24].w));
	MaterialFloat Local291 = lerp(Material.PreshaderBuffer[25].y,Material.PreshaderBuffer[25].x,Local290.r);
	MaterialFloat Local292 = (DERIV_BASE_VALUE(Local282) * Local291);
	MaterialFloat Local293 = (Local291 * 0.50000000);
	MaterialFloat Local294 = (-1.00000000 - Local293);
	MaterialFloat Local295 = (Local292 + Local294);
	MaterialFloat Local296 = saturate(Local295);
	MaterialFloat Local297 = (Local296 * 2.00000000);
	MaterialFloat Local298 = (Local279 + Local297);
	MaterialFloat Local299 = saturate(Local298);
	MaterialFloat Local300 = lerp(Material.PreshaderBuffer[25].w,Material.PreshaderBuffer[25].z,Local299);
	MaterialFloat Local301 = saturate(Local300);
	MaterialFloat3 Local302 = lerp(Local140,MaterialFloat3(MaterialFloat2(Local248,Local249),Local247.b),Local301.r);
	MaterialFloat Local303 = (Material.PreshaderBuffer[26].x * View.GameTime);
	MaterialFloat Local304 = (Local303 * 0.10000000);
	FWSVector3 Local305 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local283)), WSGetY(DERIV_BASE_VALUE(Local283)), WSGetZ(DERIV_BASE_VALUE(Local283)));
	FWSVector3 Local306 = WSDivide(DERIV_BASE_VALUE(Local305), ((MaterialFloat3)1000.00000000));
	FWSVector3 Local307 = WSMultiply(DERIV_BASE_VALUE(Local306), ((MaterialFloat3)Material.PreshaderBuffer[26].y));
	FWSVector2 Local308 = MakeWSVector(WSGetComponent(DERIV_BASE_VALUE(Local307), 0),WSGetComponent(DERIV_BASE_VALUE(Local307), 1));
	FWSVector2 Local309 = WSAdd(MaterialFloat2(Local304,Local304), DERIV_BASE_VALUE(Local308));
	MaterialFloat2 Local310 = WSApplyAddressMode(DERIV_BASE_VALUE(Local309), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local311 = MaterialStoreTexCoordScale(Parameters, Local310, 1);
	MaterialFloat4 Local312 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local310,View.MaterialTextureMipBias));
	MaterialFloat Local313 = MaterialStoreTexSample(Parameters, Local312, 1);
	MaterialFloat Local314 = (Local303 * -0.10000000);
	FWSVector2 Local315 = WSAdd(DERIV_BASE_VALUE(Local308), MaterialFloat2(0.50000000,0.40000001).rg);
	FWSVector2 Local316 = WSAdd(MaterialFloat2(Local314,Local304), DERIV_BASE_VALUE(Local315));
	MaterialFloat2 Local317 = WSApplyAddressMode(DERIV_BASE_VALUE(Local316), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local318 = MaterialStoreTexCoordScale(Parameters, Local317, 1);
	MaterialFloat4 Local319 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local317,View.MaterialTextureMipBias));
	MaterialFloat Local320 = MaterialStoreTexSample(Parameters, Local319, 1);
	MaterialFloat3 Local321 = (Local312.rgb + Local319.rgb);
	FWSVector2 Local322 = WSAdd(DERIV_BASE_VALUE(Local308), MaterialFloat2(0.89999998,0.20000000).rg);
	FWSVector2 Local323 = WSAdd(MaterialFloat2(Local304,Local314), DERIV_BASE_VALUE(Local322));
	MaterialFloat2 Local324 = WSApplyAddressMode(DERIV_BASE_VALUE(Local323), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local325 = MaterialStoreTexCoordScale(Parameters, Local324, 1);
	MaterialFloat4 Local326 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local324,View.MaterialTextureMipBias));
	MaterialFloat Local327 = MaterialStoreTexSample(Parameters, Local326, 1);
	FWSVector2 Local328 = WSAdd(DERIV_BASE_VALUE(Local308), MaterialFloat2(0.69999999,0.80000001).rg);
	FWSVector2 Local329 = WSAdd(MaterialFloat2(Local314,Local314), DERIV_BASE_VALUE(Local328));
	MaterialFloat2 Local330 = WSApplyAddressMode(DERIV_BASE_VALUE(Local329), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local331 = MaterialStoreTexCoordScale(Parameters, Local330, 1);
	MaterialFloat4 Local332 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local330,View.MaterialTextureMipBias));
	MaterialFloat Local333 = MaterialStoreTexSample(Parameters, Local332, 1);
	MaterialFloat3 Local334 = (Local326.rgb + Local332.rgb);
	MaterialFloat3 Local335 = (Local321 + Local334);
	MaterialFloat3 Local336 = (Local335 * ((MaterialFloat3)0.20000000));
	MaterialFloat2 Local337 = (Local336.rg * ((MaterialFloat2)Material.PreshaderBuffer[26].z));
	MaterialFloat3 Local338 = normalize(MaterialFloat3(Local337,Local336.b));
	MaterialFloat4 Local339 = Parameters.VertexColor;
	MaterialFloat Local340 = DERIV_BASE_VALUE(Local339).a;
	MaterialFloat Local341 = (1.00000000 - DERIV_BASE_VALUE(Local340));
	MaterialFloat Local342 = (1.00000000 - DERIV_BASE_VALUE(Local341));
	MaterialFloat Local343 = lerp(Local250,Local275,Local301.r);
	MaterialFloat Local344 = lerp(Material.PreshaderBuffer[26].w,Local343,DERIV_BASE_VALUE(Local342));
	MaterialFloat Local345 = lerp(0.00000000,Material.PreshaderBuffer[27].x,Local344);
	MaterialFloat Local346 = (Local345 + 1.00000000);
	MaterialFloat Local347 = saturate(Local346);
	MaterialFloat Local348 = (DERIV_BASE_VALUE(Local342) - Local347);
	MaterialFloat Local349 = (Local348 / -0.50000000);
	MaterialFloat Local350 = saturate(Local349);
	MaterialFloat3 Local351 = lerp(Local302,Local338,Local350);
	MaterialFloat3 Local352 = normalize(Local351);
	MaterialFloat Local353 = saturate(Local343);
	MaterialFloat Local354 = lerp(Local353,Local344,Material.PreshaderBuffer[27].y);
	MaterialFloat Local355 = (1.00000000 - Local354);
	MaterialFloat Local356 = (Local355 * Material.PreshaderBuffer[27].z);
	MaterialFloat Local357 = (Local356 - 1.00000000);
	MaterialFloat Local358 = (Local357 + 0.00000000);
	MaterialFloat Local359 = saturate(Local358);
	MaterialFloat Local360 = lerp(Material.PreshaderBuffer[28].x,Material.PreshaderBuffer[27].w,Local359);
	MaterialFloat Local361 = saturate(Local360);
	MaterialFloat3 Local362 = lerp(Local302,Local352,Local361.r);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local362;

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
	MaterialFloat3 Local363 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[29].xyz,Material.PreshaderBuffer[28].y);
	MaterialFloat Local364 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 6);
	MaterialFloat4 Local365 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_18,GetMaterialSharedSampler(samplerMaterial_Texture2D_18,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local366 = MaterialStoreTexSample(Parameters, Local365, 6);
	MaterialFloat3 Local367 = (Local365.rgb * ((MaterialFloat3)Material.PreshaderBuffer[30].x));
	MaterialFloat Local368 = dot(Local367,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local369 = lerp(Local367,((MaterialFloat3)Local368),Material.PreshaderBuffer[30].y);
	MaterialFloat3 Local370 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[29].w),((MaterialFloat3)0.00000000),Local369);
	MaterialFloat3 Local371 = (Local370 + Local369);
	MaterialFloat3 Local372 = (Local371 * Material.PreshaderBuffer[32].xyz);
	MaterialFloat Local373 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local18), 6);
	MaterialFloat4 Local374 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_19,GetMaterialSharedSampler(samplerMaterial_Texture2D_19,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local18)));
	MaterialFloat Local375 = MaterialStoreTexSample(Parameters, Local374, 6);
	MaterialFloat3 Local376 = (Local374.rgb * ((MaterialFloat3)Material.PreshaderBuffer[33].x));
	MaterialFloat Local377 = dot(Local376,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local378 = lerp(Local376,((MaterialFloat3)Local377),Material.PreshaderBuffer[33].y);
	MaterialFloat3 Local379 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[32].w),((MaterialFloat3)0.00000000),Local378);
	MaterialFloat3 Local380 = (Local379 + Local378);
	MaterialFloat3 Local381 = (Local380 * Material.PreshaderBuffer[34].xyz);
	MaterialFloat3 Local382 = lerp(Local372,Local381,Local59.r);
	MaterialFloat Local383 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local66), 6);
	MaterialFloat4 Local384 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_20,GetMaterialSharedSampler(samplerMaterial_Texture2D_20,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local66)));
	MaterialFloat Local385 = MaterialStoreTexSample(Parameters, Local384, 6);
	MaterialFloat3 Local386 = (Local384.rgb * ((MaterialFloat3)Material.PreshaderBuffer[35].x));
	MaterialFloat Local387 = dot(Local386,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local388 = lerp(Local386,((MaterialFloat3)Local387),Material.PreshaderBuffer[35].y);
	MaterialFloat3 Local389 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[34].w),((MaterialFloat3)0.00000000),Local388);
	MaterialFloat3 Local390 = (Local389 + Local388);
	MaterialFloat3 Local391 = (Local390 * Material.PreshaderBuffer[36].xyz);
	MaterialFloat3 Local392 = lerp(Local382,Local391,Local99.r);
	MaterialFloat Local393 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local106), 6);
	MaterialFloat4 Local394 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_21,GetMaterialSharedSampler(samplerMaterial_Texture2D_21,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local106)));
	MaterialFloat Local395 = MaterialStoreTexSample(Parameters, Local394, 6);
	MaterialFloat3 Local396 = (Local394.rgb * ((MaterialFloat3)Material.PreshaderBuffer[37].x));
	MaterialFloat Local397 = dot(Local396,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local398 = lerp(Local396,((MaterialFloat3)Local397),Material.PreshaderBuffer[37].y);
	MaterialFloat3 Local399 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[36].w),((MaterialFloat3)0.00000000),Local398);
	MaterialFloat3 Local400 = (Local399 + Local398);
	MaterialFloat3 Local401 = (Local400 * Material.PreshaderBuffer[38].xyz);
	MaterialFloat3 Local402 = lerp(Local392,Local401,Local139.r);
	MaterialFloat3 Local403 = (Local402 * ((MaterialFloat3)Material.PreshaderBuffer[39].x));
	MaterialFloat Local404 = dot(Local403,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local405 = lerp(Local403,((MaterialFloat3)Local404),Material.PreshaderBuffer[39].y);
	MaterialFloat3 Local406 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[38].w),((MaterialFloat3)0.00000000),Local405);
	MaterialFloat3 Local407 = (Local406 + Local405);
	MaterialFloat3 Local408 = (Local407 * Material.PreshaderBuffer[40].xyz);
	MaterialFloat3 Local409 = lerp(Local402,Local408,Local154);
	MaterialFloat Local410 = MaterialStoreTexCoordScale(Parameters, Local174, 6);
	MaterialFloat4 Local411 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_22,GetMaterialSharedSampler(samplerMaterial_Texture2D_22,View_MaterialTextureBilinearWrapedSampler),Local174));
	MaterialFloat Local412 = MaterialStoreTexSample(Parameters, Local411, 6);
	MaterialFloat Local413 = MaterialStoreTexCoordScale(Parameters, Local187, 6);
	MaterialFloat4 Local414 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_22,GetMaterialSharedSampler(samplerMaterial_Texture2D_22,View_MaterialTextureBilinearWrapedSampler),Local187));
	MaterialFloat Local415 = MaterialStoreTexSample(Parameters, Local414, 6);
	MaterialFloat3 Local416 = lerp(Local411.rgb,Local414.rgb,DERIV_BASE_VALUE(Local262));
	MaterialFloat Local417 = MaterialStoreTexCoordScale(Parameters, Local225, 6);
	MaterialFloat4 Local418 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_22,GetMaterialSharedSampler(samplerMaterial_Texture2D_22,View_MaterialTextureBilinearWrapedSampler),Local225));
	MaterialFloat Local419 = MaterialStoreTexSample(Parameters, Local418, 6);
	MaterialFloat3 Local420 = lerp(Local416,Local418.rgb,DERIV_BASE_VALUE(Local271));
	MaterialFloat3 Local421 = (Local420 * ((MaterialFloat3)Material.PreshaderBuffer[41].x));
	MaterialFloat Local422 = dot(Local421,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local423 = lerp(Local421,((MaterialFloat3)Local422),Material.PreshaderBuffer[41].y);
	MaterialFloat3 Local424 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[40].w),((MaterialFloat3)0.00000000),Local423);
	MaterialFloat3 Local425 = (Local424 + Local423);
	MaterialFloat3 Local426 = (Local425 * Material.PreshaderBuffer[42].xyz);
	MaterialFloat3 Local427 = lerp(Local409,Local426,Local301.r);
	MaterialFloat3 Local428 = (Local427 * Material.PreshaderBuffer[43].xyz);
	MaterialFloat Local429 = (DERIV_BASE_VALUE(Local342) + Material.PreshaderBuffer[43].w);
	MaterialFloat Local430 = saturate(DERIV_BASE_VALUE(Local429));
	MaterialFloat Local431 = (Local344 - DERIV_BASE_VALUE(Local430));
	MaterialFloat Local432 = (Local431 + Material.PreshaderBuffer[44].x);
	MaterialFloat Local433 = saturate(Local432);
	MaterialFloat3 Local434 = lerp(Local427,Local428,Local433);
	MaterialFloat Local435 = (Local350 * Material.PreshaderBuffer[44].y);
	MaterialFloat Local436 = saturate(Local435);
	MaterialFloat Local437 = dot(Parameters.CameraVector,DERIV_BASE_VALUE(Local155));
	MaterialFloat Local438 = saturate(Local437);
	MaterialFloat Local439 = PositiveClampedPow(Local438,Material.PreshaderBuffer[44].z);
	MaterialFloat Local440 = (1.00000000 - Local439);
	MaterialFloat Local441 = (Local436 * Local440);
	MaterialFloat3 Local442 = lerp(Local434,Material.PreshaderBuffer[45].xyz,Local441);
	MaterialFloat3 Local443 = lerp(Local427,Local442,Local361.r);
	MaterialFloat Local444 = lerp(Material.PreshaderBuffer[45].w,Material.PreshaderBuffer[46].x,Local59.r);
	MaterialFloat Local445 = lerp(Local444,Material.PreshaderBuffer[46].y,Local99.r);
	MaterialFloat Local446 = lerp(Local445,Material.PreshaderBuffer[46].z,Local139.r);
	MaterialFloat Local447 = lerp(Local446,Material.PreshaderBuffer[46].w,Local301.r);
	MaterialFloat Local448 = lerp(Local447,0.00000000,Local361.r);
	MaterialFloat Local449 = lerp(Material.PreshaderBuffer[47].x,Material.PreshaderBuffer[47].y,Local59.r);
	MaterialFloat Local450 = lerp(Local449,Material.PreshaderBuffer[47].z,Local99.r);
	MaterialFloat Local451 = lerp(Local450,Material.PreshaderBuffer[47].w,Local139.r);
	MaterialFloat Local452 = lerp(Local451,Material.PreshaderBuffer[48].x,Local301.r);
	MaterialFloat Local453 = lerp(Local452,0.50000000,Local361.r);
	MaterialFloat Local454 = lerp(Material.PreshaderBuffer[48].z,Material.PreshaderBuffer[48].y,Local25.g);
	MaterialFloat Local455 = lerp(Material.PreshaderBuffer[49].x,Material.PreshaderBuffer[48].w,Local32.g);
	MaterialFloat Local456 = lerp(Local454,Local455,Local59.r);
	MaterialFloat Local457 = lerp(Material.PreshaderBuffer[49].z,Material.PreshaderBuffer[49].y,Local75.g);
	MaterialFloat Local458 = lerp(Local456,Local457,Local99.r);
	MaterialFloat Local459 = lerp(Material.PreshaderBuffer[50].x,Material.PreshaderBuffer[49].w,Local115.g);
	MaterialFloat Local460 = lerp(Local458,Local459,Local139.r);
	MaterialFloat Local461 = lerp(Material.PreshaderBuffer[50].z,Material.PreshaderBuffer[50].y,Local460);
	MaterialFloat Local462 = lerp(Local460,Local461,Local154);
	MaterialFloat Local463 = lerp(Material.PreshaderBuffer[51].x,Material.PreshaderBuffer[50].w,Local272.g);
	MaterialFloat Local464 = lerp(Local462,Local463,Local301.r);
	MaterialFloat Local465 = lerp(Local464,Material.PreshaderBuffer[51].y,Local433);
	MaterialFloat Local466 = lerp(Local465,Material.PreshaderBuffer[51].z,Local350);
	MaterialFloat Local467 = lerp(Local464,Local466,Local361.r);
	MaterialFloat Local658 = PositiveClampedPow(Local25.r,Material.PreshaderBuffer[51].w);
	MaterialFloat Local659 = (Local658 + Material.PreshaderBuffer[52].x);
	MaterialFloat Local660 = PositiveClampedPow(Local32.r,Material.PreshaderBuffer[52].y);
	MaterialFloat Local661 = (Local660 + Material.PreshaderBuffer[52].z);
	MaterialFloat Local662 = lerp(Local659,Local661,Local59.r);
	MaterialFloat Local663 = PositiveClampedPow(Local75.r,Material.PreshaderBuffer[52].w);
	MaterialFloat Local664 = (Local663 + Material.PreshaderBuffer[53].x);
	MaterialFloat Local665 = lerp(Local662,Local664,Local99.r);
	MaterialFloat Local666 = PositiveClampedPow(Local115.r,Material.PreshaderBuffer[53].y);
	MaterialFloat Local667 = (Local666 + Material.PreshaderBuffer[53].z);
	MaterialFloat Local668 = lerp(Local665,Local667,Local139.r);
	MaterialFloat Local669 = PositiveClampedPow(Local272.r,Material.PreshaderBuffer[53].w);
	MaterialFloat Local670 = (Local669 + Material.PreshaderBuffer[54].x);
	MaterialFloat Local671 = lerp(Local668,Local670,Local301.r);
	MaterialFloat Local672 = lerp(Local671,1.00000000,Local350);
	MaterialFloat Local673 = lerp(Local671,Local672,Local361.r);

	PixelMaterialInputs.EmissiveColor = Local363;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local443;
	PixelMaterialInputs.Metallic = Local448;
	PixelMaterialInputs.Specular = Local453;
	PixelMaterialInputs.Roughness = Local467;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local362;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = Local673;
	PixelMaterialInputs.Refraction = 0;
	PixelMaterialInputs.PixelDepthOffset = 0.00000000;
	PixelMaterialInputs.ShadingModel = 1;
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
	//if( PixelMaterialInputs.OpacityMask < 0.333 ) discard;
	//o.Alpha = PixelMaterialInputs.OpacityMask;

	o.Metallic = PixelMaterialInputs.Metallic;
	o.Smoothness = 1.0 - PixelMaterialInputs.Roughness;
	o.Normal = normalize( PixelMaterialInputs.Normal );
	o.Emission = PixelMaterialInputs.EmissiveColor.rgb;
	o.Occlusion = PixelMaterialInputs.AmbientOcclusion;
}