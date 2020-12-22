Shader "SleeplessOwl/Post-Process/Example"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    struct appdata
    {
        uint vertexID: SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float4 uv: TEXCOORD0;
        float4 vertex: SV_POSITION;
    };

    TEXTURE2D(_PostSource);

    v2f vert(appdata v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        o.vertex = GetFullScreenTriangleVertexPosition(v.vertexID);
        o.uv.xy = GetFullScreenTriangleTexCoord(v.vertexID);

        return o;
    }

    float3 paintHeart(float3 col, float3 col1, float x, float y)
    {
        float r = x * x + pow((y - pow(x * x, 1.0 / 4.0)), 2.0);
        r -= pow(abs(sin(_Time.z)), 5.0);

        if (r < 1.5)
        {
            col = col1 * r;
        }

        return col;
    }

    float4 frag(v2f i): SV_Target
    {
        float3 col = LOAD_TEXTURE2D(_PostSource, i.uv.xy * _ScreenParams.xy).rgb;

        float2 p = 4.0 * - i.uv;
        float x = p.x + 2.0;
        float y = p.y + 2.5;

        float3 col2 = float3(1, 0, 0);
        col = paintHeart(col, col2, x, y);

        return float4(col, 1);
    }
    ENDHLSL
    
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
			
            ENDHLSL
        }
    }
}