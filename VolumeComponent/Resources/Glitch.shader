Shader "SleeplessOwl/Post-Processing/Glitch"
{
    SubShader
    {
        Pass
        {
            Name "Glitch"
            Cull Off ZWrite Off ZTest Always
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "Glitch.hlsl"
            ENDHLSL
        }

    }
    Fallback Off
}
