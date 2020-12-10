Shader "Owl Post-Processing/Glitch"
{
    SubShader
    {
        Pass
        {
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
