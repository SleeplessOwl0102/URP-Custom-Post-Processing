Shader "SleeplessOwl/Post-Processing/Cloud Shadow"
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

			#pragma multi_compile_local __ DEBUG_MODE

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 ray : TEXCOORD2;
			};

			TEXTURE2D_X(_PostSource);
			TEXTURE2D_X(_CameraDepthTexture);

			TEXTURE2D_X(_NoiseTexture1);
			TEXTURE2D_X(_NoiseTexture2);
			
			SAMPLER(sampler_NoiseTexture1);
			SAMPLER(sampler_NoiseTexture2);

			SAMPLER(sampler_PostSource);

			float4 _PostSource_TexelSize;
			float4x4 _InverseView;

			float _ShadowStrength;
			float2 _SmoothStepRange;

			float _NoiseTime1;
			float2 _NoiseScale1;

			float _NoiseTime2;
			float2 _NoiseScale2;

			half2 Fade2(half2 t)
			{
				return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
			}

			half2 Hash22(half2 p)
			{
				p = half2(dot(p, half2(127.1, 311.7)),
					dot(p, half2(269.5, 183.3)));

				return -1.0 + 2.0 * frac(sin(p) * 43758.5453123) * 0.8;
			}
			
			half PerlinNoise_2D(half2 p)
			{
				half2 pi = floor(p);
				half2 pf = p - pi;
				half2 t = Fade2(pf);

				half2 p1 = half2(0, 0);
				half2 p2 = half2(1, 0);
				half2 p3 = half2(0, 1);
				half2 p4 = half2(1, 1);

				half g1 = dot(Hash22(pi + p1), pf - p1);
				half g2 = dot(Hash22(pi + p2), pf - p2);
				half g3 = dot(Hash22(pi + p3), pf - p3);
				half g4 = dot(Hash22(pi + p4), pf - p4);

				half x1 = lerp(g1, g2, t.x);
				half x2 = lerp(g3, g4, t.x);

				half y1 = lerp(x1, x2, t.y);

				return y1 + 0.5;
			}

			v2f vert(uint vertexID : SV_VertexID)
			{
				v2f o;
				o.vertex = GetFullScreenTriangleVertexPosition(vertexID);
				o.uv.xy = GetFullScreenTriangleTexCoord(vertexID);

				float far = _ProjectionParams.z;
				float4 clipPos = float4(o.vertex.x, -o.vertex.y, 1, 1);// need negative y, but why???
				float3 viewPos = mul(unity_CameraInvProjection, clipPos * far);
				o.ray = mul(_InverseView, float4(viewPos, 0));
				return o;
			}

			//Directional light world space direction
			float3 _WorldSpaceLightPos0;

			float4 frag(v2f i) : SV_Target
			{
				float3 oriCol = LOAD_TEXTURE2D_X(_PostSource, i.vertex.xy).rgb;
				float depth = LOAD_TEXTURE2D_X(_CameraDepthTexture, i.vertex.xy).r;
				depth = Linear01Depth(depth, _ZBufferParams);

				if (depth < .9)
				{
					float3 pos = _WorldSpaceCameraPos + i.ray.xyz * depth;

					//calculate light direction offset by Y-Axis
					float2 heightOffset = (pos.y / _WorldSpaceLightPos0.y) * _WorldSpaceLightPos0.xz;
					pos.xz += heightOffset;

					float2 uv = pos.xz * _NoiseScale1 + _Time.y * _NoiseTime1;
					float noise1 = PerlinNoise_2D(uv).r;

					/*
					uv = pos.xz * _NoiseScale2 + _Time.y * _NoiseTime2;
					float value2 = SAMPLE_TEXTURE2D_X(_NoiseTexture2, sampler_NoiseTexture2, uv).r;
					*/
	
					float  value = smoothstep(_SmoothStepRange.x, _SmoothStepRange.y, noise1);
#if DEBUG_MODE
					return float4(value.xxx, 1);
#endif

					float3 col = oriCol * value;
					
					oriCol = lerp(oriCol, col, _ShadowStrength * pow((1- depth),10));
				}
				return float4(oriCol, 1);
			}
			ENDHLSL
		}
	}
}