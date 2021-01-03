Shader "SleeplessOwl/Post-Processing/World Position"
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

			#pragma multi_compile_local __ ADD_MODE

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
			SAMPLER(sampler_PostSource);

			float4 _PostSource_TexelSize;
			float4x4 _InverseView;

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

			float _UnitCubeGridCount;
			float _GridLineWidth;

			float4 frag(v2f i) : SV_Target
			{
				//i.vertex.xy == i.uv.xy * _ScreenParams.xy
				float depth = LOAD_TEXTURE2D_X(_CameraDepthTexture, i.vertex.xy).r;
				depth = Linear01Depth(depth, _ZBufferParams);
				float drawPass = depth < .9;

				float3 pos = _WorldSpaceCameraPos + i.ray.xyz * depth;
				pos *= _UnitCubeGridCount;

				// Detect borders with using derivatives.
				float3 fw = fwidth(pos);
				half3 bc = saturate(_GridLineWidth - abs(1 - 2 * frac(pos)) / fw * 1.5);
				
				// Frequency filter
				half3 f1 = smoothstep(1 / _UnitCubeGridCount, 2 / _UnitCubeGridCount, fw);
				half3 f2 = smoothstep(2 / _UnitCubeGridCount, 4 / _UnitCubeGridCount, fw);
				bc = lerp(lerp(bc, 0.5, f1), 0, f2);
				
				float3 oriCol = LOAD_TEXTURE2D_X(_PostSource, i.vertex.xy).rgb;
				float3 combineCol;
				combineCol = lerp(oriCol, bc, length(bc) * drawPass);
#if ADD_MODE
				combineCol = oriCol + bc * drawPass;
#endif

				return float4(combineCol, 1);
			}
			ENDHLSL
		}
	}
}