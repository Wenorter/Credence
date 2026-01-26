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
uniform float Rotation_custom_TopTint_layer;
uniform float U_tile_TopTint_layer;
uniform float V_tile_TopTint_layer;
uniform float UV_Tiling_TopTint_layer;
uniform float Base_UV___U_Tiling_TopTint_layer;
uniform float Base_UV___V_Tiling_TopTint_layer;
uniform float Normal_intensity_TopTint_layer;
uniform float TopTint_layer_contrast;
uniform float Disp_Contrast_TopTint_layer;
uniform float Disp_Intensity_TopTint_layer;
uniform float TopTint_layer_blend;
uniform float TopTint_layer_extend;
uniform float BlendSharp_2;
uniform float BlendSharp_1;
uniform float Tiling_noise;
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
	float4 PreshaderBuffer[54];
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
	Material.PreshaderBuffer[15] = float4(0.000000,0.000000,25.000000,25.000000);//(Unknown)
	Material.PreshaderBuffer[16] = float4(1.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[17] = float4(2.265750,1.685635,1.000000,150.000000);//(Unknown)
	Material.PreshaderBuffer[18] = float4(4.000000,2.000000,-1.000000,1.370000);//(Unknown)
	Material.PreshaderBuffer[19] = float4(-0.370000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[20] = float4(0.664000,0.336000,1.000000,-0.000000);//(Unknown)
	Material.PreshaderBuffer[21] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[22] = float4(20.000000,20.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[23] = float4(0.000000,1.000000,1.000000,0.300000);//(Unknown)
	Material.PreshaderBuffer[24] = float4(1.000000,1.000000,5.000000,3.221240);//(Unknown)
	Material.PreshaderBuffer[25] = float4(2.000000,-1.000000,0.001000,1.000000);//(Unknown)
	Material.PreshaderBuffer[26] = float4(0.100000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[27] = float4(4.000000,2.000000,-1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[28] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[29] = float4(0.616000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[30] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[31] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[32] = float4(0.700000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[33] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[34] = float4(0.750000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[35] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[36] = float4(1.000000,0.264000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[37] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[38] = float4(0.608000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[39] = float4(0.611328,0.572602,0.527748,0.000000);//(Unknown)
	Material.PreshaderBuffer[40] = float4(1.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[41] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[42] = float4(0.000000,0.000000,0.000000,-0.800000);//(Unknown)
	Material.PreshaderBuffer[43] = float4(-0.950000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[44] = float4(0.009258,0.044271,0.040740,0.000000);//(Unknown)
	Material.PreshaderBuffer[45] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[46] = float4(0.150000,0.150000,0.150000,0.150000);//(Unknown)
	Material.PreshaderBuffer[47] = float4(0.150000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[48] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[49] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[50] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[51] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[52] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[53] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)

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
	Material.PreshaderBuffer[20].zw = Append(Cos( Rotation_custom_TopTint_layer * 6.283 ), Sin( Rotation_custom_TopTint_layer * 6.283 ) * -1);
	Material.PreshaderBuffer[21].xy = Append(Sin( Rotation_custom_TopTint_layer * 6.283 ), Cos( Rotation_custom_TopTint_layer * 6.283 ));
	Material.PreshaderBuffer[21].zw = Append(U_tile_TopTint_layer, V_tile_TopTint_layer);
	Material.PreshaderBuffer[22].xy = UV_Tiling_TopTint_layer * Append(Base_UV___U_Tiling_TopTint_layer, Base_UV___V_Tiling_TopTint_layer);
	Material.PreshaderBuffer[22].z = Normal_intensity_TopTint_layer;
	Material.PreshaderBuffer[22].w = Disp_Contrast_TopTint_layer + 1;
	Material.PreshaderBuffer[23].x = 0 - Disp_Contrast_TopTint_layer;
	Material.PreshaderBuffer[23].y = Disp_Intensity_TopTint_layer;
	Material.PreshaderBuffer[23].z = TopTint_layer_blend;
	Material.PreshaderBuffer[23].w = TopTint_layer_extend;
	Material.PreshaderBuffer[24].x = Tiling_noise;
	Material.PreshaderBuffer[24].y = noise_power;
	Material.PreshaderBuffer[24].z = BlendSharp_1;
	Material.PreshaderBuffer[24].w = BlendSharp_2;
	Material.PreshaderBuffer[25].x = TopTint_layer_contrast + 1;
	Material.PreshaderBuffer[25].y = 0 - TopTint_layer_contrast;
	Material.PreshaderBuffer[25].z = wave_speed;
	Material.PreshaderBuffer[25].w = wave_scale;
	Material.PreshaderBuffer[26].x = wave_Normal_int;
	Material.PreshaderBuffer[26].y = getting_wet_Height_setting;
	Material.PreshaderBuffer[26].z = getting_wet_depth;
	Material.PreshaderBuffer[26].w = layer_getting_wet_blend;
	Material.PreshaderBuffer[27].x = layer_getting_wet_extend;
	Material.PreshaderBuffer[27].y = layer_getting_wet_contrast + 1;
	Material.PreshaderBuffer[27].z = 0 - layer_getting_wet_contrast;
	Material.PreshaderBuffer[27].w = SelectionColor.w;
	Material.PreshaderBuffer[28].xyz = SelectionColor.xyz;
	Material.PreshaderBuffer[28].w = color_tone_main_layer * 6.283;
	Material.PreshaderBuffer[29].x = Brightness_main_layer;
	Material.PreshaderBuffer[29].y = contrast_main_layer;
	Material.PreshaderBuffer[30].xyz = Tint_base_color_main_layer.xyz;
	Material.PreshaderBuffer[30].w = color_tone_R_channel * 6.283;
	Material.PreshaderBuffer[32].x = Brightness_R_channel;
	Material.PreshaderBuffer[32].y = contrast_R_channel;
	Material.PreshaderBuffer[33].xyz = Tint_base_color_R_channel.xyz;
	Material.PreshaderBuffer[33].w = color_tone_G_channel * 6.283;
	Material.PreshaderBuffer[34].x = Brightness_G_channel;
	Material.PreshaderBuffer[34].y = contrast_G_channel;
	Material.PreshaderBuffer[35].xyz = Tint_base_color_G_channel.xyz;
	Material.PreshaderBuffer[35].w = color_tone_B_channel * 6.283;
	Material.PreshaderBuffer[36].x = Brightness_B_channel;
	Material.PreshaderBuffer[36].y = contrast_B_channel;
	Material.PreshaderBuffer[37].xyz = Tint_base_color_B_channel.xyz;
	Material.PreshaderBuffer[37].w = color_tone_B_all_mask * 6.283;
	Material.PreshaderBuffer[38].x = Brightness_B_all_mask;
	Material.PreshaderBuffer[38].y = contrast_B_all_mask;
	Material.PreshaderBuffer[39].xyz = Tint_base_color_B_all_mask.xyz;
	Material.PreshaderBuffer[39].w = color_tone_TopTint_layer * 6.283;
	Material.PreshaderBuffer[40].x = Brightness_TopTint_layer;
	Material.PreshaderBuffer[40].y = contrast_TopTint_layer;
	Material.PreshaderBuffer[41].xyz = Tint_base_color_TopTint_layer.xyz;
	Material.PreshaderBuffer[42].xyz = getting_wet_color_depth.xyz;
	Material.PreshaderBuffer[42].w = wetness_limit;
	Material.PreshaderBuffer[43].x = wetness_limit_bloom;
	Material.PreshaderBuffer[43].y = getting_wet_opacity;
	Material.PreshaderBuffer[43].z = getting_wet_contrast;
	Material.PreshaderBuffer[44].xyz = getting_wet_color.xyz;
	Material.PreshaderBuffer[44].w = Metallic_main_layer;
	Material.PreshaderBuffer[45].x = Metallic_R_channel;
	Material.PreshaderBuffer[45].y = Metallic_G_channel;
	Material.PreshaderBuffer[45].z = Metallic_B_channel;
	Material.PreshaderBuffer[45].w = Metallic_TopTint_layer;
	Material.PreshaderBuffer[46].x = Specular_main_layer;
	Material.PreshaderBuffer[46].y = Specular_R_channel;
	Material.PreshaderBuffer[46].z = Specular_G_channel;
	Material.PreshaderBuffer[46].w = Specular_B_channel;
	Material.PreshaderBuffer[47].x = Specular_TopTint_layer;
	Material.PreshaderBuffer[47].y = Roughness_max_main_layer;
	Material.PreshaderBuffer[47].z = Roughness_min_main_layer;
	Material.PreshaderBuffer[47].w = Roughness_max_R_channel;
	Material.PreshaderBuffer[48].x = Roughness_min_R_channel;
	Material.PreshaderBuffer[48].y = Roughness_max_G_channel;
	Material.PreshaderBuffer[48].z = Roughness_min_G_channel;
	Material.PreshaderBuffer[48].w = Roughness_max_B_channel;
	Material.PreshaderBuffer[49].x = Roughness_min_B_channel;
	Material.PreshaderBuffer[49].y = Roughness_max_B_all_mask;
	Material.PreshaderBuffer[49].z = Roughness_min_B_all_mask;
	Material.PreshaderBuffer[49].w = Roughness_max_TopTint_layer;
	Material.PreshaderBuffer[50].x = Roughness_min_TopTint_layer;
	Material.PreshaderBuffer[50].y = getting_wet_Roughness_bloom;
	Material.PreshaderBuffer[50].z = getting_wet_Roughness;
	Material.PreshaderBuffer[50].w = AO_Contrast_main_layer;
	Material.PreshaderBuffer[51].x = 1 - AO_Intensity_main_layer;
	Material.PreshaderBuffer[51].y = AO_Contrast_R_channel;
	Material.PreshaderBuffer[51].z = 1 - AO_Intensity_R_channel;
	Material.PreshaderBuffer[51].w = AO_Contrast_G_channel;
	Material.PreshaderBuffer[52].x = 1 - AO_Intensity_G_channel;
	Material.PreshaderBuffer[52].y = AO_Contrast_B_channel;
	Material.PreshaderBuffer[52].z = 1 - AO_Intensity_B_channel;
	Material.PreshaderBuffer[52].w = AO_Contrast_TopTint_layer;
	Material.PreshaderBuffer[53].x = 1 - AO_Intensity_TopTint_layer;
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
	MaterialFloat Local155 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[20].zw);
	MaterialFloat Local156 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[21].xy);
	MaterialFloat2 Local157 = MaterialFloat2(DERIV_BASE_VALUE(Local155),DERIV_BASE_VALUE(Local156));
	MaterialFloat2 Local158 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local157));
	MaterialFloat2 Local159 = (DERIV_BASE_VALUE(Local158) + Material.PreshaderBuffer[21].zw);
	MaterialFloat2 Local160 = (DERIV_BASE_VALUE(Local159) * Material.PreshaderBuffer[22].xy);
	MaterialFloat Local161 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local160), 5);
	MaterialFloat4 Local162 = UnpackNormalMap(Texture2DSample(Material_Texture2D_14,GetMaterialSharedSampler(samplerMaterial_Texture2D_14,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local160)));
	MaterialFloat Local163 = MaterialStoreTexSample(Parameters, Local162, 5);
	MaterialFloat Local164 = (Local162.rgb.r * Material.PreshaderBuffer[22].z);
	MaterialFloat Local165 = (Local162.rgb.g * Material.PreshaderBuffer[22].z);
	MaterialFloat Local166 = lerp(Local112,Local119,Local139.r);
	MaterialFloat Local167 = saturate(Local166);
	MaterialFloat Local168 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local160), 7);
	MaterialFloat4 Local169 = Texture2DSample(Material_Texture2D_15,GetMaterialSharedSampler(samplerMaterial_Texture2D_15,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local160));
	MaterialFloat Local170 = MaterialStoreTexSample(Parameters, Local169, 7);
	MaterialFloat Local171 = lerp(Material.PreshaderBuffer[23].x,Material.PreshaderBuffer[22].w,Local169.b);
	MaterialFloat Local172 = saturate(Local171);
	MaterialFloat Local173 = (Local172.r * Material.PreshaderBuffer[23].y);
	MaterialFloat Local174 = lerp(Local167,Local173,Material.PreshaderBuffer[23].z);
	MaterialFloat Local175 = (1.00000000 - Local174);
	MaterialFloat Local176 = (Local175 * Material.PreshaderBuffer[23].w);
	MaterialFloat Local177 = (Local176 - 1.00000000);
	MaterialFloat3 Local178 = Parameters.TangentToWorld[2];
	MaterialFloat Local179 = dot(DERIV_BASE_VALUE(Local178),normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb));
	MaterialFloat Local180 = (DERIV_BASE_VALUE(Local179) * 0.50000000);
	MaterialFloat Local181 = (DERIV_BASE_VALUE(Local180) + 0.50000000);
	MaterialFloat2 Local182 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[24].x));
	MaterialFloat Local183 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local182), 8);
	MaterialFloat4 Local184 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_16,GetMaterialSharedSampler(samplerMaterial_Texture2D_16,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local182)));
	MaterialFloat Local185 = MaterialStoreTexSample(Parameters, Local184, 8);
	MaterialFloat3 Local186 = (Local184.rgb * ((MaterialFloat3)Material.PreshaderBuffer[24].y));
	MaterialFloat Local187 = lerp(Material.PreshaderBuffer[24].w,Material.PreshaderBuffer[24].z,Local186.r);
	MaterialFloat Local188 = (DERIV_BASE_VALUE(Local181) * Local187);
	MaterialFloat Local189 = (Local187 * 0.50000000);
	MaterialFloat Local190 = (-1.00000000 - Local189);
	MaterialFloat Local191 = (Local188 + Local190);
	MaterialFloat Local192 = saturate(Local191);
	MaterialFloat Local193 = (Local192 * 2.00000000);
	MaterialFloat Local194 = (Local177 + Local193);
	MaterialFloat Local195 = saturate(Local194);
	MaterialFloat Local196 = lerp(Material.PreshaderBuffer[25].y,Material.PreshaderBuffer[25].x,Local195);
	MaterialFloat Local197 = saturate(Local196);
	MaterialFloat3 Local198 = lerp(Local140,MaterialFloat3(MaterialFloat2(Local164,Local165),Local162.rgb.b),Local197.r);
	MaterialFloat Local199 = (Material.PreshaderBuffer[25].z * View.GameTime);
	MaterialFloat Local200 = (Local199 * 0.10000000);
	FWSVector3 Local201 = GetWorldPosition(Parameters);
	FWSVector3 Local202 = MakeWSVector(WSGetX(DERIV_BASE_VALUE(Local201)), WSGetY(DERIV_BASE_VALUE(Local201)), WSGetZ(DERIV_BASE_VALUE(Local201)));
	FWSVector3 Local203 = WSDivide(DERIV_BASE_VALUE(Local202), ((MaterialFloat3)1000.00000000));
	FWSVector3 Local204 = WSMultiply(DERIV_BASE_VALUE(Local203), ((MaterialFloat3)Material.PreshaderBuffer[25].w));
	FWSVector2 Local205 = MakeWSVector(WSGetComponent(DERIV_BASE_VALUE(Local204), 0),WSGetComponent(DERIV_BASE_VALUE(Local204), 1));
	FWSVector2 Local206 = WSAdd(MaterialFloat2(Local200,Local200), DERIV_BASE_VALUE(Local205));
	MaterialFloat2 Local207 = WSApplyAddressMode(DERIV_BASE_VALUE(Local206), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local208 = MaterialStoreTexCoordScale(Parameters, Local207, 1);
	MaterialFloat4 Local209 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local207,View.MaterialTextureMipBias));
	MaterialFloat Local210 = MaterialStoreTexSample(Parameters, Local209, 1);
	MaterialFloat Local211 = (Local199 * -0.10000000);
	FWSVector2 Local212 = WSAdd(DERIV_BASE_VALUE(Local205), MaterialFloat2(0.50000000,0.40000001).rg);
	FWSVector2 Local213 = WSAdd(MaterialFloat2(Local211,Local200), DERIV_BASE_VALUE(Local212));
	MaterialFloat2 Local214 = WSApplyAddressMode(DERIV_BASE_VALUE(Local213), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local215 = MaterialStoreTexCoordScale(Parameters, Local214, 1);
	MaterialFloat4 Local216 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local214,View.MaterialTextureMipBias));
	MaterialFloat Local217 = MaterialStoreTexSample(Parameters, Local216, 1);
	MaterialFloat3 Local218 = (Local209.rgb + Local216.rgb);
	FWSVector2 Local219 = WSAdd(DERIV_BASE_VALUE(Local205), MaterialFloat2(0.89999998,0.20000000).rg);
	FWSVector2 Local220 = WSAdd(MaterialFloat2(Local200,Local211), DERIV_BASE_VALUE(Local219));
	MaterialFloat2 Local221 = WSApplyAddressMode(DERIV_BASE_VALUE(Local220), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local222 = MaterialStoreTexCoordScale(Parameters, Local221, 1);
	MaterialFloat4 Local223 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local221,View.MaterialTextureMipBias));
	MaterialFloat Local224 = MaterialStoreTexSample(Parameters, Local223, 1);
	FWSVector2 Local225 = WSAdd(DERIV_BASE_VALUE(Local205), MaterialFloat2(0.69999999,0.80000001).rg);
	FWSVector2 Local226 = WSAdd(MaterialFloat2(Local211,Local211), DERIV_BASE_VALUE(Local225));
	MaterialFloat2 Local227 = WSApplyAddressMode(DERIV_BASE_VALUE(Local226), LWCADDRESSMODE_WRAP, LWCADDRESSMODE_WRAP);
	MaterialFloat Local228 = MaterialStoreTexCoordScale(Parameters, Local227, 1);
	MaterialFloat4 Local229 = UnpackNormalMap(Texture2DSampleBias(Material_Texture2D_17,samplerMaterial_Texture2D_17,Local227,View.MaterialTextureMipBias));
	MaterialFloat Local230 = MaterialStoreTexSample(Parameters, Local229, 1);
	MaterialFloat3 Local231 = (Local223.rgb + Local229.rgb);
	MaterialFloat3 Local232 = (Local218 + Local231);
	MaterialFloat3 Local233 = (Local232 * ((MaterialFloat3)0.20000000));
	MaterialFloat2 Local234 = (Local233.rg * ((MaterialFloat2)Material.PreshaderBuffer[26].x));
	MaterialFloat3 Local235 = normalize(MaterialFloat3(Local234,Local233.b));
	MaterialFloat4 Local236 = Parameters.VertexColor;
	MaterialFloat Local237 = DERIV_BASE_VALUE(Local236).a;
	MaterialFloat Local238 = (1.00000000 - DERIV_BASE_VALUE(Local237));
	MaterialFloat Local239 = (1.00000000 - DERIV_BASE_VALUE(Local238));
	MaterialFloat Local240 = lerp(Local166,Local173,Local197.r);
	MaterialFloat Local241 = lerp(Material.PreshaderBuffer[26].y,Local240,DERIV_BASE_VALUE(Local239));
	MaterialFloat Local242 = lerp(0.00000000,Material.PreshaderBuffer[26].z,Local241);
	MaterialFloat Local243 = (Local242 + 1.00000000);
	MaterialFloat Local244 = saturate(Local243);
	MaterialFloat Local245 = (DERIV_BASE_VALUE(Local239) - Local244);
	MaterialFloat Local246 = (Local245 / -0.50000000);
	MaterialFloat Local247 = saturate(Local246);
	MaterialFloat3 Local248 = lerp(Local198,Local235,Local247);
	MaterialFloat3 Local249 = normalize(Local248);
	MaterialFloat Local250 = saturate(Local240);
	MaterialFloat Local251 = lerp(Local250,Local241,Material.PreshaderBuffer[26].w);
	MaterialFloat Local252 = (1.00000000 - Local251);
	MaterialFloat Local253 = (Local252 * Material.PreshaderBuffer[27].x);
	MaterialFloat Local254 = (Local253 - 1.00000000);
	MaterialFloat Local255 = (Local254 + 0.00000000);
	MaterialFloat Local256 = saturate(Local255);
	MaterialFloat Local257 = lerp(Material.PreshaderBuffer[27].z,Material.PreshaderBuffer[27].y,Local256);
	MaterialFloat Local258 = saturate(Local257);
	MaterialFloat3 Local259 = lerp(Local198,Local249,Local258.r);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local259;

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
	MaterialFloat3 Local260 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[28].xyz,Material.PreshaderBuffer[27].w);
	MaterialFloat Local261 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 6);
	MaterialFloat4 Local262 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_18,GetMaterialSharedSampler(samplerMaterial_Texture2D_18,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local263 = MaterialStoreTexSample(Parameters, Local262, 6);
	MaterialFloat3 Local264 = (Local262.rgb * ((MaterialFloat3)Material.PreshaderBuffer[29].x));
	MaterialFloat Local265 = dot(Local264,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local266 = lerp(Local264,((MaterialFloat3)Local265),Material.PreshaderBuffer[29].y);
	MaterialFloat3 Local267 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[28].w),((MaterialFloat3)0.00000000),Local266);
	MaterialFloat3 Local268 = (Local267 + Local266);
	MaterialFloat3 Local269 = (Local268 * Material.PreshaderBuffer[30].xyz);
	MaterialFloat Local270 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local18), 6);
	MaterialFloat4 Local271 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_19,GetMaterialSharedSampler(samplerMaterial_Texture2D_19,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local18)));
	MaterialFloat Local272 = MaterialStoreTexSample(Parameters, Local271, 6);
	MaterialFloat3 Local273 = (Local271.rgb * ((MaterialFloat3)Material.PreshaderBuffer[32].x));
	MaterialFloat Local274 = dot(Local273,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local275 = lerp(Local273,((MaterialFloat3)Local274),Material.PreshaderBuffer[32].y);
	MaterialFloat3 Local276 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[30].w),((MaterialFloat3)0.00000000),Local275);
	MaterialFloat3 Local277 = (Local276 + Local275);
	MaterialFloat3 Local278 = (Local277 * Material.PreshaderBuffer[33].xyz);
	MaterialFloat3 Local279 = lerp(Local269,Local278,Local59.r);
	MaterialFloat Local280 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local66), 6);
	MaterialFloat4 Local281 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_20,GetMaterialSharedSampler(samplerMaterial_Texture2D_20,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local66)));
	MaterialFloat Local282 = MaterialStoreTexSample(Parameters, Local281, 6);
	MaterialFloat3 Local283 = (Local281.rgb * ((MaterialFloat3)Material.PreshaderBuffer[34].x));
	MaterialFloat Local284 = dot(Local283,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local285 = lerp(Local283,((MaterialFloat3)Local284),Material.PreshaderBuffer[34].y);
	MaterialFloat3 Local286 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[33].w),((MaterialFloat3)0.00000000),Local285);
	MaterialFloat3 Local287 = (Local286 + Local285);
	MaterialFloat3 Local288 = (Local287 * Material.PreshaderBuffer[35].xyz);
	MaterialFloat3 Local289 = lerp(Local279,Local288,Local99.r);
	MaterialFloat Local290 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local106), 6);
	MaterialFloat4 Local291 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_21,GetMaterialSharedSampler(samplerMaterial_Texture2D_21,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local106)));
	MaterialFloat Local292 = MaterialStoreTexSample(Parameters, Local291, 6);
	MaterialFloat3 Local293 = (Local291.rgb * ((MaterialFloat3)Material.PreshaderBuffer[36].x));
	MaterialFloat Local294 = dot(Local293,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local295 = lerp(Local293,((MaterialFloat3)Local294),Material.PreshaderBuffer[36].y);
	MaterialFloat3 Local296 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[35].w),((MaterialFloat3)0.00000000),Local295);
	MaterialFloat3 Local297 = (Local296 + Local295);
	MaterialFloat3 Local298 = (Local297 * Material.PreshaderBuffer[37].xyz);
	MaterialFloat3 Local299 = lerp(Local289,Local298,Local139.r);
	MaterialFloat3 Local300 = (Local299 * ((MaterialFloat3)Material.PreshaderBuffer[38].x));
	MaterialFloat Local301 = dot(Local300,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local302 = lerp(Local300,((MaterialFloat3)Local301),Material.PreshaderBuffer[38].y);
	MaterialFloat3 Local303 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[37].w),((MaterialFloat3)0.00000000),Local302);
	MaterialFloat3 Local304 = (Local303 + Local302);
	MaterialFloat3 Local305 = (Local304 * Material.PreshaderBuffer[39].xyz);
	MaterialFloat3 Local306 = lerp(Local299,Local305,Local154);
	MaterialFloat Local307 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local160), 6);
	MaterialFloat4 Local308 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_22,GetMaterialSharedSampler(samplerMaterial_Texture2D_22,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local160)));
	MaterialFloat Local309 = MaterialStoreTexSample(Parameters, Local308, 6);
	MaterialFloat3 Local310 = (Local308.rgb * ((MaterialFloat3)Material.PreshaderBuffer[40].x));
	MaterialFloat Local311 = dot(Local310,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local312 = lerp(Local310,((MaterialFloat3)Local311),Material.PreshaderBuffer[40].y);
	MaterialFloat3 Local313 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[39].w),((MaterialFloat3)0.00000000),Local312);
	MaterialFloat3 Local314 = (Local313 + Local312);
	MaterialFloat3 Local315 = (Local314 * Material.PreshaderBuffer[41].xyz);
	MaterialFloat3 Local316 = lerp(Local306,Local315,Local197.r);
	MaterialFloat3 Local317 = (Local316 * Material.PreshaderBuffer[42].xyz);
	MaterialFloat Local318 = (DERIV_BASE_VALUE(Local239) + Material.PreshaderBuffer[42].w);
	MaterialFloat Local319 = saturate(DERIV_BASE_VALUE(Local318));
	MaterialFloat Local320 = (Local241 - DERIV_BASE_VALUE(Local319));
	MaterialFloat Local321 = (Local320 + Material.PreshaderBuffer[43].x);
	MaterialFloat Local322 = saturate(Local321);
	MaterialFloat3 Local323 = lerp(Local316,Local317,Local322);
	MaterialFloat Local324 = (Local247 * Material.PreshaderBuffer[43].y);
	MaterialFloat Local325 = saturate(Local324);
	MaterialFloat Local326 = dot(Parameters.CameraVector,DERIV_BASE_VALUE(Local178));
	MaterialFloat Local327 = saturate(Local326);
	MaterialFloat Local328 = PositiveClampedPow(Local327,Material.PreshaderBuffer[43].z);
	MaterialFloat Local329 = (1.00000000 - Local328);
	MaterialFloat Local330 = (Local325 * Local329);
	MaterialFloat3 Local331 = lerp(Local323,Material.PreshaderBuffer[44].xyz,Local330);
	MaterialFloat3 Local332 = lerp(Local316,Local331,Local258.r);
	MaterialFloat Local333 = lerp(Material.PreshaderBuffer[44].w,Material.PreshaderBuffer[45].x,Local59.r);
	MaterialFloat Local334 = lerp(Local333,Material.PreshaderBuffer[45].y,Local99.r);
	MaterialFloat Local335 = lerp(Local334,Material.PreshaderBuffer[45].z,Local139.r);
	MaterialFloat Local336 = lerp(Local335,Material.PreshaderBuffer[45].w,Local197.r);
	MaterialFloat Local337 = lerp(Local336,0.00000000,Local258.r);
	MaterialFloat Local338 = lerp(Material.PreshaderBuffer[46].x,Material.PreshaderBuffer[46].y,Local59.r);
	MaterialFloat Local339 = lerp(Local338,Material.PreshaderBuffer[46].z,Local99.r);
	MaterialFloat Local340 = lerp(Local339,Material.PreshaderBuffer[46].w,Local139.r);
	MaterialFloat Local341 = lerp(Local340,Material.PreshaderBuffer[47].x,Local197.r);
	MaterialFloat Local342 = lerp(Local341,0.50000000,Local258.r);
	MaterialFloat Local343 = lerp(Material.PreshaderBuffer[47].z,Material.PreshaderBuffer[47].y,Local25.g);
	MaterialFloat Local344 = lerp(Material.PreshaderBuffer[48].x,Material.PreshaderBuffer[47].w,Local32.g);
	MaterialFloat Local345 = lerp(Local343,Local344,Local59.r);
	MaterialFloat Local346 = lerp(Material.PreshaderBuffer[48].z,Material.PreshaderBuffer[48].y,Local75.g);
	MaterialFloat Local347 = lerp(Local345,Local346,Local99.r);
	MaterialFloat Local348 = lerp(Material.PreshaderBuffer[49].x,Material.PreshaderBuffer[48].w,Local115.g);
	MaterialFloat Local349 = lerp(Local347,Local348,Local139.r);
	MaterialFloat Local350 = lerp(Material.PreshaderBuffer[49].z,Material.PreshaderBuffer[49].y,Local349);
	MaterialFloat Local351 = lerp(Local349,Local350,Local154);
	MaterialFloat Local352 = lerp(Material.PreshaderBuffer[50].x,Material.PreshaderBuffer[49].w,Local169.g);
	MaterialFloat Local353 = lerp(Local351,Local352,Local197.r);
	MaterialFloat Local354 = lerp(Local353,Material.PreshaderBuffer[50].y,Local322);
	MaterialFloat Local355 = lerp(Local354,Material.PreshaderBuffer[50].z,Local247);
	MaterialFloat Local356 = lerp(Local353,Local355,Local258.r);
	MaterialFloat Local517 = PositiveClampedPow(Local25.r,Material.PreshaderBuffer[50].w);
	MaterialFloat Local518 = (Local517 + Material.PreshaderBuffer[51].x);
	MaterialFloat Local519 = PositiveClampedPow(Local32.r,Material.PreshaderBuffer[51].y);
	MaterialFloat Local520 = (Local519 + Material.PreshaderBuffer[51].z);
	MaterialFloat Local521 = lerp(Local518,Local520,Local59.r);
	MaterialFloat Local522 = PositiveClampedPow(Local75.r,Material.PreshaderBuffer[51].w);
	MaterialFloat Local523 = (Local522 + Material.PreshaderBuffer[52].x);
	MaterialFloat Local524 = lerp(Local521,Local523,Local99.r);
	MaterialFloat Local525 = PositiveClampedPow(Local115.r,Material.PreshaderBuffer[52].y);
	MaterialFloat Local526 = (Local525 + Material.PreshaderBuffer[52].z);
	MaterialFloat Local527 = lerp(Local524,Local526,Local139.r);
	MaterialFloat Local528 = PositiveClampedPow(Local169.r,Material.PreshaderBuffer[52].w);
	MaterialFloat Local529 = (Local528 + Material.PreshaderBuffer[53].x);
	MaterialFloat Local530 = lerp(Local527,Local529,Local197.r);
	MaterialFloat Local531 = lerp(Local530,1.00000000,Local247);
	MaterialFloat Local532 = lerp(Local530,Local531,Local258.r);

	PixelMaterialInputs.EmissiveColor = Local260;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local332;
	PixelMaterialInputs.Metallic = Local337;
	PixelMaterialInputs.Specular = Local342;
	PixelMaterialInputs.Roughness = Local356;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local259;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = Local532;
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