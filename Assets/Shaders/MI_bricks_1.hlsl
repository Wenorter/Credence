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
uniform float R_channel_blend;
uniform float R_channel_extend;
uniform float UVmask_R_opacity_bloor;
uniform float UVmask_R_opacity;
uniform float Grunge_tiling_R_channel;
uniform float Grunge_channel_R_power;
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
uniform float color_tone_main_layer;
uniform float Brightness_main_layer;
uniform float contrast_main_layer;
uniform float4 Tint_base_color_main_layer;
uniform float color_tone_R_channel;
uniform float Brightness_R_channel;
uniform float contrast_R_channel;
uniform float4 Tint_base_color_R_channel;
uniform float color_tone_TopTint_layer;
uniform float Brightness_TopTint_layer;
uniform float contrast_TopTint_layer;
uniform float4 Tint_base_color_TopTint_layer;
uniform float Metallic_main_layer;
uniform float Metallic_R_channel;
uniform float Metallic_TopTint_layer;
uniform float Specular_main_layer;
uniform float Specular_R_channel;
uniform float Specular_TopTint_layer;
uniform float Roughness_min_main_layer;
uniform float Roughness_max_main_layer;
uniform float Roughness_min_R_channel;
uniform float Roughness_max_R_channel;
uniform float Roughness_min_TopTint_layer;
uniform float Roughness_max_TopTint_layer;
uniform float AO_Contrast_main_layer;
uniform float AO_Intensity_main_layer;
uniform float AO_Contrast_R_channel;
uniform float AO_Intensity_R_channel;
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
	float4 PreshaderBuffer[24];
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
	Material.PreshaderBuffer[1] = float4(0.000000,0.000000,1.500000,1.500000);//(Unknown)
	Material.PreshaderBuffer[2] = float4(1.000000,1.000000,-0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(0.000000,1.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[4] = float4(1.000000,1.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[5] = float4(0.200000,80.000000,2.000000,-1.000000);//(Unknown)
	Material.PreshaderBuffer[6] = float4(-0.700000,0.957333,0.042667,0.000000);//(Unknown)
	Material.PreshaderBuffer[7] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[8] = float4(0.000000,0.000000,4.500000,4.500000);//(Unknown)
	Material.PreshaderBuffer[9] = float4(1.000000,1.000000,0.000000,0.050000);//(Unknown)
	Material.PreshaderBuffer[10] = float4(1.000000,0.762994,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[11] = float4(4.708986,2.326780,1.009699,-0.009699);//(Unknown)
	Material.PreshaderBuffer[12] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[13] = float4(0.000000,0.963113,0.078000,0.000000);//(Unknown)
	Material.PreshaderBuffer[14] = float4(1.000000,0.973828,0.976319,0.000000);//(Unknown)
	Material.PreshaderBuffer[15] = float4(0.552000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[16] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[17] = float4(0.612000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[18] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[19] = float4(0.000000,0.000000,0.150000,0.150000);//(Unknown)
	Material.PreshaderBuffer[20] = float4(0.150000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[21] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[22] = float4(0.000000,1.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[23] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)

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
	Material.PreshaderBuffer[4].w = UVmask_R_opacity;
	Material.PreshaderBuffer[5].x = Grunge_tiling_R_channel;
	Material.PreshaderBuffer[5].y = Grunge_channel_R_power;
	Material.PreshaderBuffer[5].z = UVmask_R_opacity_bloor + 1;
	Material.PreshaderBuffer[5].w = 0 - UVmask_R_opacity_bloor;
	Material.PreshaderBuffer[6].x = R_channel_extend - 1;
	Material.PreshaderBuffer[6].y = R_channel_contrast + 1;
	Material.PreshaderBuffer[6].z = 0 - R_channel_contrast;
	Material.PreshaderBuffer[7].xy = Append(Cos( Rotation_custom_TopTint_layer * 6.283 ), Sin( Rotation_custom_TopTint_layer * 6.283 ) * -1);
	Material.PreshaderBuffer[7].zw = Append(Sin( Rotation_custom_TopTint_layer * 6.283 ), Cos( Rotation_custom_TopTint_layer * 6.283 ));
	Material.PreshaderBuffer[8].xy = Append(U_tile_TopTint_layer, V_tile_TopTint_layer);
	Material.PreshaderBuffer[8].zw = UV_Tiling_TopTint_layer * Append(Base_UV___U_Tiling_TopTint_layer, Base_UV___V_Tiling_TopTint_layer);
	Material.PreshaderBuffer[9].x = Normal_intensity_TopTint_layer;
	Material.PreshaderBuffer[9].y = Disp_Contrast_TopTint_layer + 1;
	Material.PreshaderBuffer[9].z = 0 - Disp_Contrast_TopTint_layer;
	Material.PreshaderBuffer[9].w = Disp_Intensity_TopTint_layer;
	Material.PreshaderBuffer[10].x = TopTint_layer_blend;
	Material.PreshaderBuffer[10].y = TopTint_layer_extend;
	Material.PreshaderBuffer[10].z = Tiling_noise;
	Material.PreshaderBuffer[10].w = noise_power;
	Material.PreshaderBuffer[11].x = BlendSharp_1;
	Material.PreshaderBuffer[11].y = BlendSharp_2;
	Material.PreshaderBuffer[11].z = TopTint_layer_contrast + 1;
	Material.PreshaderBuffer[11].w = 0 - TopTint_layer_contrast;
	Material.PreshaderBuffer[12].x = SelectionColor.w;
	Material.PreshaderBuffer[12].yzw = SelectionColor.xyz;
	Material.PreshaderBuffer[13].x = color_tone_main_layer * 6.283;
	Material.PreshaderBuffer[13].y = Brightness_main_layer;
	Material.PreshaderBuffer[13].z = contrast_main_layer;
	Material.PreshaderBuffer[14].xyz = Tint_base_color_main_layer.xyz;
	Material.PreshaderBuffer[14].w = color_tone_R_channel * 6.283;
	Material.PreshaderBuffer[15].x = Brightness_R_channel;
	Material.PreshaderBuffer[15].y = contrast_R_channel;
	Material.PreshaderBuffer[16].xyz = Tint_base_color_R_channel.xyz;
	Material.PreshaderBuffer[16].w = color_tone_TopTint_layer * 6.283;
	Material.PreshaderBuffer[17].x = Brightness_TopTint_layer;
	Material.PreshaderBuffer[17].y = contrast_TopTint_layer;
	Material.PreshaderBuffer[18].xyz = Tint_base_color_TopTint_layer.xyz;
	Material.PreshaderBuffer[18].w = Metallic_main_layer;
	Material.PreshaderBuffer[19].x = Metallic_R_channel;
	Material.PreshaderBuffer[19].y = Metallic_TopTint_layer;
	Material.PreshaderBuffer[19].z = Specular_main_layer;
	Material.PreshaderBuffer[19].w = Specular_R_channel;
	Material.PreshaderBuffer[20].x = Specular_TopTint_layer;
	Material.PreshaderBuffer[20].y = Roughness_max_main_layer;
	Material.PreshaderBuffer[20].z = Roughness_min_main_layer;
	Material.PreshaderBuffer[20].w = Roughness_max_R_channel;
	Material.PreshaderBuffer[21].x = Roughness_min_R_channel;
	Material.PreshaderBuffer[21].y = Roughness_max_TopTint_layer;
	Material.PreshaderBuffer[21].z = Roughness_min_TopTint_layer;
	Material.PreshaderBuffer[21].w = AO_Contrast_main_layer;
	Material.PreshaderBuffer[22].x = 1 - AO_Intensity_main_layer;
	Material.PreshaderBuffer[22].y = AO_Contrast_R_channel;
	Material.PreshaderBuffer[22].z = 1 - AO_Intensity_R_channel;
	Material.PreshaderBuffer[22].w = AO_Contrast_TopTint_layer;
	Material.PreshaderBuffer[23].x = 1 - AO_Intensity_TopTint_layer;
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
	MaterialFloat Local24 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local0), 8);
	MaterialFloat4 Local25 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_2,GetMaterialSharedSampler(samplerMaterial_Texture2D_2,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local0)));
	MaterialFloat Local26 = MaterialStoreTexSample(Parameters, Local25, 8);
	MaterialFloat Local27 = (Local25.r * Material.PreshaderBuffer[4].w);
	MaterialFloat2 Local28 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[5].x));
	MaterialFloat Local29 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local28), 9);
	MaterialFloat4 Local30 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_3,GetMaterialSharedSampler(samplerMaterial_Texture2D_3,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local28)));
	MaterialFloat Local31 = MaterialStoreTexSample(Parameters, Local30, 9);
	MaterialFloat3 Local32 = (Local30.rgb * ((MaterialFloat3)Material.PreshaderBuffer[5].y));
	MaterialFloat3 Local33 = (((MaterialFloat3)Local25.r) + Local32);
	MaterialFloat3 Local34 = (((MaterialFloat3)Local27) * Local33);
	MaterialFloat Local35 = lerp(Material.PreshaderBuffer[5].w,Material.PreshaderBuffer[5].z,Local34.x);
	MaterialFloat Local36 = saturate(Local35);
	MaterialFloat Local37 = saturate(Local36.r);
	MaterialFloat Local38 = (Local37 * 2.00000000);
	MaterialFloat Local39 = (Material.PreshaderBuffer[6].x + Local38);
	MaterialFloat Local40 = saturate(Local39);
	MaterialFloat Local41 = lerp(Material.PreshaderBuffer[6].z,Material.PreshaderBuffer[6].y,Local40);
	MaterialFloat Local42 = saturate(Local41);
	MaterialFloat3 Local43 = lerp(MaterialFloat3(MaterialFloat2(Local11,Local12),Local9.rgb.b),MaterialFloat3(MaterialFloat2(Local22,Local23),Local20.rgb.b),Local42.r);
	MaterialFloat Local44 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[7].xy);
	MaterialFloat Local45 = dot(DERIV_BASE_VALUE(Local1),Material.PreshaderBuffer[7].zw);
	MaterialFloat2 Local46 = MaterialFloat2(DERIV_BASE_VALUE(Local44),DERIV_BASE_VALUE(Local45));
	MaterialFloat2 Local47 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local46));
	MaterialFloat2 Local48 = (DERIV_BASE_VALUE(Local47) + Material.PreshaderBuffer[8].xy);
	MaterialFloat2 Local49 = (DERIV_BASE_VALUE(Local48) * Material.PreshaderBuffer[8].zw);
	MaterialFloat Local50 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local49), 5);
	MaterialFloat4 Local51 = UnpackNormalMap(Texture2DSample(Material_Texture2D_4,GetMaterialSharedSampler(samplerMaterial_Texture2D_4,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local49)));
	MaterialFloat Local52 = MaterialStoreTexSample(Parameters, Local51, 5);
	MaterialFloat Local53 = (Local51.rgb.r * Material.PreshaderBuffer[9].x);
	MaterialFloat Local54 = (Local51.rgb.g * Material.PreshaderBuffer[9].x);
	MaterialFloat Local55 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local49), 7);
	MaterialFloat4 Local56 = Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local49));
	MaterialFloat Local57 = MaterialStoreTexSample(Parameters, Local56, 7);
	MaterialFloat Local58 = lerp(Material.PreshaderBuffer[9].z,Material.PreshaderBuffer[9].y,Local56.b);
	MaterialFloat Local59 = saturate(Local58);
	MaterialFloat Local60 = (Local59.r * Material.PreshaderBuffer[9].w);
	MaterialFloat Local61 = lerp(0.00000000,Local60,Material.PreshaderBuffer[10].x);
	MaterialFloat Local62 = (1.00000000 - Local61);
	MaterialFloat Local63 = (Local62 * Material.PreshaderBuffer[10].y);
	MaterialFloat Local64 = (Local63 - 1.00000000);
	MaterialFloat3 Local65 = Parameters.TangentToWorld[2];
	MaterialFloat Local66 = dot(DERIV_BASE_VALUE(Local65),normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb));
	MaterialFloat Local67 = (DERIV_BASE_VALUE(Local66) * 0.50000000);
	MaterialFloat Local68 = (DERIV_BASE_VALUE(Local67) + 0.50000000);
	MaterialFloat2 Local69 = (DERIV_BASE_VALUE(Local0) * ((MaterialFloat2)Material.PreshaderBuffer[10].z));
	MaterialFloat Local70 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local69), 8);
	MaterialFloat4 Local71 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_6,GetMaterialSharedSampler(samplerMaterial_Texture2D_6,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local69)));
	MaterialFloat Local72 = MaterialStoreTexSample(Parameters, Local71, 8);
	MaterialFloat3 Local73 = (Local71.rgb * ((MaterialFloat3)Material.PreshaderBuffer[10].w));
	MaterialFloat Local74 = lerp(Material.PreshaderBuffer[11].y,Material.PreshaderBuffer[11].x,Local73.r);
	MaterialFloat Local75 = (DERIV_BASE_VALUE(Local68) * Local74);
	MaterialFloat Local76 = (Local74 * 0.50000000);
	MaterialFloat Local77 = (-1.00000000 - Local76);
	MaterialFloat Local78 = (Local75 + Local77);
	MaterialFloat Local79 = saturate(Local78);
	MaterialFloat Local80 = (Local79 * 2.00000000);
	MaterialFloat Local81 = (Local64 + Local80);
	MaterialFloat Local82 = saturate(Local81);
	MaterialFloat Local83 = lerp(Material.PreshaderBuffer[11].w,Material.PreshaderBuffer[11].z,Local82);
	MaterialFloat Local84 = saturate(Local83);
	MaterialFloat3 Local85 = lerp(Local43,MaterialFloat3(MaterialFloat2(Local53,Local54),Local51.rgb.b),Local84.r);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local85;

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
	MaterialFloat3 Local86 = lerp(MaterialFloat3(0.00000000,0.00000000,0.00000000),Material.PreshaderBuffer[12].yzw,Material.PreshaderBuffer[12].x);
	MaterialFloat Local87 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 6);
	MaterialFloat4 Local88 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local89 = MaterialStoreTexSample(Parameters, Local88, 6);
	MaterialFloat3 Local90 = (Local88.rgb * ((MaterialFloat3)Material.PreshaderBuffer[13].y));
	MaterialFloat Local91 = dot(Local90,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local92 = lerp(Local90,((MaterialFloat3)Local91),Material.PreshaderBuffer[13].z);
	MaterialFloat3 Local93 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[13].x),((MaterialFloat3)0.00000000),Local92);
	MaterialFloat3 Local94 = (Local93 + Local92);
	MaterialFloat3 Local95 = (Local94 * Material.PreshaderBuffer[14].xyz);
	MaterialFloat Local96 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local18), 6);
	MaterialFloat4 Local97 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_8,GetMaterialSharedSampler(samplerMaterial_Texture2D_8,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local18)));
	MaterialFloat Local98 = MaterialStoreTexSample(Parameters, Local97, 6);
	MaterialFloat3 Local99 = (Local97.rgb * ((MaterialFloat3)Material.PreshaderBuffer[15].x));
	MaterialFloat Local100 = dot(Local99,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local101 = lerp(Local99,((MaterialFloat3)Local100),Material.PreshaderBuffer[15].y);
	MaterialFloat3 Local102 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[14].w),((MaterialFloat3)0.00000000),Local101);
	MaterialFloat3 Local103 = (Local102 + Local101);
	MaterialFloat3 Local104 = (Local103 * Material.PreshaderBuffer[16].xyz);
	MaterialFloat3 Local105 = lerp(Local95,Local104,Local42.r);
	MaterialFloat Local106 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local49), 6);
	MaterialFloat4 Local107 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_9,GetMaterialSharedSampler(samplerMaterial_Texture2D_9,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local49)));
	MaterialFloat Local108 = MaterialStoreTexSample(Parameters, Local107, 6);
	MaterialFloat3 Local109 = (Local107.rgb * ((MaterialFloat3)Material.PreshaderBuffer[17].x));
	MaterialFloat Local110 = dot(Local109,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local111 = lerp(Local109,((MaterialFloat3)Local110),Material.PreshaderBuffer[17].y);
	MaterialFloat3 Local112 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[16].w),((MaterialFloat3)0.00000000),Local111);
	MaterialFloat3 Local113 = (Local112 + Local111);
	MaterialFloat3 Local114 = (Local113 * Material.PreshaderBuffer[18].xyz);
	MaterialFloat3 Local115 = lerp(Local105,Local114,Local84.r);
	MaterialFloat Local116 = lerp(Material.PreshaderBuffer[18].w,Material.PreshaderBuffer[19].x,Local42.r);
	MaterialFloat Local117 = lerp(Local116,Material.PreshaderBuffer[19].y,Local84.r);
	MaterialFloat Local118 = lerp(Material.PreshaderBuffer[19].z,Material.PreshaderBuffer[19].w,Local42.r);
	MaterialFloat Local119 = lerp(Local118,Material.PreshaderBuffer[20].x,Local84.r);
	MaterialFloat Local120 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 7);
	MaterialFloat4 Local121 = Texture2DSample(Material_Texture2D_10,GetMaterialSharedSampler(samplerMaterial_Texture2D_10,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7));
	MaterialFloat Local122 = MaterialStoreTexSample(Parameters, Local121, 7);
	MaterialFloat Local123 = lerp(Material.PreshaderBuffer[20].z,Material.PreshaderBuffer[20].y,Local121.g);
	MaterialFloat Local124 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local18), 7);
	MaterialFloat4 Local125 = Texture2DSample(Material_Texture2D_11,GetMaterialSharedSampler(samplerMaterial_Texture2D_11,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local18));
	MaterialFloat Local126 = MaterialStoreTexSample(Parameters, Local125, 7);
	MaterialFloat Local127 = lerp(Material.PreshaderBuffer[21].x,Material.PreshaderBuffer[20].w,Local125.g);
	MaterialFloat Local128 = lerp(Local123,Local127,Local42.r);
	MaterialFloat Local129 = lerp(Material.PreshaderBuffer[21].z,Material.PreshaderBuffer[21].y,Local56.g);
	MaterialFloat Local130 = lerp(Local128,Local129,Local84.r);
	MaterialFloat Local179 = PositiveClampedPow(Local121.r,Material.PreshaderBuffer[21].w);
	MaterialFloat Local180 = (Local179 + Material.PreshaderBuffer[22].x);
	MaterialFloat Local181 = PositiveClampedPow(Local125.r,Material.PreshaderBuffer[22].y);
	MaterialFloat Local182 = (Local181 + Material.PreshaderBuffer[22].z);
	MaterialFloat Local183 = lerp(Local180,Local182,Local42.r);
	MaterialFloat Local184 = PositiveClampedPow(Local56.r,Material.PreshaderBuffer[22].w);
	MaterialFloat Local185 = (Local184 + Material.PreshaderBuffer[23].x);
	MaterialFloat Local186 = lerp(Local183,Local185,Local84.r);

	PixelMaterialInputs.EmissiveColor = Local86;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = 1.00000000;
	PixelMaterialInputs.BaseColor = Local115;
	PixelMaterialInputs.Metallic = Local117;
	PixelMaterialInputs.Specular = Local119;
	PixelMaterialInputs.Roughness = Local130;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local85;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = 0;
	PixelMaterialInputs.AmbientOcclusion = Local186;
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