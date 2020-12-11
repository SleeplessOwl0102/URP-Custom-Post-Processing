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
    float4 positionCS : SV_POSITION;
    float2 texcoord   : TEXCOORD0;
};

Varyings Vertex(Attributes input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
    output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
    return output;
}

uint _Seed;

float _BlockStrength;
uint _BlockStride;
uint _BlockSeed1;
uint _BlockSeed2;

float2 _Drift;
float2 _Jitter;
float2 _Jump;
float _Shake;

TEXTURE2D(_PostSource);

float FRandom(uint seed)
{
    return GenerateHashedRandomFloat(seed);
}

float4 Fragment(Varyings input) : SV_Target
{
    float2 uv = input.texcoord;

    uint block_size = 32;
	uint columns = _ScreenParams.x / block_size;

    // Block index
	uint2 block_xy = input.texcoord * _ScreenParams.xy / block_size;
    uint block = block_xy.y * columns + block_xy.x;

    // Segment index
    uint segment = block / _BlockStride;

    // Per-block random number
    float r1 = FRandom(block     + _BlockSeed1);
    float r3 = FRandom(block / 3 + _BlockSeed2);
    uint seed = (r1 + r3) < 1 ? _BlockSeed1 : _BlockSeed2;
    float rand = FRandom(segment + seed);

    // Block damage (offsetting)
    block += rand * 20000 * (rand < _BlockStrength);

    // Screen space position reconstruction
    uint2 ssp = uint2(block % columns, block / columns) * block_size;
	ssp += (uint2) (input.texcoord * _ScreenParams.xy) % block_size;

    // UV recalculation
	uv = frac((ssp + 0.5) / _ScreenParams.xy);

	float4 c = LOAD_TEXTURE2D(_PostSource, uv * _ScreenParams.xy);

    // Block damage (color mixing)
    if (frac(rand * 1234) < _BlockStrength * 0.1)
    {
        float3 hsv = RgbToHsv(c.rgb);
        hsv = hsv * float3(-1, 1, 0) + float3(0.5, 0, 0.9);
        c.rgb = HsvToRgb(hsv);
    }


    return c;
}
