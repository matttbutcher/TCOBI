// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef CTI_BUILTIN_3X_TREE_LIBRARY_INCLUDED
#define CTI_BUILTIN_3X_TREE_LIBRARY_INCLUDED

float _TumbleStrength;
float _TumbleFrequency;
float _LeafTurbulence;

// see http://www.neilmendoza.com/glsl-rotation-about-an-arbitrary-axis/
float3x3 AfsRotationMatrix(float3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;

    return float3x3	(	oc * axis.x * axis.x + c,			oc * axis.x * axis.y - axis.z * s,	oc * axis.z * axis.x + axis.y * s,
                		oc * axis.x * axis.y + axis.z * s,	oc * axis.y * axis.y + c,			oc * axis.y * axis.z - axis.x * s,
                		oc * axis.z * axis.x - axis.y * s,	oc * axis.y * axis.z + axis.x * s,	oc * axis.z * axis.z + c);   
}

// Detail bending
inline float4 CTI_AnimateVertex(float4 pos, float3 normal, float4 animParams, float3 pivot, float tumbleStrength)
{	
	// animParams stored in color
	// animParams.x = branch phase
	// animParams.y = edge flutter factor
	// animParams.z = primary factor
	// animParams.w = secondary factor

	float fDetailAmp = 0.1f;
	float fBranchAmp = 0.3f;
	
	// Phases (object, vertex, branch)
	float fObjPhase = dot(unity_ObjectToWorld[3].xyz, 1);
	float fBranchPhase = fObjPhase + animParams.x;
	float fVtxPhase = dot(pos.xyz, animParams.y + fBranchPhase);
	
	// x is used for edges; y is used for branches
	float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase );
	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
	vWaves = SmoothTriangleWave( vWaves );
	float2 vWavesSum = vWaves.xz + vWaves.yw;

//	Tumbling / Should be done before all other deformations
	#if defined (LEAFTUMBLING)
		if(_TumbleStrength > 0 && tumbleStrength > 0) {
			// _Wind.w is turbulence
			// Move point to 0,0,0
			pos.xyz -= pivot;
			// Add variance to the different leaf planes
			float3 fracs = frac(pivot * 33.3); // + pos.w
	 		float offset = fracs.x + fracs.y + fracs.z;
			float tFrequency = _TumbleFrequency * _Time.y; // + _Wind.w;
			// Add different speeds: (1.0 + offset * 0.25)
			float4 vWaves1 = SmoothTriangleWave( float4( (tFrequency + offset) * (1.0 + offset * 0.25), tFrequency * 0.75 - offset, tFrequency * 0.1 + offset, tFrequency * 1.0 + offset));
			//float3 windTangent = cross(normalize(-pivot), _Wind.xyz);
			float3 windDir = normalize (_Wind.xyz);
			float facingWind =  (dot(normalize(float3(pos.x, 0, pos.z)), windDir)); //saturate 
			float3 windTangent = float3(-windDir.z, windDir.y, windDir.x);
			float twigPhase = vWaves1.x + vWaves1.y + vWaves1.z;
			// Use abs(_Wind)!!!!!!
			float windStrength = dot( abs(_Wind.xyz), 1) * pos.w * (1.35 - facingWind) * _Wind.w;
			pos.xyz = mul(AfsRotationMatrix(windTangent, windStrength * (twigPhase + fBranchPhase * 0.25 ) * _TumbleStrength * tumbleStrength ), pos.xyz);
			// Move point back to origin
			pos.xyz += pivot;
		}
	#endif

//	Preserve Length
	float origLength = length(pos.xyz);

	// Edge (xz) and branch bending (y)
	float3 bend = animParams.y * fDetailAmp * normal * sign(normal);
//	bend.y = animParams.w * fBranchAmp;
	bend.y = (animParams.w + animParams.y * _LeafTurbulence) * fBranchAmp;
	pos.xyz += ((vWavesSum.xyx * bend) + (_Wind.xyz * vWavesSum.y * animParams.w)) * _Wind.w;

//	Primary bending / Displace position
	pos.xyz += animParams.z * _Wind.xyz;

//	Preserve Length
	pos.xyz = normalize(pos.xyz) * origLength; // Reduces alse secondary bending...
	
	return pos;
}

void CTI_TreeVertLeaf (inout appdata_full v)
{
	ExpandBillboard (UNITY_MATRIX_IT_MV, v.vertex, v.normal, v.tangent);
	v.vertex.xyz *= _TreeInstanceScale.xyz;
	//	Decode UV3
	float3 pivot;
	float3 growDir;
	#if defined(LEAFTUMBLING)
		// 8 bit compression
		//pivot = (frac(float3(1.0f, 256.0f, 65536.0f) * v.texcoord2.x) * 2) - 1;
		// 10 bit compression
		//pivot = (frac(float3(1.0f, 1024.0f, 1048576.0f) * v.texcoord2.x) * 2) - 1;
		// 15bit compression 2 components only, important: sign of y
		pivot.xz = (frac(float2(1.0f, 32768.0f) * v.texcoord2.xx) * 2) - 1;
		pivot.y = sqrt(1 - saturate(dot(pivot.xz, pivot.xz)));
	//	growDir = -pivot;
	//	growDir = normalize(mul((float3x3)_Object2World, -pivot));

		pivot *= v.texcoord2.y;
		pivot *= _TreeInstanceScale.xyz;
	#endif
	v.vertex = CTI_AnimateVertex( float4(v.vertex.xyz, v.color.b), v.normal, float4(v.color.xy, v.texcoord1.xy), pivot, v.color.b);
	v.vertex = Squash(v.vertex);
	v.color.rgb = _TreeInstanceColor.rgb * _Color.rgb;
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz);

	//v.color.r = v.texcoord1.y;
}

void CTI_TreeVertBark (inout appdata_full v)
{
	v.vertex.xyz *= _TreeInstanceScale.xyz;
	v.vertex = CTI_AnimateVertex( float4(v.vertex.xyz, v.color.b), v.normal, float4(v.color.xy, v.texcoord1.xy), float3(0,0,0), 0);
	v.vertex = Squash(v.vertex);
	v.color.rgb = _TreeInstanceColor.rgb * _Color.rgb;
	v.normal = normalize(v.normal);
	v.tangent.xyz = normalize(v.tangent.xyz); 
}

#endif // CTI_BUILTIN_3X_TREE_LIBRARY_INCLUDED
