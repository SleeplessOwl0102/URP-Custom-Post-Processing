#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes
{
	uint vertexID : SV_VertexID;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
	float4 pos : SV_POSITION;
	float2 uv : TEXCOORD0;
};

Varyings Vertex(Attributes input)
{
	Varyings output;
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
	output.pos = GetFullScreenTriangleVertexPosition(input.vertexID);
	output.uv = GetFullScreenTriangleTexCoord(input.vertexID);
	return output;
}

float _Speed;
float _BlockSize;
float _MaxRGBSplitX;
float _MaxRGBSplitY;


TEXTURE2D_X(_PostSource);
SAMPLER(sampler_PostSource);

float FRandom(uint seed)
{
	return GenerateHashedRandomFloat(seed);
}

float randomNoise(float2 seed)
{
	return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
}

float randomNoise(float seed)
{
	return randomNoise(float2(seed, 1.0));
}

float3 SampleTexture(float2 uv)
{
	return SAMPLE_TEXTURE2D_X(_PostSource, sampler_PostSource, uv);
}

float4 Fragment(Varyings i) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
	
	half2 block = randomNoise(floor(i.uv * _BlockSize));

	float displaceNoise = pow(block.x, 20);
	float splitRGBNoise = pow(randomNoise(7.2341), 17.0);
	float offsetX = displaceNoise - splitRGBNoise * _MaxRGBSplitX;
	float offsetY = displaceNoise - splitRGBNoise * _MaxRGBSplitY;

	float noiseX = 0.05 * randomNoise(13.0);
	float noiseY = 0.05 * randomNoise(7.0);
	float2 offset = float2(offsetX * noiseX, offsetY * noiseY);

	half3 colorR = SampleTexture(i.uv);
	half3 colorG = SampleTexture(i.uv + offset);
	half3 colorB = SampleTexture(i.uv - offset);

	return half4(colorR.r, colorG.g, colorB.b, 1);
	
}
