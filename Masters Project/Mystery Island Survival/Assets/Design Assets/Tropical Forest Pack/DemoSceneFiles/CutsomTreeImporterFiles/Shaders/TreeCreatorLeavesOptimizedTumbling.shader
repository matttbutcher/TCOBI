Shader "Nature/Tree Creator Leaves Optimized Tumbling" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TranslucencyColor ("Translucency Color", Color) = (0.73,0.85,0.41,1) // (187,219,106,255)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.3
	_TranslucencyViewDependency ("View dependency", Range(0,1)) = 0.7
	_ShadowStrength("Shadow Strength", Range(0,1)) = 0.8
	
	_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	_BumpSpecMap ("Normalmap (GA) Spec (R) Shadow Offset (B)", 2D) = "bump" {}
	_TranslucencyMap ("Trans (B) Gloss(A)", 2D) = "white" {}

	_TumbleStrength("Tumble Strength", Range(0,1)) = 0.1
	_TumbleFrequency("Tumble Frequency", Range(0,4)) = 1

	_LeafTurbulence("Leaf Turbulence", Range(0,1)) = 0.5

	// These are here only to provide default values
	[HideInInspector] _TreeInstanceColor ("TreeInstanceColor", Vector) = (1,1,1,1)
	[HideInInspector] _TreeInstanceScale ("TreeInstanceScale", Vector) = (1,1,1,1)
	[HideInInspector] _SquashAmount ("Squash", Float) = 1
}

SubShader { 
	Tags {
		"IgnoreProjector"="True"
		"RenderType"="CTI-TreeLeaf"
	}
	LOD 200

CGPROGRAM
#pragma surface surf TreeLeaf alphatest:_Cutoff vertex:CTI_TreeVertLeaf nolightmap noforwardadd addshadow
#pragma target 3.0
#include "UnityBuiltin3xTreeLibrary.cginc"
#define LEAFTUMBLING
#include "Includes/CTI_Builtin3xTreeLibraryTumbling.cginc"

sampler2D _MainTex;
sampler2D _BumpSpecMap;
sampler2D _TranslucencyMap;

struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR; // color.a = AO
};

void surf (Input IN, inout LeafSurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * IN.color.rgb * IN.color.a;
	
	fixed4 trngls = tex2D (_TranslucencyMap, IN.uv_MainTex);
	o.Translucency = trngls.b;
	o.Gloss = trngls.a * _Color.r;
	o.Alpha = c.a;
	
	half4 norspc = tex2D (_BumpSpecMap, IN.uv_MainTex);
	o.Specular = norspc.r;
	o.Normal = UnpackNormalDXT5nm(norspc);

//	o.Albedo = half3(IN.color.r, 0, 0);
}
ENDCG

}

Dependency "BillboardShader" = "Hidden/Nature/Tree Creator Leaves Rendertex"
}
