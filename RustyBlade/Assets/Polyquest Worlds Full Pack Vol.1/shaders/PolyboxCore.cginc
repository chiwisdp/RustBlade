#ifndef POLYBOX_CORE_INCLUDED
#define POLYBOX_CORE_INCLUDED

#include "UnityCG.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityInstancing.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityStandardInput.cginc"
#include "UnityPBSLighting.cginc"
#include "UnityStandardUtils.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardBRDF.cginc"

#include "AutoLight.cginc"

//Polybox Properties

half _HeightFogStart;
half _HeightFogEnd;
half4 _HeightFogColor;

//-------------------------------------------------------------------------------------
// counterpart for NormalizePerPixelNormal
// skips normalization per-vertex and expects normalization to happen per-pixel
half3 NormalizePerVertexNormal (float3 n) // takes float to avoid overflow
{
    #if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
        return normalize(n);
    #else
        return n; // will normalize per-pixel instead
    #endif
}

half3 NormalizePerPixelNormal (half3 n)
{
	#if (SHADER_TARGET < 30) || UNITY_STANDARD_SIMPLE
		return n;
	#else
		return normalize(n);
	#endif
}

//-------------------------------------------------------------------------------------
UnityLight MainLight ()
{
	UnityLight l;
	#if !defined(LIGHTMAP_ON)
		l.color = _LightColor0.rgb;
		l.dir = _WorldSpaceLightPos0.xyz;
		l.ndotl = 0; // Not used
	#else
		// no light specified by the engine
		// analytical light might be extracted from Lightmap data later on in the shader depending on the Lightmap type
		// Should be ResetUnityLight() call but can't move the function to a UnityLightingCommon.cginc header because it break UBER shader on asset store...
		l.color = half3(0, 0, 0);
		l.dir = half3(0, 1, 0); // Irrelevant direction, just not null
		l.ndotl = 0; // Not used
	#endif

	return l;
}

UnityLight AdditiveLight (half3 lightDir, half atten)
{
	UnityLight l;

	l.color = _LightColor0.rgb;
	l.dir = lightDir;
	#ifndef USING_DIRECTIONAL_LIGHT
		l.dir = NormalizePerPixelNormal(l.dir);
	#endif

	// shadow the light
	l.color *= atten;
	return l;
}

UnityLight DummyLight ()
{
	UnityLight l;
	l.color = 0;
	l.dir = half3 (0,1,0);
	return l;
}

UnityIndirect ZeroIndirect ()
{
	UnityIndirect ind;
	ind.diffuse = 0;
	ind.specular = 0;
	return ind;
}

//-------------------------------------------------------------------------------------
// Common fragment setup

// deprecated
half3 WorldNormal(half4 tan2world[3])
{
	return normalize(tan2world[2].xyz);
}

// deprecated
#ifdef _TANGENT_TO_WORLD
	half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
	{
		half3 t = tan2world[0].xyz;
		half3 b = tan2world[1].xyz;
		half3 n = tan2world[2].xyz;

	#if UNITY_TANGENT_ORTHONORMALIZE
		n = NormalizePerPixelNormal(n);

		// ortho-normalize Tangent
		t = normalize (t - n * dot(t, n));

		// recalculate Binormal
		half3 newB = cross(n, t);
		b = newB * sign (dot (newB, b));
	#endif

		return half3x3(t, b, n);
	}
#else
	half3x3 ExtractTangentToWorldPerPixel(half4 tan2world[3])
	{
		return half3x3(0,0,0,0,0,0,0,0,0);
	}
#endif

half3 PerPixelWorldNormal(float4 i_tex, half4 tangentToWorld[3])
{
#ifdef _NORMALMAP
	half3 tangent = tangentToWorld[0].xyz;
	half3 binormal = tangentToWorld[1].xyz;
	half3 normal = tangentToWorld[2].xyz;

	#if UNITY_TANGENT_ORTHONORMALIZE
		normal = NormalizePerPixelNormal(normal);

		// ortho-normalize Tangent
		tangent = normalize (tangent - normal * dot(tangent, normal));

		// recalculate Binormal
		half3 newB = cross(normal, tangent);
		binormal = newB * sign (dot (newB, binormal));
	#endif

	half3 normalTangent = NormalInTangentSpace(i_tex);
	half3 normalWorld = NormalizePerPixelNormal(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z); // @TODO: see if we can squeeze this normalize on SM2.0 as well
#else
	half3 normalWorld = normalize(tangentToWorld[2].xyz);
#endif
	return normalWorld;
}

#ifdef _PARALLAXMAP
	#define IN_VIEWDIR4PARALLAX(i) NormalizePerPixelNormal(half3(i.tangentToWorldAndParallax[0].w,i.tangentToWorldAndParallax[1].w,i.tangentToWorldAndParallax[2].w))
	#define IN_VIEWDIR4PARALLAX_FWDADD(i) NormalizePerPixelNormal(i.viewDirForParallax.xyz)
#else
	#define IN_VIEWDIR4PARALLAX(i) half3(0,0,0)
	#define IN_VIEWDIR4PARALLAX_FWDADD(i) half3(0,0,0)
#endif

#if UNITY_REQUIRE_FRAG_WORLDPOS
	#define IN_WORLDPOS(i) i.posWorld
#else
	#define IN_WORLDPOS(i) half3(0,0,0)
#endif

#define IN_LIGHTDIR_FWDADD(i) half3(i.tangentToWorldAndLightDir[0].w, i.tangentToWorldAndLightDir[1].w, i.tangentToWorldAndLightDir[2].w)

#define FRAGMENT_SETUP(x) FragmentCommonData x = \
	FragmentSetup(i.tex, i.eyeVec, IN_VIEWDIR4PARALLAX(i), i.tangentToWorldAndParallax, IN_WORLDPOS(i));

#define FRAGMENT_SETUP_FWDADD(x) FragmentCommonData x = \
	FragmentSetup(i.tex, i.eyeVec, IN_VIEWDIR4PARALLAX_FWDADD(i), i.tangentToWorldAndLightDir, half3(0,0,0));

struct FragmentCommonData
{
	half3 diffColor, specColor;
	// Note: smoothness & oneMinusReflectivity for optimization purposes, mostly for DX9 SM2.0 level.
	// Most of the math is being done on these (1-x) values, and that saves a few precious ALU slots.
	half oneMinusReflectivity, smoothness;
	half3 normalWorld, eyeVec, posWorld;
	half alpha;

#if UNITY_OPTIMIZE_TEXCUBELOD || UNITY_STANDARD_SIMPLE
	half3 reflUVW;
#endif

#if UNITY_STANDARD_SIMPLE
	half3 tangentSpaceNormal;
#endif
};

#ifndef UNITY_SETUP_BRDF_INPUT
	#define UNITY_SETUP_BRDF_INPUT SpecularSetup
#endif

inline FragmentCommonData SpecularSetup (float4 i_tex)
{
	half4 specGloss = SpecularGloss(i_tex.xy);
	half3 specColor = specGloss.rgb;
	half smoothness = specGloss.a;

	half oneMinusReflectivity;
	half3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (Albedo(i_tex), specColor, /*out*/ oneMinusReflectivity);
	
	FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.smoothness = smoothness;
	return o;
}

inline FragmentCommonData MetallicSetup (float4 i_tex)
{
	half2 metallicGloss = MetallicGloss(i_tex.xy);
	half metallic = metallicGloss.x;
	half smoothness = metallicGloss.y; // this is 1 minus the square root of real roughness m.

	half oneMinusReflectivity;
	half3 specColor;
	half3 diffColor = DiffuseAndSpecularFromMetallic (Albedo(i_tex), metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

	FragmentCommonData o = (FragmentCommonData)0;
	o.diffColor = diffColor;
	o.specColor = specColor;
	o.oneMinusReflectivity = oneMinusReflectivity;
	o.smoothness = smoothness;
	return o;
} 

inline FragmentCommonData FragmentSetup (float4 i_tex, half3 i_eyeVec, half3 i_viewDirForParallax, half4 tangentToWorld[3], half3 i_posWorld)
{
	i_tex = Parallax(i_tex, i_viewDirForParallax);

	half alpha = Alpha(i_tex.xy);
	#if defined(_ALPHATEST_ON)
		clip (alpha - _Cutoff);
	#endif

	FragmentCommonData o = UNITY_SETUP_BRDF_INPUT (i_tex);
	o.normalWorld = PerPixelWorldNormal(i_tex, tangentToWorld);
	o.eyeVec = NormalizePerPixelNormal(i_eyeVec);
	o.posWorld = i_posWorld;

	// NOTE: shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	o.diffColor = PreMultiplyAlpha (o.diffColor, alpha, o.oneMinusReflectivity, /*out*/ o.alpha);
	return o;
}

inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light, bool reflections)
{
	UnityGIInput d;
	d.light = light;
	d.worldPos = s.posWorld;
	d.worldViewDir = -s.eyeVec;
	d.atten = atten;
	#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
		d.ambient = 0;
		d.lightmapUV = i_ambientOrLightmapUV;
	#else
		d.ambient = i_ambientOrLightmapUV.rgb;
		d.lightmapUV = 0;
	#endif

	d.probeHDR[0] = unity_SpecCube0_HDR;
	d.probeHDR[1] = unity_SpecCube1_HDR;
	#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
	  d.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
	#endif
	#if UNITY_SPECCUBE_BOX_PROJECTION
	  d.boxMax[0] = unity_SpecCube0_BoxMax;
	  d.probePosition[0] = unity_SpecCube0_ProbePosition;
	  d.boxMax[1] = unity_SpecCube1_BoxMax;
	  d.boxMin[1] = unity_SpecCube1_BoxMin;
	  d.probePosition[1] = unity_SpecCube1_ProbePosition;
	#endif

	if(reflections)
	{
		Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.smoothness, -s.eyeVec, s.normalWorld, s.specColor);
		// Replace the reflUVW if it has been compute in Vertex shader. Note: the compiler will optimize the calcul in UnityGlossyEnvironmentSetup itself
		#if UNITY_OPTIMIZE_TEXCUBELOD || UNITY_STANDARD_SIMPLE
			g.reflUVW = s.reflUVW;
		#endif

		return UnityGlobalIllumination (d, occlusion, s.normalWorld, g);
	}
	else
	{
		return UnityGlobalIllumination (d, occlusion, s.normalWorld);
	}
}

inline UnityGI FragmentGI (FragmentCommonData s, half occlusion, half4 i_ambientOrLightmapUV, half atten, UnityLight light)
{
	return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, true);
}


//-------------------------------------------------------------------------------------
half4 OutputForward (half4 output, half alphaFromSurface)
{
	#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
		output.a = alphaFromSurface;
	#else
		UNITY_OPAQUE_ALPHA(output.a);
	#endif
	return output;
}

inline half4 VertexGIForward(VertexInput v, float3 posWorld, half3 normalWorld)
{
	half4 ambientOrLightmapUV = 0;
	// Static lightmaps
	#ifdef LIGHTMAP_ON
		ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
		ambientOrLightmapUV.zw = 0;
	// Sample light probe for Dynamic objects only (no static or dynamic lightmaps)
	#elif UNITY_SHOULD_SAMPLE_SH
		#ifdef VERTEXLIGHT_ON
			// Approximated illumination from non-important point lights
			ambientOrLightmapUV.rgb = Shade4PointLights (
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, posWorld, normalWorld);
		#endif

		ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, ambientOrLightmapUV.rgb);		
	#endif

	#ifdef DYNAMICLIGHTMAP_ON
		ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	#endif

	return ambientOrLightmapUV;
}

// ------------------------------------------------------------------
//  Base forward pass (directional light, emission, lightmaps, ...)

struct VertexOutputForwardBase
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;
	half4 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndParallax[3]	: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:viewDirForParallax]
	half4 ambientOrLightmapUV			: TEXCOORD5;	// SH or Lightmap UV
	SHADOW_COORDS(6)
	UNITY_FOG_COORDS(7)

	// next ones would not fit into SM2.0 limits, but they are always for SM3.0+
	#if UNITY_REQUIRE_FRAG_WORLDPOS
		float3 posWorld					: TEXCOORD8;
	#endif

	#if UNITY_OPTIMIZE_TEXCUBELOD
		#if UNITY_REQUIRE_FRAG_WORLDPOS
			half3 reflUVW				: TEXCOORD9;
		#else
			half3 reflUVW				: TEXCOORD8;
		#endif
	#endif

	UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutputForwardBase vertForwardBase (VertexInput v)
{
	VertexOutputForwardBase o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	#if UNITY_REQUIRE_FRAG_WORLDPOS
		o.posWorld = posWorld.xyz;
	#endif
	o.pos = UnityObjectToClipPos(v.vertex);
		
	o.tex = TexCoords(v);
	o.eyeVec.xyz = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	o.eyeVec.w = posWorld.y;
	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndParallax[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndParallax[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndParallax[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndParallax[0].xyz = 0;
		o.tangentToWorldAndParallax[1].xyz = 0;
		o.tangentToWorldAndParallax[2].xyz = normalWorld;
	#endif
	//We need this for shadow receving
	TRANSFER_SHADOW(o);

	o.ambientOrLightmapUV = VertexGIForward(v, posWorld, normalWorld);
	
	#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
		o.tangentToWorldAndParallax[0].w = viewDirForParallax.x;
		o.tangentToWorldAndParallax[1].w = viewDirForParallax.y;
		o.tangentToWorldAndParallax[2].w = viewDirForParallax.z;
	#endif

	#if UNITY_OPTIMIZE_TEXCUBELOD
		o.reflUVW 		= reflect(o.eyeVec, normalWorld);
	#endif

	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}

half4 fragForwardBaseInternal (VertexOutputForwardBase i)
{
	FRAGMENT_SETUP(s)
#if UNITY_OPTIMIZE_TEXCUBELOD
	s.reflUVW		= i.reflUVW;
#endif

	UnityLight mainLight = MainLight ();
	half atten = SHADOW_ATTENUATION(i);


	half occlusion = Occlusion(i.tex.xy);
	UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, mainLight);

	half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, occlusion, gi);
	c.rgb += Emission(i.tex.xy);

	//Polybox Calculate heightFog
	half heightFog = saturate((_HeightFogEnd - i.eyeVec.w) / (_HeightFogEnd-_HeightFogStart));
	heightFog = pow(heightFog,2.2);
	half4 heightFogColor = unity_FogColor;

	#ifdef _HEIGHTFOGCOLOR
		heightFogColor = _HeightFogColor;
	#endif

	c.rgb = lerp(c.rgb,heightFogColor,heightFog);

	UNITY_APPLY_FOG(i.fogCoord, c.rgb);
	return OutputForward (c, s.alpha);
}

half4 fragForwardBase (VertexOutputForwardBase i) : SV_Target	// backward compatibility (this used to be the fragment entry function)
{
	return fragForwardBaseInternal(i);
}

// ------------------------------------------------------------------
//  Additive forward pass (one light per pass)

struct VertexOutputForwardAdd
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;
	half4 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndLightDir[3]	: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:lightDir]
	LIGHTING_COORDS(5,6)
	UNITY_FOG_COORDS(7)

	// next ones would not fit into SM2.0 limits, but they are always for SM3.0+
#if defined(_PARALLAXMAP)
	half3 viewDirForParallax			: TEXCOORD8;
#endif

	UNITY_VERTEX_OUTPUT_STEREO
};

VertexOutputForwardAdd vertForwardAdd (VertexInput v)
{
	VertexOutputForwardAdd o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_OUTPUT(VertexOutputForwardAdd, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.pos = UnityObjectToClipPos(v.vertex);

	o.tex = TexCoords(v);
	o.eyeVec.xyz = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	o.eyeVec.w = posWorld.y;
	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndLightDir[0].xyz = 0;
		o.tangentToWorldAndLightDir[1].xyz = 0;
		o.tangentToWorldAndLightDir[2].xyz = normalWorld;
	#endif
	//We need this for shadow receiving
	TRANSFER_VERTEX_TO_FRAGMENT(o);

	float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
	#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = NormalizePerVertexNormal(lightDir);
	#endif
	o.tangentToWorldAndLightDir[0].w = lightDir.x;
	o.tangentToWorldAndLightDir[1].w = lightDir.y;
	o.tangentToWorldAndLightDir[2].w = lightDir.z;

	#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
	#endif
	
	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}

half4 fragForwardAddInternal (VertexOutputForwardAdd i)
{
	FRAGMENT_SETUP_FWDADD(s);

	UnityLight light = AdditiveLight (IN_LIGHTDIR_FWDADD(i), LIGHT_ATTENUATION(i));
	UnityIndirect noIndirect = ZeroIndirect ();

	half4 c = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, light, noIndirect);

	//Polybox Calculate heightFog
	half3 heightFog = saturate((_HeightFogEnd - i.eyeVec.w) / (_HeightFogEnd-_HeightFogStart));
	heightFog = pow(heightFog,2.2);
	c.rgb = lerp(c.rgb,0.0,heightFog);
	
	UNITY_APPLY_FOG_COLOR(i.fogCoord, c.rgb, half4(0,0,0,0)); // fog towards black in additive pass
	return OutputForward (c, s.alpha);
}

half4 fragForwardAdd (VertexOutputForwardAdd i) : SV_Target		// backward compatibility (this used to be the fragment entry function)
{
	return fragForwardAddInternal(i);
}

// ------------------------------------------------------------------
//  Deferred pass

struct VertexOutputDeferred
{
	float4 pos							: SV_POSITION;
	float4 tex							: TEXCOORD0;
	half3 eyeVec 						: TEXCOORD1;
	half4 tangentToWorldAndParallax[3]	: TEXCOORD2;	// [3x3:tangentToWorld | 1x3:viewDirForParallax]
	half4 ambientOrLightmapUV			: TEXCOORD5;	// SH or Lightmap UVs

	#if UNITY_REQUIRE_FRAG_WORLDPOS
		float3 posWorld						: TEXCOORD6;
	#endif

	#if UNITY_OPTIMIZE_TEXCUBELOD
		#if UNITY_REQUIRE_FRAG_WORLDPOS
			half3 reflUVW				: TEXCOORD7;
		#else
			half3 reflUVW				: TEXCOORD6;
		#endif
	#endif

	UNITY_VERTEX_OUTPUT_STEREO
};


VertexOutputDeferred vertDeferred (VertexInput v)
{
	VertexOutputDeferred o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
	#if UNITY_REQUIRE_FRAG_WORLDPOS
		o.posWorld = posWorld;
	#endif
	o.pos = UnityObjectToClipPos(v.vertex);

	o.tex = TexCoords(v);
	o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
	float3 normalWorld = UnityObjectToWorldNormal(v.normal);
	#ifdef _TANGENT_TO_WORLD
		float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

		float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
		o.tangentToWorldAndParallax[0].xyz = tangentToWorld[0];
		o.tangentToWorldAndParallax[1].xyz = tangentToWorld[1];
		o.tangentToWorldAndParallax[2].xyz = tangentToWorld[2];
	#else
		o.tangentToWorldAndParallax[0].xyz = 0;
		o.tangentToWorldAndParallax[1].xyz = 0;
		o.tangentToWorldAndParallax[2].xyz = normalWorld;
	#endif

	o.ambientOrLightmapUV = 0;
	#ifdef LIGHTMAP_ON
		o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
	#elif UNITY_SHOULD_SAMPLE_SH
		o.ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, o.ambientOrLightmapUV.rgb);
	#endif
	#ifdef DYNAMICLIGHTMAP_ON
		o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
	#endif
	
	#ifdef _PARALLAXMAP
		TANGENT_SPACE_ROTATION;
		half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
		o.tangentToWorldAndParallax[0].w = viewDirForParallax.x;
		o.tangentToWorldAndParallax[1].w = viewDirForParallax.y;
		o.tangentToWorldAndParallax[2].w = viewDirForParallax.z;
	#endif

	#if UNITY_OPTIMIZE_TEXCUBELOD
		o.reflUVW		= reflect(o.eyeVec, normalWorld);
	#endif

	return o;
}

void fragDeferred (
	VertexOutputDeferred i,
	out half4 outGBuffer0 : SV_Target0,
	out half4 outGBuffer1 : SV_Target1,
	out half4 outGBuffer2 : SV_Target2,
	out half4 outEmission : SV_Target3			// RT3: emission (rgb), --unused-- (a)
)
{
	#if (SHADER_TARGET < 30)
		outGBuffer0 = 1;
		outGBuffer1 = 1;
		outGBuffer2 = 0;
		outEmission = 0;
		return;
	#endif

	FRAGMENT_SETUP(s)
#if UNITY_OPTIMIZE_TEXCUBELOD
	s.reflUVW		= i.reflUVW;
#endif

	// no analytic lights in this pass
	UnityLight dummyLight = DummyLight ();
	half atten = 1;

	// only GI
	half occlusion = Occlusion(i.tex.xy);
#if UNITY_ENABLE_REFLECTION_BUFFERS
	bool sampleReflectionsInDeferred = false;
#else
	bool sampleReflectionsInDeferred = true;
#endif

	UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);

	half3 emissiveColor = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;
	emissiveColor += UNITY_BRDF_GI (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, occlusion, gi);

	#ifdef _EMISSION
		emissiveColor += Emission (i.tex.xy);
	#endif

	#ifndef UNITY_HDR_ON
		emissiveColor.rgb = exp2(-emissiveColor.rgb);
	#endif

	UnityStandardData data;
	data.diffuseColor	= s.diffColor;
	data.occlusion		= occlusion;		
	data.specularColor	= s.specColor;
	data.smoothness		= s.smoothness;	
	data.normalWorld	= s.normalWorld;

	UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

	// Emisive lighting buffer
	outEmission = half4(emissiveColor, 1);
}


//
// Old FragmentGI signature. Kept only for backward compatibility and will be removed soon
//

inline UnityGI FragmentGI(
	float3 posWorld,
	half occlusion, half4 i_ambientOrLightmapUV, half atten, half smoothness, half3 normalWorld, half3 eyeVec,
	UnityLight light,
	bool reflections)
{
	// we init only fields actually used
	FragmentCommonData s = (FragmentCommonData)0;
	s.smoothness = smoothness;
	s.normalWorld = normalWorld;
	s.eyeVec = eyeVec;
	s.posWorld = posWorld;
#if UNITY_OPTIMIZE_TEXCUBELOD
	s.reflUVW = reflect(eyeVec, normalWorld);
#endif
	return FragmentGI(s, occlusion, i_ambientOrLightmapUV, atten, light, reflections);
}
inline UnityGI FragmentGI (
	float3 posWorld,
	half occlusion, half4 i_ambientOrLightmapUV, half atten, half smoothness, half3 normalWorld, half3 eyeVec,
	UnityLight light)
{
	return FragmentGI (posWorld, occlusion, i_ambientOrLightmapUV, atten, smoothness, normalWorld, eyeVec, light, true);
}

#endif // POLYBOX_CORE_INCLUDED
