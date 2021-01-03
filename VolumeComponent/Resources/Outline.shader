Shader "SleeplessOwl/Post-Processing/Outline"
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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uvShift[9] : TEXCOORD1;
			};

			TEXTURE2D(_PostSource);
			TEXTURE2D(_CameraDepthTexture);
			SAMPLER(sampler_PostSource);

			float4 _PostSource_TexelSize;
			float _sampleDistance;
			float _strengthPow;

			float Sobel(float2 uv[9])
			{
				const float gx[9] =
				{
					-1, -2, -1,
					0,  0,  0,
					1,  2,  1
				};

				const float gy[9] =
				{
					1, 0, -1,
					2, 0, -2,
					1, 0, -1
				};

				float edgeX, edgeY;
				for (int j = 0; j < 9; j++)
				{
					float3 col = SAMPLE_TEXTURE2D(_PostSource, sampler_PostSource, uv[j]).rgb;

					float lum = (col.r + col.g + col.b) * 0.3333333;

					edgeX += lum * gx[j];
					edgeY += lum * gy[j];
				}
				return 1 - abs(edgeX) - abs(edgeY);
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = GetFullScreenTriangleVertexPosition(v.vertexID);
				o.uv.xy = GetFullScreenTriangleTexCoord(v.vertexID);

				//_MainTex_TexelSize => Vector4(1 / width, 1 / height, width, height)
				float2 uvShift = _PostSource_TexelSize.xy * _sampleDistance;
				o.uvShift[0] = o.uv + float2(-1, -1) * uvShift;
				o.uvShift[1] = o.uv + float2(0, -1) * uvShift;
				o.uvShift[2] = o.uv + float2(1, -1) * uvShift;
				o.uvShift[3] = o.uv + float2(-1, 0) * uvShift;
				o.uvShift[4] = o.uv + float2(0, 0) * uvShift;
				o.uvShift[5] = o.uv + float2(1, 0) * uvShift;
				o.uvShift[6] = o.uv + float2(-1, 1) * uvShift;
				o.uvShift[7] = o.uv + float2(0, 1) * uvShift;
				o.uvShift[8] = o.uv + float2(1, 1) * uvShift;
				return o;
			}

			inline float Linear01Depth(float z)
			{
				return 1.0 / (_ZBufferParams.x * z + _ZBufferParams.y);
			}

			float4 frag(v2f i) : SV_Target
			{
				float outline = Sobel(i.uvShift);
				outline = pow(outline, _strengthPow);
				outline = step(0.5, outline);

				float3 colOri = LOAD_TEXTURE2D(_PostSource, i.uv.xy * _ScreenParams.xy).rgb;
				float depth = Linear01Depth(LOAD_TEXTURE2D(_CameraDepthTexture, i.uv.xy * _ScreenParams.xy).r);
				float3 colOutline = min(1, (outline + .8)) * colOri.rgb;

				////ignore mid view
				//depth *= 4;
				//float value = pow(depth - .5, 8) * 256;
				//value = pow(saturate(value + .2), 2);
				//return float4(lerp(colOri, colOutline, value), 1);

				return float4(colOutline, 1);
			}
			ENDHLSL
		}
	}
}