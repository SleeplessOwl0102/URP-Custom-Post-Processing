Shader "URP Custom PostEffect/HalfTone"
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
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

			TEXTURE2D(_PostSource);
			//SAMPLER(sampler_PostSource);

			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
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
				float3 texColor = LOAD_TEXTURE2D(_PostSource, i.uv.xy * _ScreenParams.xy).rgb;
				//float3 texColor = SAMPLE_TEXTURE2D(_PostSource, sampler_PostSource, i.uv.xy).rgb;

				float lightness = (texColor.r * _ColorFactor.r + texColor.g * _ColorFactor.g + texColor.b * _ColorFactor.b) * _Lightness;
				float radius = 1 - lightness + _Radius;

				//9.2376043070 = tan(30(half camera fov) * 8(z depth) * 2)
				//compute shift let post effect point no move on Z-axis equal 0
				i.uv.zw = frac(i.uv.zw + _WorldSpaceCameraPos.x / 9.2376043070 * sin(radians(45)));
				i.uv.z = frac(i.uv.z - _WorldSpaceCameraPos.y / 9.2376043070 * sin(radians(45)));
				i.uv.w = frac(i.uv.w + _WorldSpaceCameraPos.y / 9.2376043070 * sin(radians(45)));

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