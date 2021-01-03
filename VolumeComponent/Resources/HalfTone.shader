Shader "SleeplessOwl/Post-Processing/HalfTone"
{
	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Off

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct appdata
			{
				uint vertexID : SV_VertexID;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

			};

			float _Density;
			float _Radius;
			float _SmoothEdge;
			float _HalfToneFactor;
			float _SourceFactor;
			float _Lightness;
			float4 _Color01;
			float4 _ColorFactor;

			TEXTURE2D_X(_PostSource);
			SAMPLER(sampler_PostSource);

			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = GetFullScreenTriangleVertexPosition(v.vertexID);
				o.uv.xy = GetFullScreenTriangleTexCoord(v.vertexID);

				float aspect = _ScreenParams.x / _ScreenParams.y;
				float2 uu;
				uu.x = o.uv.x * aspect;
				uu.y = o.uv.y;
				float angle = sin(radians(45));
				o.uv.zw = mul(float2x2(angle, -angle, angle, angle), uu);
				return o;
			}

			sampler2D _halfToneFlag;

			float4 frag(v2f i) : SV_Target
			{

				float3 texColor = SAMPLE_TEXTURE2D_X(_PostSource, sampler_PostSource, i.uv.xy).rgb;

				float lightness = (texColor.r * _ColorFactor.r + texColor.g * _ColorFactor.g + texColor.b * _ColorFactor.b) * _Lightness;
				float radius = 1 - lightness + _Radius;

				/*
				//如果項目是2D Platform類遊戲，並且有一個主要focus的深度，可以使用距離對UV做位移，讓在主要深度上的點點看起來是固定不動的。
				//以下為 FOV 60  深度 8 的例子
				//9.2376043070 = tan(30(half camera fov) * 8(z depth) * 2)
				//compute shift let post effect point no move on Z-axis equal 0
				
				i.uv.zw = frac(i.uv.zw + _WorldSpaceCameraPos.x / 9.2376043070 * sin(radians(45)));
				i.uv.z = frac(i.uv.z - _WorldSpaceCameraPos.y / 9.2376043070 * sin(radians(45)));
				i.uv.w = frac(i.uv.w + _WorldSpaceCameraPos.y / 9.2376043070 * sin(radians(45)));
				*/

				float2 vectorCenter = 2 * frac(_Density * i.uv.zw) - 1;
				float distance = length(vectorCenter);

				float circle = 1 - smoothstep(radius, radius + _SmoothEdge, distance);
				float3 halftoneColor = lerp(float4(1,1,1,1), _Color01, circle).rgb;

				halftoneColor = texColor * halftoneColor * _HalfToneFactor;

				float3 color = texColor * _SourceFactor + halftoneColor;
				return float4(color, 1);
			}
			ENDHLSL
		}
	}
}