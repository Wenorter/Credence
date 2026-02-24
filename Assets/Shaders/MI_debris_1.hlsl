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
uniform float4 SelectionColor;
uniform float Rotation_custom;
uniform float U_tile;
uniform float V_tile;
uniform float Tiling;
uniform float Base_UV___U_Tiling;
uniform float Base_UV___V_Tiling;
uniform float UTG_3_Normal_Strengh;
uniform float UVmask_R_opacity_bloor;
uniform float UVmask_R_opacity;
uniform float Grunge_tiling;
uniform float Grunge_channel_RGBA_power;
uniform float UVmask_G_opacity_bloor;
uniform float UVmask_G_opacity;
uniform float UVmask_B_opacity_bloor;
uniform float UVmask_B_opacity;
uniform float UVmask_A_opacity_bloor;
uniform float UVmask_A_opacity;
uniform float Rotation_custom_Mask_RGBA;
uniform float U_tile_Mask_RGBA;
uniform float V_tile_Mask_RGBA;
uniform float Tiling_Mask_RGBA;
uniform float Base_UV___U_Tiling_Mask_RGBA;
uniform float Base_UV___V_Tiling_Mask_RGBA;
uniform float Tiling_moss;
uniform float Normal_moss;
uniform float TopTint_normal;
uniform float BlendSharp_2;
uniform float BlendSharp_1;
uniform float Tiling_noise;
uniform float4 emissive_color;
uniform float emissive_power;
uniform float4 UTG_1_Tint_base_texture;
uniform float UTG_1_color_tone;
uniform float UTG_1_Brightness;
uniform float UTG_1_contrast;
uniform float4 tint_moss;
uniform float color_2_moss;
uniform float Brightness_moss;
uniform float color_1_moss;
uniform float Metallic_Contrast_main_layer;
uniform float Metallic_Intensity_main_layer;
uniform float metal_power_moss;
uniform float Specular_14;
uniform float UTG_2_Roughness_min;
uniform float UTG_2_Roughness_max;
uniform float Rough_min_moss;
uniform float Rough_max_moss;
uniform float UTG_4_Opacity_Mask_power;
uniform float UTG_1_SSS;
uniform float __SubsurfaceProfile;
uniform float UTG_2_AO_power;
uniform float AO_power_moss;

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
	float4 PreshaderBuffer[19];
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
	Material.PreshaderBuffer[2] = float4(1.000000,0.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[3] = float4(2.000000,-1.000000,0.000000,2.000000);//(Unknown)
	Material.PreshaderBuffer[4] = float4(-1.000000,0.000000,2.000000,-1.000000);//(Unknown)
	Material.PreshaderBuffer[5] = float4(0.000000,2.000000,-1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[6] = float4(1.000000,-0.000000,0.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[7] = float4(0.000000,0.000000,1.000000,1.000000);//(Unknown)
	Material.PreshaderBuffer[8] = float4(1.000000,1.000000,2.000000,2.665312);//(Unknown)
	Material.PreshaderBuffer[9] = float4(3.364401,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[10] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[11] = float4(0.000000,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[12] = float4(0.750000,-0.500000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[13] = float4(1.000000,1.000000,1.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[14] = float4(0.918018,0.000000,0.000000,0.000000);//(Unknown)
	Material.PreshaderBuffer[15] = float4(0.916667,0.875790,0.816406,1.000000);//(Unknown)
	Material.PreshaderBuffer[16] = float4(0.000000,1.000000,0.150000,1.000000);//(Unknown)
	Material.PreshaderBuffer[17] = float4(0.000000,1.000000,0.000000,0.500000);//(Unknown)
	Material.PreshaderBuffer[18] = float4(0.000000,0.000000,1.000000,1.000000);//(Unknown)

	Material.PreshaderBuffer[0].xy = Append(Cos( Rotation_custom * 6.283 ), Sin( Rotation_custom * 6.283 ) * -1);
	Material.PreshaderBuffer[0].zw = Append(Sin( Rotation_custom * 6.283 ), Cos( Rotation_custom * 6.283 ));
	Material.PreshaderBuffer[1].xy = Append(U_tile, V_tile);
	Material.PreshaderBuffer[1].zw = Tiling * Append(Base_UV___U_Tiling, Base_UV___V_Tiling);
	Material.PreshaderBuffer[2].x = UTG_3_Normal_Strengh;
	Material.PreshaderBuffer[2].y = UVmask_R_opacity;
	Material.PreshaderBuffer[2].z = Grunge_tiling;
	Material.PreshaderBuffer[2].w = Grunge_channel_RGBA_power;
	Material.PreshaderBuffer[3].x = UVmask_R_opacity_bloor + 1;
	Material.PreshaderBuffer[3].y = 0 - UVmask_R_opacity_bloor;
	Material.PreshaderBuffer[3].z = UVmask_G_opacity;
	Material.PreshaderBuffer[3].w = UVmask_G_opacity_bloor + 1;
	Material.PreshaderBuffer[4].x = 0 - UVmask_G_opacity_bloor;
	Material.PreshaderBuffer[4].y = UVmask_B_opacity;
	Material.PreshaderBuffer[4].z = UVmask_B_opacity_bloor + 1;
	Material.PreshaderBuffer[4].w = 0 - UVmask_B_opacity_bloor;
	Material.PreshaderBuffer[5].x = UVmask_A_opacity;
	Material.PreshaderBuffer[5].y = UVmask_A_opacity_bloor + 1;
	Material.PreshaderBuffer[5].z = 0 - UVmask_A_opacity_bloor;
	Material.PreshaderBuffer[6].xy = Append(Cos( Rotation_custom_Mask_RGBA * 6.283 ), Sin( Rotation_custom_Mask_RGBA * 6.283 ) * -1);
	Material.PreshaderBuffer[6].zw = Append(Sin( Rotation_custom_Mask_RGBA * 6.283 ), Cos( Rotation_custom_Mask_RGBA * 6.283 ));
	Material.PreshaderBuffer[7].xy = Append(U_tile_Mask_RGBA, V_tile_Mask_RGBA);
	Material.PreshaderBuffer[7].zw = Tiling_Mask_RGBA * Append(Base_UV___U_Tiling_Mask_RGBA, Base_UV___V_Tiling_Mask_RGBA);
	Material.PreshaderBuffer[8].x = Tiling_moss;
	Material.PreshaderBuffer[8].y = Normal_moss;
	Material.PreshaderBuffer[8].z = Tiling_noise;
	Material.PreshaderBuffer[8].w = BlendSharp_1;
	Material.PreshaderBuffer[9].x = BlendSharp_2;
	Material.PreshaderBuffer[9].y = TopTint_normal;
	Material.PreshaderBuffer[10].xyz = emissive_color.xyz * emissive_power;
	Material.PreshaderBuffer[10].w = SelectionColor.w;
	Material.PreshaderBuffer[11].xyz = SelectionColor.xyz;
	Material.PreshaderBuffer[11].w = UTG_1_color_tone * 6.283;
	Material.PreshaderBuffer[12].x = UTG_1_Brightness;
	Material.PreshaderBuffer[12].y = UTG_1_contrast;
	Material.PreshaderBuffer[13].xyz = UTG_1_Tint_base_texture.xyz;
	Material.PreshaderBuffer[13].w = color_2_moss * 6.283;
	Material.PreshaderBuffer[14].x = Brightness_moss;
	Material.PreshaderBuffer[14].y = color_1_moss;
	Material.PreshaderBuffer[15].xyz = tint_moss.xyz;
	Material.PreshaderBuffer[15].w = Metallic_Contrast_main_layer;
	Material.PreshaderBuffer[16].x = 1 - Metallic_Intensity_main_layer;
	Material.PreshaderBuffer[16].y = metal_power_moss;
	Material.PreshaderBuffer[16].z = Specular_14;
	Material.PreshaderBuffer[16].w = UTG_2_Roughness_max;
	Material.PreshaderBuffer[17].x = UTG_2_Roughness_min;
	Material.PreshaderBuffer[17].y = Rough_max_moss;
	Material.PreshaderBuffer[17].z = Rough_min_moss;
	Material.PreshaderBuffer[17].w = UTG_4_Opacity_Mask_power;
	Material.PreshaderBuffer[18].x = UTG_1_SSS;
	Material.PreshaderBuffer[18].y = __SubsurfaceProfile;
	Material.PreshaderBuffer[18].z = UTG_2_AO_power;
	Material.PreshaderBuffer[18].w = AO_power_moss;
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
	MaterialFloat Local17 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local0), 1);
	MaterialFloat4 Local18 = Texture2DSampleBias(Material_Texture2D_1,samplerMaterial_Texture2D_1,DERIV_BASE_VALUE(Local0),View.MaterialTextureMipBias);
	MaterialFloat Local19 = MaterialStoreTexSample(Parameters, Local18, 1);
	MaterialFloat Local20 = (Local18.r * Material.PreshaderBuffer[2].y);
	MaterialFloat2 Local21 = (((MaterialFloat2)Material.PreshaderBuffer[2].z) * DERIV_BASE_VALUE(Local0));
	MaterialFloat Local22 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local21), 4);
	MaterialFloat4 Local23 = ProcessMaterialColorTextureLookup(Texture2DSampleBias(Material_Texture2D_2,samplerMaterial_Texture2D_2,DERIV_BASE_VALUE(Local21),View.MaterialTextureMipBias));
	MaterialFloat Local24 = MaterialStoreTexSample(Parameters, Local23, 4);
	MaterialFloat3 Local25 = (Local23.rgb * ((MaterialFloat3)Material.PreshaderBuffer[2].w));
	MaterialFloat3 Local26 = (((MaterialFloat3)Local18.r) + Local25);
	MaterialFloat3 Local27 = (((MaterialFloat3)Local20) * Local26);
	MaterialFloat Local28 = lerp(Material.PreshaderBuffer[3].y,Material.PreshaderBuffer[3].x,Local27.x);
	MaterialFloat Local29 = saturate(Local28);
	MaterialFloat Local30 = saturate(Local29.r);
	MaterialFloat3 Local31 = lerp(MaterialFloat3(MaterialFloat2(Local15,Local16),Local13.b),((MaterialFloat3)1.00000000),Local30);
	MaterialFloat Local32 = (Local18.g * Material.PreshaderBuffer[3].z);
	MaterialFloat3 Local33 = (((MaterialFloat3)Local18.g) + Local25);
	MaterialFloat3 Local34 = (((MaterialFloat3)Local32) * Local33);
	MaterialFloat Local35 = lerp(Material.PreshaderBuffer[4].x,Material.PreshaderBuffer[3].w,Local34.x);
	MaterialFloat Local36 = saturate(Local35);
	MaterialFloat Local37 = saturate(Local36.r);
	MaterialFloat3 Local38 = lerp(Local31,((MaterialFloat3)1.00000000),Local37);
	MaterialFloat Local39 = (Local18.b * Material.PreshaderBuffer[4].y);
	MaterialFloat3 Local40 = (((MaterialFloat3)Local18.b) + Local25);
	MaterialFloat3 Local41 = (((MaterialFloat3)Local39) * Local40);
	MaterialFloat Local42 = lerp(Material.PreshaderBuffer[4].w,Material.PreshaderBuffer[4].z,Local41.x);
	MaterialFloat Local43 = saturate(Local42);
	MaterialFloat Local44 = saturate(Local43.r);
	MaterialFloat3 Local45 = lerp(Local38,((MaterialFloat3)1.00000000),Local44);
	MaterialFloat Local46 = (Local18.a * Material.PreshaderBuffer[5].x);
	MaterialFloat3 Local47 = (((MaterialFloat3)Local18.a) + Local25);
	MaterialFloat3 Local48 = (((MaterialFloat3)Local46) * Local47);
	MaterialFloat Local49 = lerp(Material.PreshaderBuffer[5].z,Material.PreshaderBuffer[5].y,Local48.x);
	MaterialFloat Local50 = saturate(Local49);
	MaterialFloat Local51 = saturate(Local50.r);
	MaterialFloat3 Local52 = lerp(Local45,((MaterialFloat3)1.00000000),Local51);
	MaterialFloat2 Local53 = Parameters.TexCoords[1].xy;
	MaterialFloat2 Local54 = (MaterialFloat2(-0.50000000,-0.50000000) + DERIV_BASE_VALUE(Local53));
	MaterialFloat Local55 = dot(DERIV_BASE_VALUE(Local54),Material.PreshaderBuffer[6].xy);
	MaterialFloat Local56 = dot(DERIV_BASE_VALUE(Local54),Material.PreshaderBuffer[6].zw);
	MaterialFloat2 Local57 = MaterialFloat2(DERIV_BASE_VALUE(Local55),DERIV_BASE_VALUE(Local56));
	MaterialFloat2 Local58 = (MaterialFloat2(0.50000000,0.50000000) + DERIV_BASE_VALUE(Local57));
	MaterialFloat2 Local59 = (DERIV_BASE_VALUE(Local58) + Material.PreshaderBuffer[7].xy);
	MaterialFloat2 Local60 = (DERIV_BASE_VALUE(Local59) * Material.PreshaderBuffer[7].zw);
	MaterialFloat2 Local61 = DDY((float2)DERIV_BASE_VALUE(Local60));
	MaterialFloat2 Local62 = DDX((float2)DERIV_BASE_VALUE(Local60));
	MaterialFloat2 Local63 = (DERIV_BASE_VALUE(Local60) * ((MaterialFloat2)Material.PreshaderBuffer[8].x));
	MaterialFloat2 Local64 = (DERIV_BASE_VALUE(Local62) * ((MaterialFloat2)View.MaterialTextureDerivativeMultiply));
	MaterialFloat2 Local65 = (DERIV_BASE_VALUE(Local61) * ((MaterialFloat2)View.MaterialTextureDerivativeMultiply));
	MaterialFloat Local66 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local63), 2);
	MaterialFloat4 Local67 = UnpackNormalMap(Texture2DSampleGrad(Material_Texture2D_3,samplerMaterial_Texture2D_3,DERIV_BASE_VALUE(Local63),DERIV_BASE_VALUE(Local64),DERIV_BASE_VALUE(Local65)));
	MaterialFloat Local68 = MaterialStoreTexSample(Parameters, Local67, 2);
	MaterialFloat Local69 = (Local67.r * Material.PreshaderBuffer[8].y);
	MaterialFloat Local70 = (Local67.g * Material.PreshaderBuffer[8].y);
	MaterialFloat3 Local71 = mul(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb, Parameters.TangentToWorld);
	MaterialFloat Local72 = dot(Local71,MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb);
	MaterialFloat3 Local73 = normalize(Local71);
	MaterialFloat Local74 = dot(DERIV_BASE_VALUE(Local73),normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb));
	MaterialFloat Local75 = (DERIV_BASE_VALUE(Local74) * 0.50000000);
	MaterialFloat Local76 = (DERIV_BASE_VALUE(Local75) + 0.50000000);
	MaterialFloat2 Local77 = (DERIV_BASE_VALUE(Local53) * ((MaterialFloat2)Material.PreshaderBuffer[8].z));
	MaterialFloat Local78 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local77), 3);
	MaterialFloat4 Local79 = ProcessMaterialColorTextureLookup(Texture2DSampleBias(Material_Texture2D_4,samplerMaterial_Texture2D_4,DERIV_BASE_VALUE(Local77),View.MaterialTextureMipBias));
	MaterialFloat Local80 = MaterialStoreTexSample(Parameters, Local79, 3);
	MaterialFloat Local81 = lerp(Material.PreshaderBuffer[9].x,Material.PreshaderBuffer[8].w,Local79.rgb.r);
	MaterialFloat Local82 = (DERIV_BASE_VALUE(Local76) * Local81);
	MaterialFloat Local83 = (Local81 * 0.50000000);
	MaterialFloat Local84 = (-1.00000000 - Local83);
	MaterialFloat Local85 = (Local82 + Local84);
	MaterialFloat Local86 = saturate(Local85);
	MaterialFloat Local87 = lerp(0.00000000,Material.PreshaderBuffer[9].y,Local86);
	MaterialFloat Local88 = (Local72 * Local87);
	MaterialFloat Local89 = saturate(Local88);
	MaterialFloat3 Local90 = lerp(Local52,MaterialFloat3(MaterialFloat2(Local69,Local70),Local67.b),Local89);

	// The Normal is a special case as it might have its own expressions and also be used to calculate other inputs, so perform the assignment here
	PixelMaterialInputs.Normal = Local90;

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
	MaterialFloat3 Local91 = lerp(Material.PreshaderBuffer[10].xyz,Material.PreshaderBuffer[11].xyz,Material.PreshaderBuffer[10].w);
	MaterialFloat Local92 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 0);
	MaterialFloat4 Local93 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_5,GetMaterialSharedSampler(samplerMaterial_Texture2D_5,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7)));
	MaterialFloat Local94 = MaterialStoreTexSample(Parameters, Local93, 0);
	MaterialFloat3 Local95 = (Local93.rgb * ((MaterialFloat3)Material.PreshaderBuffer[12].x));
	MaterialFloat Local96 = dot(Local95,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local97 = lerp(Local95,((MaterialFloat3)Local96),Material.PreshaderBuffer[12].y);
	MaterialFloat3 Local98 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[11].w),((MaterialFloat3)0.00000000),Local97);
	MaterialFloat3 Local99 = (Local98 + Local97);
	MaterialFloat3 Local100 = (Material.PreshaderBuffer[13].xyz * Local99);
	MaterialFloat3 Local101 = lerp(Local100,((MaterialFloat3)1.00000000),Local30);
	MaterialFloat3 Local102 = lerp(Local101,((MaterialFloat3)1.00000000),Local37);
	MaterialFloat3 Local103 = lerp(Local102,((MaterialFloat3)1.00000000),Local44);
	MaterialFloat3 Local104 = lerp(Local103,((MaterialFloat3)1.00000000),Local51);
	MaterialFloat Local105 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local63), 0);
	MaterialFloat4 Local106 = ProcessMaterialColorTextureLookup(Texture2DSample(Material_Texture2D_6,GetMaterialSharedSampler(samplerMaterial_Texture2D_6,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local63)));
	MaterialFloat Local107 = MaterialStoreTexSample(Parameters, Local106, 0);
	MaterialFloat3 Local108 = (Local106.rgb * ((MaterialFloat3)Material.PreshaderBuffer[14].x));
	MaterialFloat Local109 = dot(Local108,MaterialFloat3(0.21263900,0.71516865,0.07219232));
	MaterialFloat3 Local110 = lerp(Local108,((MaterialFloat3)Local109),Material.PreshaderBuffer[14].y);
	MaterialFloat3 Local111 = RotateAboutAxis(MaterialFloat4(normalize(MaterialFloat3(1.00000000,1.00000000,1.00000000).rgb),Material.PreshaderBuffer[13].w),((MaterialFloat3)0.00000000),Local110);
	MaterialFloat3 Local112 = (Local111 + Local110);
	MaterialFloat3 Local113 = (Material.PreshaderBuffer[15].xyz * Local112);
	MaterialFloat Local114 = dot(WorldNormalCopy,normalize(MaterialFloat3(0.00000000,0.00000000,1.00000000).rgb));
	MaterialFloat Local115 = (Local114 * 0.50000000);
	MaterialFloat Local116 = (Local115 + 0.50000000);
	MaterialFloat Local117 = (Local116 * Local81);
	MaterialFloat Local118 = (Local117 + Local84);
	MaterialFloat Local119 = saturate(Local118);
	MaterialFloat3 Local120 = lerp(Local104,Local113,Local119);
	MaterialFloat Local121 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local7), 1);
	MaterialFloat4 Local122 = Texture2DSample(Material_Texture2D_7,GetMaterialSharedSampler(samplerMaterial_Texture2D_7,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local7));
	MaterialFloat Local123 = MaterialStoreTexSample(Parameters, Local122, 1);
	MaterialFloat Local124 = PositiveClampedPow(Local122.b,Material.PreshaderBuffer[15].w);
	MaterialFloat Local125 = (Local124 + Material.PreshaderBuffer[16].x);
	MaterialFloat Local126 = lerp(Local125,1.00000000,Local30);
	MaterialFloat Local127 = lerp(Local126,1.00000000,Local37);
	MaterialFloat Local128 = lerp(Local127,1.00000000,Local44);
	MaterialFloat Local129 = lerp(Local128,1.00000000,Local51);
	MaterialFloat Local130 = MaterialStoreTexCoordScale(Parameters, DERIV_BASE_VALUE(Local63), 1);
	MaterialFloat4 Local131 = Texture2DSample(Material_Texture2D_8,GetMaterialSharedSampler(samplerMaterial_Texture2D_8,View_MaterialTextureBilinearWrapedSampler),DERIV_BASE_VALUE(Local63));
	MaterialFloat Local132 = MaterialStoreTexSample(Parameters, Local131, 1);
	MaterialFloat Local133 = PositiveClampedPow(Local131.b,Material.PreshaderBuffer[16].y);
	MaterialFloat Local134 = lerp(Local129,Local133,Local119);
	MaterialFloat Local135 = lerp(Material.PreshaderBuffer[17].x,Material.PreshaderBuffer[16].w,Local122.g);
	MaterialFloat Local136 = lerp(Local135,1.00000000,Local30);
	MaterialFloat Local137 = lerp(Local136,1.00000000,Local37);
	MaterialFloat Local138 = lerp(Local137,1.00000000,Local44);
	MaterialFloat Local139 = lerp(Local138,1.00000000,Local51);
	MaterialFloat Local140 = lerp(Material.PreshaderBuffer[17].z,Material.PreshaderBuffer[17].y,Local131.g);
	MaterialFloat Local141 = lerp(Local139,Local140,Local119);
	MaterialFloat Local142 = lerp(Local93.a,1.00000000,0.50000000);
	MaterialFloat Local143 = (Local142 * Material.PreshaderBuffer[17].w);
	MaterialFloat3 Local144 = (Local120 * ((MaterialFloat3)Material.PreshaderBuffer[18].x));
	MaterialFloat Local145 = PositiveClampedPow(Local122.r,Material.PreshaderBuffer[18].z);
	MaterialFloat Local146 = lerp(Local145,1.00000000,Local30);
	MaterialFloat Local147 = lerp(Local146,1.00000000,Local37);
	MaterialFloat Local148 = lerp(Local147,1.00000000,Local44);
	MaterialFloat Local149 = lerp(Local148,1.00000000,Local51);
	MaterialFloat Local150 = PositiveClampedPow(Local131.r,Material.PreshaderBuffer[18].w);
	MaterialFloat Local151 = lerp(Local149,Local150,Local119);

	PixelMaterialInputs.EmissiveColor = Local91;
	PixelMaterialInputs.Opacity = 1.00000000;
	PixelMaterialInputs.OpacityMask = Local143;
	PixelMaterialInputs.BaseColor = Local120;
	PixelMaterialInputs.Metallic = Local134;
	PixelMaterialInputs.Specular = Material.PreshaderBuffer[16].z;
	PixelMaterialInputs.Roughness = Local141;
	PixelMaterialInputs.Anisotropy = 0.00000000;
	PixelMaterialInputs.Normal = Local90;
	PixelMaterialInputs.Tangent = MaterialFloat3(1.00000000,0.00000000,0.00000000);
	PixelMaterialInputs.Subsurface = MaterialFloat4(Local144,Material.PreshaderBuffer[18].y);
	PixelMaterialInputs.AmbientOcclusion = Local151;
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